using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Services.Content.Dtos;
using MoviesAPI.Services.Tmdb.Dtos;

namespace MoviesAPI.Services.Content
{
    public interface ISeriesSearcher
    {
        Task<string> GetSerieFrenchTitleAsync(string seriesId);
        Task<IEnumerable<LiteContentDto>> SearchSeriesAsync(string seriesName);
        Task<IEnumerable<LiteContentDto>> GetSeriesOfTodayAsync();
        Task<IEnumerable<LiteContentDto>> GetPopularSeriesAsync();
        Task<IEnumerable<LiteContentDto>> GetRecommandationsAsync(string[] genreIds, string minDate, string maxDate, string[] excludedTmdbContentIds);
        Task<IEnumerable<LiteContentDto>> GetSimilarSeriesAsync(string seriesId);
        Task<ContentDto> GetSerieDetailsAsync(string seriesId);
        Task<IEnumerable<LiteContentDto>> GetSeriesByGenreAsync(int genreId, int page);
        Task<IEnumerable<LiteContentDto>> GetSeriesByPlatformAsync(int platformId, int page);
        Task<IEnumerable<LiteContentDto>> GetPopularSeriesByGenreAsync(int genreId);
        Task<IEnumerable<LiteContentDto>> GetPopularNetflixSeriesAsync();
        Task<IEnumerable<LiteContentDto>> GetPopularDisneyPlusSeriesAsync();
        Task<IEnumerable<LiteContentDto>> GetPopularAmazonPrimeSeriesAsync();

        Task<IEnumerable<LiteContentDto>> GetPopularAppleTvSeriesAsync();
        Task<IEnumerable<Genre>> GetSerieGenresAsync();
        Task<IEnumerable<Platform>> GetSeriePlatformsAsync();
        Task<IEnumerable<EpisodeDto>> GetEpisodes(string serieId, int seasonNumber);
    }
}
