using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Movies.Dtos;

namespace MoviesAPI.Services.Movies
{
    internal class TmdbClient : IMovieSearcher
    {
        private readonly TmdbUrlBuilder tmdbUrlBuilder;
        public TmdbClient(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("apiKey cannot be null or empty");

            tmdbUrlBuilder = new TmdbUrlBuilder(apiKey);
        }


        public async Task<IEnumerable<LiteMovieDto>> SearchMoviesAsync(string movieName)
        {
            var response = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildSearchUrl(movieName));

            return ToLiteMovieDtos(response);
        }

        public async Task<IEnumerable<LiteMovieDto>> GetMoviesOfTodayAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildMoviesOfTodayUrl());

            var liteMovieDtos = ToLiteMovieDtos(results).ToList();//ToList() is needed to update LogoImageUrl on each element

            var tasks = new List<Task>();
            var movieLogos = new List<Tuple<string, string>>();

            foreach (var movie in liteMovieDtos)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var logoImgUrl = await GetLogoImageUrlAsync(movie.Id);
                    movieLogos.Add(new Tuple<string, string>(movie.Id, logoImgUrl));
                }));
                 
            }

            await Task.WhenAll(tasks);

            foreach (var movieDto in liteMovieDtos)
            {
                movieDto.LogoImageUrl = movieLogos.SingleOrDefault(m => m.Item1 == movieDto.Id)?.Item2;
            }

            return liteMovieDtos.OrderBy(m => m.Rating);
        }

        public async Task<IEnumerable<LiteMovieDto>> GetPopularMoviesAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularMoviesUrl());

            return ToLiteMovieDtos(results);
        }

        public async Task<IEnumerable<LiteMovieDto>> GetRecommandationsAsync(string tmdbMovieId)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildRecommandationsUrl(tmdbMovieId));

            return ToLiteMovieDtos(results);
        }

        public async Task<IEnumerable<LiteMovieDto>> GetSimilarMoviesAsync(string tmdbMovieId)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildSimilarMoviesUrl(tmdbMovieId));

            return ToLiteMovieDtos(results);
        }

        public async Task<string> GetFrenchTitleAsync(string tmdbMovieId)
        {
            var tmdbTranslations = await HttpRequester.GetAsync<TmdbTranslations>(tmdbUrlBuilder.BuildTranslationUrl(tmdbMovieId));

            var frenchTitle = tmdbTranslations?.Translations?.FirstOrDefault(t => t.CountryCode == "FR")?.Data.Title;

            return frenchTitle;
        }

        private async Task<string> GetMovieIdAsync(string originalTitle, int year)
        {
            var tmdbResults = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildSearchUrl(originalTitle));

            return tmdbResults?.Results?.FirstOrDefault(r => r.Title.Equals(originalTitle, StringComparison.OrdinalIgnoreCase) && r.Year == year)?.Id;
        }

        private async Task<string> GetLogoImageUrlAsync(string movieId)
        {
            var tmdbImages = await HttpRequester.GetAsync<TmdbImages>(tmdbUrlBuilder.BuildGetImagesUrl(movieId));

            var filePath = tmdbImages?.Logos.Where(l => string.IsNullOrEmpty(l.CountryCode) || l.CountryCode == "en").OrderBy(l => l.VoteAverage).FirstOrDefault()?.FilePath;

            return String.IsNullOrEmpty(filePath) ? null : tmdbUrlBuilder.BuildLogoImageUrl(filePath);
        }

     
        public async Task<MovieDto> GetMovieDetailsAsync(string movieId)
        {
            var tmdbSearchResult = await HttpRequester.GetAsync<TmdbSearchResult>(tmdbUrlBuilder.BuildMovieDetailsUrl(movieId));
            var logoImgUrl = await GetLogoImageUrlAsync(movieId);
            var tmdbCredits = await GetMovieCredits(movieId);
           
            return new MovieDto()
            {
                Title = tmdbSearchResult.Title,
                Year = tmdbSearchResult.ReleaseDate?.Split('-')[0],
                CoverImageUrl = tmdbUrlBuilder.BuildCoverImageUrl(tmdbSearchResult.PosterPath),
                BackgroundImageUrl = tmdbUrlBuilder.BuildBackgroundImageUrl(tmdbSearchResult.BackdropPath),
                LogoImageUrl = logoImgUrl,
                Rating = Math.Round(tmdbSearchResult.VoteAverage, 1),
                Id = tmdbSearchResult.Id,
                Synopsis = tmdbSearchResult.Overview,
                Genre = tmdbSearchResult.Genres.Select(g => g.Name).Aggregate((a, b) => $"{a}, {b}"),
                Duration = tmdbSearchResult.Runtime.GetValueOrDefault(0),
                YoutubeTrailerUrl = GetYoutubeTrailerUrlVideo(tmdbSearchResult.Videos.Results),
                Cast = tmdbCredits?.Cast?.Take(4).Select(c => c.Name).Aggregate((a, b) => $"{a}, {b}"),
                Director = tmdbCredits?.Crew?.FirstOrDefault(c => c.Job.Equals("director", StringComparison.OrdinalIgnoreCase))?.Name,
                ImdbId = tmdbSearchResult.ImdbId
            };
        }

        public async Task<IEnumerable<LiteMovieDto>> GetMoviesByGenreAsync(int genreId, int page)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildMoviesByGenreUrl(genreId, page));

            return ToLiteMovieDtos(results);
        }


        public async Task<IEnumerable<LiteMovieDto>> GetPopularMoviesByGenreAsync(int genreId)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularMoviesByGenreUrl(genreId));

            return ToLiteMovieDtos(results);
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            var result = await HttpRequester.GetAsync<TmdbGenres>(tmdbUrlBuilder.BuilGenresListUrl());

            var genreIdsToRemove = new int[] { 10402, 9648, 10770, 10752, 37, 36 };

            return result?.Genres.Where(g => !genreIdsToRemove.Contains(g.Id));
        }

        public async Task<IEnumerable<LiteMovieDto>> GetTopNetflixMoviesAsync()
        {
            var moviesName = await NetflixRequester.GetTopNetflixMoviesNameAsync();

            var tasks = new List<Task<LiteMovieDto>>();

            foreach (var movieName in moviesName)
            {
                tasks.Add(SearchMoviesAsync(movieName).ContinueWith(t => t.Result.FirstOrDefault(t => t.Title.Equals(movieName, StringComparison.OrdinalIgnoreCase))));
            }

            return await Task.WhenAll(tasks).ContinueWith(t => t.Result.Where(m => m != null));
        }

        public async Task<IEnumerable<LiteMovieDto>> GetPopularNetflixMoviesAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularMoviesByPlatformUrl(8));
            results.Results = results.Results.OrderByDescending(r => r.VoteCount).ToArray();

            return ToLiteMovieDtos(results);
        }

        public async Task<IEnumerable<LiteMovieDto>> GetPopularDisneyPlusMoviesAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularMoviesByPlatformUrl(337));

            return ToLiteMovieDtos(results);
        }

        public async Task<IEnumerable<LiteMovieDto>> GetPopularAmazonPrimeMoviesAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularMoviesByPlatformUrl(9));

            return ToLiteMovieDtos(results);
        }

        private async Task<TmdbCredits> GetMovieCredits(string movieId)
        {
            return await HttpRequester.GetAsync<TmdbCredits>(tmdbUrlBuilder.BuildGetCreditsUrl(movieId));
        }

        private string GetYoutubeTrailerUrlVideo(TmdbVideo[] tmdbVideos)
        {
            var youtubeVideos = tmdbVideos.Where(v => v.Site.Equals("youtube", StringComparison.OrdinalIgnoreCase));

            var trailerVideo = youtubeVideos?.FirstOrDefault(v => v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase))
                                ?? youtubeVideos?.FirstOrDefault();

            return trailerVideo != null ? $"https://www.youtube.com/embed/{trailerVideo?.Key}?rel=0&wmode=transparent&border=0&autoplay=1&iv_load_policy=3" : null;
        }

        private IEnumerable<LiteMovieDto> ToLiteMovieDtos(TmdbSearchResults tmdbSearchResults)
        {
            return tmdbSearchResults?.Results?.Where(r => r.VoteCount > 0).Select(r => ToLiteMovieDto(r));
        }

        private LiteMovieDto ToLiteMovieDto(TmdbSearchResult tmdbSearchResult)
        {
            return new LiteMovieDto()
            {
                Title = tmdbSearchResult.Title,
                Year = tmdbSearchResult.ReleaseDate?.Split('-')[0],
                CoverImageUrl = tmdbUrlBuilder.BuildCoverImageUrl(tmdbSearchResult.PosterPath),
                BackgroundImageUrl = tmdbUrlBuilder.BuildBackgroundImageUrl(tmdbSearchResult.BackdropPath),
                Rating = Math.Round(tmdbSearchResult.VoteAverage, 1),
                Id = tmdbSearchResult.Id,
                Synopsis = tmdbSearchResult.Overview
            };
           
        }

      
    }
}
