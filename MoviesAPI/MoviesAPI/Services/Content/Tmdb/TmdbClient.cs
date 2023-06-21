using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Content.Dtos;
using MoviesAPI.Services.Tmdb.Dtos;

namespace MoviesAPI.Services.Tmdb
{
    internal abstract class TmdbClient
    {
        protected readonly TmdbUrlBuilder tmdbUrlBuilder;
        public TmdbClient(TmdbUrlBuilder tmdbUrlBuilder)
        {
            this.tmdbUrlBuilder = tmdbUrlBuilder;
        }

        public async Task<IEnumerable<LiteContentDto>> SearchContentAsync(string contentName)
        {
            var response = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildSearchUrl(contentName));

            return ToLiteContentDtos(response);
        }

        protected async Task<IEnumerable<LiteContentDto>> GetTrendingOfTodayAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildTrendingOfTodayUrl());

            var liteContentDtos = ToLiteContentDtos(results).ToList();//ToList() is needed to update LogoImageUrl on each element

            var tasks = new List<Task>();
            var mediaLogos = new List<Tuple<string, string>>();

            foreach (var liteContentDto in liteContentDtos)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var logoImgUrl = await GetLogoImageUrlAsync(liteContentDto.Id);
                    mediaLogos.Add(new Tuple<string, string>(liteContentDto.Id, logoImgUrl));
                }));
                 
            }

            await Task.WhenAll(tasks);

            foreach (var contentDto in liteContentDtos)
            {
                contentDto.LogoImageUrl = mediaLogos.SingleOrDefault(m => m.Item1 == contentDto.Id)?.Item2;
            }

            return liteContentDtos.OrderBy(m => m.Rating);
        }

        protected async Task<IEnumerable<LiteContentDto>> GetPopularContentAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularContentUrl());

            return ToLiteContentDtos(results);
        }

        protected async Task<IEnumerable<LiteContentDto>> GetRecommandedContentAsync(string[] genreIds, string minDate, string maxDate, string[] excludedTmdbContentIds)
        {
            var recommandations = new List<LiteContentDto>();
            var recommandationsCount = 15;
            var page = 0;

            while (recommandations.Count < recommandationsCount)
            {
                page++;
                var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildRecommandationsUrl(genreIds, minDate, maxDate, page));

                if (results == null || !results.Results.Any())
                    break;
                else
                    recommandations.AddRange(ToLiteContentDtos(results).Where(content => !excludedTmdbContentIds.Contains(content.Id)));
            }

            return recommandations.Take(recommandationsCount);
        }

        protected async Task<IEnumerable<LiteContentDto>> GetSimilarContentAsync(string tmdbContentId)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildSimilarContentUrl(tmdbContentId));

            return ToLiteContentDtos(results);
        }

        protected async Task<string> GetFrenchTitleAsync(string tmdbContentId)
        {
            var tmdbTranslations = await HttpRequester.GetAsync<TmdbTranslations>(tmdbUrlBuilder.BuildTranslationUrl(tmdbContentId));

            var frenchTranslation = tmdbTranslations?.Translations?.FirstOrDefault(t => t.CountryCode == "FR");

            return frenchTranslation?.Data.Title ?? frenchTranslation?.Data.Name;
        }

        protected async Task<string> GetIdAsync(string originalTitle, int year)
        {
            var tmdbResults = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildSearchUrl(originalTitle));

            return tmdbResults?.Results?.FirstOrDefault(r => r.Title.Equals(originalTitle, StringComparison.OrdinalIgnoreCase) && r.Year == year)?.Id;
        }

        protected async Task<string> GetLogoImageUrlAsync(string tmdbContentId)
        {
            var tmdbImages = await HttpRequester.GetAsync<TmdbImages>(tmdbUrlBuilder.BuildGetImagesUrl(tmdbContentId));

            var filePath = tmdbImages?.Logos.Where(l => string.IsNullOrEmpty(l.CountryCode) || l.CountryCode == "en").OrderBy(l => l.VoteAverage).FirstOrDefault()?.FilePath;

            return String.IsNullOrEmpty(filePath) ? null : tmdbUrlBuilder.BuildLogoImageUrl(filePath);
        }


        protected async Task<ContentDto> GetContentDetailsAsync(string tmdbContentId)
        {
            var tmdbSearchResult = await HttpRequester.GetAsync<TmdbSearchResult>(tmdbUrlBuilder.BuildContentDetailsUrl(tmdbContentId));
            var logoImgUrl = await GetLogoImageUrlAsync(tmdbContentId);
            var tmdbCredits = await GetCredits(tmdbContentId);
           
            return new ContentDto()
            {
                Title = !string.IsNullOrEmpty(tmdbSearchResult.Title) ? tmdbSearchResult.Title : tmdbSearchResult.Name,
                Year = tmdbSearchResult.Year,
                CoverImageUrl = tmdbUrlBuilder.BuildCoverImageUrl(tmdbSearchResult.PosterPath),
                BackgroundImageUrl = tmdbUrlBuilder.BuildBackgroundImageUrl(tmdbSearchResult.BackdropPath),
                LogoImageUrl = logoImgUrl,
                Rating = Math.Round(tmdbSearchResult.VoteAverage, 1),
                Id = tmdbSearchResult.Id,
                Synopsis = tmdbSearchResult.Overview,
                Genres = tmdbSearchResult.Genres,
                Duration = tmdbSearchResult.Runtime.GetValueOrDefault(0),
                YoutubeTrailerUrl = GetYoutubeTrailerUrlVideo(tmdbSearchResult.Videos.Results),
                Cast = tmdbCredits?.Cast?.Take(4).Select(c => c.Name).Aggregate((a, b) => $"{a}, {b}"),
                Director = tmdbCredits?.DirectorName,
                ImdbId = tmdbSearchResult.ImdbId,
                SeasonsCount = tmdbSearchResult.SeasonsCount
            };
        }

        protected async Task<IEnumerable<LiteContentDto>> GetContentByGenreAsync(int genreId, int page)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildContentByGenreUrl(genreId, page));

            return ToLiteContentDtos(results);
        }


        protected async Task<IEnumerable<LiteContentDto>> GetPopularContentByGenreAsync(int genreId)
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularContentByGenreUrl(genreId));

            return ToLiteContentDtos(results);
        }

        protected async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            var result = await HttpRequester.GetAsync<TmdbGenres>(tmdbUrlBuilder.BuilGenresListUrl());

            return result?.Genres;
        }

        protected async Task<IEnumerable<LiteContentDto>> GetPopularNetflixContentAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularContentByPlatformUrl(8));
            results.Results = results.Results.OrderByDescending(r => r.VoteCount).ToArray();

            return ToLiteContentDtos(results);
        }

        protected async Task<IEnumerable<LiteContentDto>> GetPopularDisneyPlusContentAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularContentByPlatformUrl(337));
            results.Results = results.Results.OrderByDescending(r => r.VoteCount).ToArray();

            return ToLiteContentDtos(results);
        }

        protected async Task<IEnumerable<LiteContentDto>> GetPopularAmazonPrimeContentAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularContentByPlatformUrl(9));
            results.Results = results.Results.OrderByDescending(r => r.VoteCount).ToArray();

            return ToLiteContentDtos(results);
        }

        protected async Task<IEnumerable<LiteContentDto>> GetPopularAppleTvContentAsync()
        {
            var results = await HttpRequester.GetAsync<TmdbSearchResults>(tmdbUrlBuilder.BuildPopularContentByPlatformUrl(2));
            results.Results = results.Results.OrderByDescending(r => r.VoteCount).ToArray();

            return ToLiteContentDtos(results);
        }

        protected async Task<TmdbSerieEpisodes> GetEpisodesBySeason(string seriesId, int seasonNumber)
        {
            return await HttpRequester.GetAsync<TmdbSerieEpisodes>(tmdbUrlBuilder.BuildEpisodesBySeasonUrl(seriesId, seasonNumber));
        }

        private async Task<TmdbCredits> GetCredits(string tmdbContentId)
        {
            return await HttpRequester.GetAsync<TmdbCredits>(tmdbUrlBuilder.BuildGetCreditsUrl(tmdbContentId));
        }

        private string GetYoutubeTrailerUrlVideo(TmdbVideo[] tmdbVideos)
        {
            var youtubeVideos = tmdbVideos.Where(v => v.Site.Equals("youtube", StringComparison.OrdinalIgnoreCase));

            var trailerVideo = youtubeVideos?.FirstOrDefault(v => v.Type.Equals("Trailer", StringComparison.OrdinalIgnoreCase))
                                ?? youtubeVideos?.FirstOrDefault();

            return trailerVideo != null ? $"https://www.youtube.com/embed/{trailerVideo?.Key}?rel=0&wmode=transparent&border=0&autoplay=1&iv_load_policy=3" : null;
        }

        private IEnumerable<LiteContentDto> ToLiteContentDtos(TmdbSearchResults tmdbSearchResults)
        {
            return tmdbSearchResults?.Results?.Where(r => r.VoteCount > 0).Select(r => ToLiteContentDto(r));
        }

        private LiteContentDto ToLiteContentDto(TmdbSearchResult tmdbSearchResult)
        {
            return new LiteContentDto()
            {
                Title = !string.IsNullOrEmpty(tmdbSearchResult.Title) ? tmdbSearchResult.Title : tmdbSearchResult.Name,
                Year = tmdbSearchResult.Year,
                CoverImageUrl = tmdbUrlBuilder.BuildCoverImageUrl(tmdbSearchResult.PosterPath),
                BackgroundImageUrl = tmdbUrlBuilder.BuildBackgroundImageUrl(tmdbSearchResult.BackdropPath),
                Rating = Math.Round(tmdbSearchResult.VoteAverage, 1),
                Id = tmdbSearchResult.Id,
                Synopsis = tmdbSearchResult.Overview
            };          
        }

        

    }
}
