﻿using MoviesAPI.Services.Tmdb.Dtos;
using MoviesAPI.Services.Tmdb;
using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Content
{
    internal class TmdbSeriesClient : TmdbClient, ISeriesSearcher
    {
        public TmdbSeriesClient(string apiKey) : base(new TmdbSeriesUrlBuilder(apiKey))
        {

        }

        public async Task<ContentDto> GetSerieDetailsAsync(string serieId)
        {
            return await GetContentDetailsAsync(serieId);
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

        public async Task<IEnumerable<LiteContentDto>> GetRecommandationsAsync(string serieId)
        {
            return await GetRecommandedContentAsync(serieId);
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
    }
}