using MoviesAPI.Services.Tmdb.Dtos;
using MoviesAPI.Services.Tmdb;
using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Helpers;

namespace MoviesAPI.Services.Content
{
    internal class TmdbSeriesClient : TmdbClient, ISeriesSearcher
    {
        public TmdbSeriesClient(string apiKey) : base(new TmdbSeriesUrlBuilder(apiKey))
        {

        }

        public async Task<ContentDto> GetSerieDetailsAsync(string serieId)
        {
            var details = await GetContentDetailsAsync(serieId);
            if (string.IsNullOrEmpty(details.ImdbId))
                details.ImdbId = await GetImdbId(serieId);

            return details;
        }

        public async Task<IEnumerable<LiteContentDto>> GetSeriesByGenreAsync(int genreId, int page)
        {
            return await GetContentByGenreAsync(genreId, page);
        }

        public async Task<IEnumerable<LiteContentDto>> GetSeriesOfTodayAsync()
        {
            return await GetTrendingOfTodayAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularAmazonPrimeSeriesAsync()
        {
            return await GetPopularAmazonPrimeContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularDisneyPlusSeriesAsync()
        {
            return await GetPopularDisneyPlusContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularSeriesAsync()
        {
            return await GetPopularContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularSeriesByGenreAsync(int genreId)
        {
            return await GetPopularContentByGenreAsync(genreId);
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularNetflixSeriesAsync()
        {
            return await GetPopularNetflixContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularAppleTvSeriesAsync()
        {
            return await GetPopularAppleTvContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetRecommandationsAsync(string[] genreIds, string minDate, string maxDate, string[] excludedTmdbContentIds)
        {
            return await GetRecommandedContentAsync(genreIds, minDate, maxDate, excludedTmdbContentIds);
        }

        public async Task<IEnumerable<LiteContentDto>> GetSimilarSeriesAsync(string serieId)
        {
            return await GetSimilarContentAsync(serieId);
        }

        public async Task<IEnumerable<LiteContentDto>> SearchSeriesAsync(string serieName)
        {
            return await SearchContentAsync(serieName);
        }

        public async Task<IEnumerable<EpisodeDto>> GetEpisodes(string serieId, int seasonNumber)
        {
            var seasonEpisodes = await GetEpisodesBySeason(serieId, seasonNumber);

            if (seasonEpisodes == null)
                return new EpisodeDto[0];

            var episodeDtos = new List<EpisodeDto>();
            foreach (var episode in seasonEpisodes.Episodes)
            {
                episodeDtos.Add(new EpisodeDto()
                {
                    EpisodeNumber = episode.EpisodeNumber,
                    Name = episode.Name,
                    Overview = episode.Overview,
                    RunTime = episode.RunTime,
                    ImagePath = tmdbUrlBuilder.BuildLogoImageUrl(episode.ImagePath)
                });
            }

            return episodeDtos;
        }

        public async Task<string> GetSerieFrenchTitleAsync(string seriesId)
        {
            return await GetFrenchTitleAsync(seriesId);
        }
        public async Task<IEnumerable<Genre>> GetSerieGenresAsync()
        {
            var genres = await GetGenresAsync();

            var genreIdsToRemove = new int[] { 10762, 10763, 10764, 10766, 10768, 37 };

            return genres != null ? genres.Where(g => !genreIdsToRemove.Contains(g.Id)) : new Genre[0];
        }

        private async Task<string> GetImdbId(string tmdbContentId)
        {
            var result = await HttpRequester.GetAsync<TmdbExternalIdDto>(tmdbUrlBuilder.BuildSerieGetExternalIds(tmdbContentId));
            return result?.ImdbId;
        }

        public async Task<IEnumerable<LiteContentDto>> GetSeriesByPlatformAsync(int platformId, int page)
        {
            return await GetContentByPlatformAsync(platformId, page);
        }

        public async Task<IEnumerable<Platform>> GetSeriePlatformsAsync()
        {
            var platforms = await GetPlatformsAsync();

            var platformsIdsToKeep = new int[] { 2, 8, 9, 337 };//keep netflix, amazon, disney plus, appletv

            return platforms.Where(platform => platformsIdsToKeep.Contains(platform.Id));
        }
    }
}
