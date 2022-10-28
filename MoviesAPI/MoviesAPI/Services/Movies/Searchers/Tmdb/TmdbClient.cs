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
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildSearchUrl(movieName));

            return ToLiteMovieDtos(results, 15);
        }

        public async Task<IEnumerable<LiteMovieDto>> GetMoviesOfTodayAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildMoviesOfTodayUrl());

            return ToLiteMovieDtos(results);
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

        public async Task<IEnumerable<LiteMovieDto>> GetSimilarMovies(string tmdbMovieId)
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

        private IEnumerable<LiteMovieDto> ToLiteMovieDtos(TmdbSearchResults tmdbSearchResults, int voteCountMin = 0)
        {
            return tmdbSearchResults?.Results?.Where(r => r.VoteCount > voteCountMin).Select(r => ToLiteMovieDto(r));
        }

        public async Task<MovieDto> GetMovieDetails(string movieId)
        {
            var tmdbSearchResult = await HttpRequester.GetAsync<TmdbSearchResult>(tmdbUrlBuilder.BuildMovieDetailsUrl(movieId));
            var tmdbCredits = await GetMovieCredits(movieId);
           
            return new MovieDto()
            {
                Title = tmdbSearchResult.Title,
                Year = tmdbSearchResult.ReleaseDate?.Split('-')[0],
                CoverImageUrl = tmdbUrlBuilder.BuildCoverImageUrl(tmdbSearchResult.PosterPath),
                BackgroundImageUrl = tmdbUrlBuilder.BuildBackgroundImageUrl(tmdbSearchResult.BackdropPath),
                Rating = tmdbSearchResult.VoteAverage,
                MovieId = tmdbSearchResult.Id,
                Synopsis = tmdbSearchResult.Overview,
                Genre = tmdbSearchResult.Genres.Select(g => g.Name).Aggregate((a, b) => $"{a}, {b}"),
                Duration = tmdbSearchResult.Runtime + " min.",
                YoutubeTrailerUrl = GetYoutubeTrailerUrlVideo(tmdbSearchResult.Videos.Results),
                Cast = tmdbCredits.Cast.Take(4).Select(c => c.Name).Aggregate((a, b) => $"{a}, {b}"),
                Director = tmdbCredits.Crew.FirstOrDefault(c => c.Job.Equals("director", StringComparison.OrdinalIgnoreCase))?.Name
            };
        }

        public async Task<IEnumerable<LiteMovieDto>> GetMoviesByGenre(int genreId, int page)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildMoviesByGenreUrl(genreId, page));

            return ToLiteMovieDtos(results);
        }


        public async Task<IEnumerable<LiteMovieDto>> GetPopularMoviesByGenre(int genreId)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularMoviesByGenreUrl(genreId));

            return ToLiteMovieDtos(results);
        }

        public async Task<IEnumerable<Genre>> GetGenres()
        {
            var result = await HttpRequester.GetAsync<TmdbGenres>(tmdbUrlBuilder.BuilGenresListUrl());

            return result?.Genres;
        }

        public async Task<IEnumerable<LiteMovieDto>> GetTopNetflixMovies()
        {
            var moviesName = await NetflixRequester.GetTopNetflixMoviesNameAsync();

            var tasks = new List<Task<LiteMovieDto>>();

            foreach (var movieName in moviesName)
            {
                tasks.Add(SearchMoviesAsync(movieName).ContinueWith(t => t.Result.FirstOrDefault(t => t.Title.Equals(movieName, StringComparison.OrdinalIgnoreCase))));
            }

            return await Task.WhenAll(tasks).ContinueWith(t => t.Result.Where(m => m != null));
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

            return trailerVideo != null ? $"https://www.youtube.com/watch?v={trailerVideo?.Key}" : null;
        }
        private LiteMovieDto ToLiteMovieDto(TmdbSearchResult tmdbSearchResult)
        {
            return new LiteMovieDto()
            {
                Title = tmdbSearchResult.Title,
                Year = tmdbSearchResult.ReleaseDate?.Split('-')[0],
                CoverImageUrl = tmdbUrlBuilder.BuildCoverImageUrl(tmdbSearchResult.PosterPath),
                BackgroundImageUrl = tmdbUrlBuilder.BuildBackgroundImageUrl(tmdbSearchResult.BackdropPath),
                Rating = tmdbSearchResult.VoteAverage,
                MovieId = tmdbSearchResult.Id,
                Synopsis = tmdbSearchResult.Overview
            };
        }

      
    }
}
