using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;
using WebHostStreaming.Models;
using System.Net;
using MoviesAPI.Services.Content.Dtos;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        IWatchedSeriesProvider watchedSeriesProvider;
        IBookmarkedSeriesProvider bookmarkedSeriesProvider;
        IRecommandationsProvider recommandationsProvider;
        public SeriesController(
            ISearchersProvider searchersProvider,
            IWatchedSeriesProvider watchedSeriesProvider,
            IBookmarkedSeriesProvider bookmarkedSeriesProvider,
             IRecommandationsProvider recommandationsProvider)
        {
            this.searchersProvider = searchersProvider;
            this.watchedSeriesProvider = watchedSeriesProvider;
            this.bookmarkedSeriesProvider = bookmarkedSeriesProvider;
            this.recommandationsProvider = recommandationsProvider;
        }

        [HttpGet("genres")]
        public async Task<IEnumerable<Genre>> GetSeriesGenres()
        {
            return await searchersProvider.SeriesSearcher.GetSerieGenresAsync();
        }

        [HttpGet("platforms")]
        public async Task<IEnumerable<Platform>> GetSeriesPlatforms()
        {
            return await searchersProvider.SeriesSearcher.GetSeriePlatformsAsync();
        }

        [HttpGet("mediasoftoday")]
        public async Task<IEnumerable<LiteContentDto>> GetSeriesOfToday()
        {
            var series = await searchersProvider.SeriesSearcher.GetSeriesOfTodayAsync();

            return series.Where(m => !string.IsNullOrEmpty(m.LogoImageUrl)).Take(5);
        }

        [HttpGet("netflix")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularNetflixSeries()
        {
            return await searchersProvider.SeriesSearcher.GetPopularNetflixSeriesAsync();
        }

        [HttpGet("disneyplus")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularDisneyPlusSeries()
        {
            return await searchersProvider.SeriesSearcher.GetPopularDisneyPlusSeriesAsync();
        }

        [HttpGet("amazonprime")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularAmazonPrimeSeries()
        {
            return await searchersProvider.SeriesSearcher.GetPopularAmazonPrimeSeriesAsync();
        }

        [HttpGet("appletv")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularAppleTvSeries()
        {
            return await searchersProvider.SeriesSearcher.GetPopularAppleTvSeriesAsync();
        }

        [HttpGet("popular")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularSeries()
        {
            return await searchersProvider.SeriesSearcher.GetPopularSeriesAsync();
        }

        [HttpGet("similar/{id}")]
        public async Task<IEnumerable<LiteContentDto>> GetSimilarSeries(string id)
        {
            return await searchersProvider.SeriesSearcher.GetSimilarSeriesAsync(id);
        }

        [HttpGet("recommandations")]
        public async Task<IEnumerable<LiteContentDto>> GetRecommandations()
        {
            return await recommandationsProvider.GetSeriesRecommandationsAsync();
        }

        [HttpGet("genre/{genreId}")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularSeriesByGenre(int genreId)
        {
            return await searchersProvider.SeriesSearcher.GetPopularSeriesByGenreAsync(genreId);
        }

        [HttpGet("genre/{genreId}/{page}")]
        public async Task<IEnumerable<LiteContentDto>> GetSeriesByGenre(int genreId, int page)
        {
            return await searchersProvider.SeriesSearcher.GetSeriesByGenreAsync(genreId, page);
        }

        [HttpGet("platform/{platformId}/{page}")]
        public async Task<IEnumerable<LiteContentDto>> GetSeriesByPlatform(int platformId, int page)
        {
            return await searchersProvider.SeriesSearcher.GetSeriesByPlatformAsync(platformId, page);
        }

        [HttpGet("search")]
        public async Task<IEnumerable<LiteContentDto>> SearchSeries([FromQuery(Name = "t")] string text)
        {
            return await searchersProvider.SeriesSearcher.SearchSeriesAsync(text);
        }

        [HttpGet("details/{id}")]
        public async Task<ContentDto> GetSerieDetails(string id)
        {
            return await searchersProvider.SeriesSearcher.GetSerieDetailsAsync(id);
        }

        [HttpGet("watchedmedia")]
        public IEnumerable<WatchedMediaDto> GetWatchedSeries()
        {
            return watchedSeriesProvider.GetWatchedSeries();
        }

        [HttpGet("watchedmedia/{id}")]
        public WatchedMediaDto GetWatchedEpisode(int id, int seasonNumber, int episodeNumber)
        {
            return watchedSeriesProvider.GetWatchedEpisode(id, seasonNumber, episodeNumber);
        }

        [HttpGet("watchedmedia/{id}/{seasonNumber}")]
        public IEnumerable<WatchedMediaDto> GetWatchedEpisodesBySeason(int id, int seasonNumber)
        {
            return watchedSeriesProvider.GetWatchedEpisodes(id, seasonNumber);
        }

        [HttpPut("watchedmedia")]
        public IActionResult SaveWatchedEpisode([FromBody] WatchedMediaDto watchedEpisode)
        {
            try
            {
                watchedSeriesProvider.SaveWatchedEpisode(watchedEpisode);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks")]
        public IEnumerable<LiteContentDto> GetBookmarkedSeries()
        {
            return bookmarkedSeriesProvider.GetBookmarkedSeries();
        }

        [HttpPut("bookmarks")]
        public IActionResult BookmarkSerie([FromBody] LiteContentDto serieToBookmark)
        {
            try
            {
                bookmarkedSeriesProvider.AddSerieBookmark(serieToBookmark);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("bookmarks")]
        public IActionResult DeleteBookmarkserie([FromQuery(Name = "id")] string serieId)
        {
            try
            {
                bookmarkedSeriesProvider.DeleteSerieBookmark(serieId);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks/exists")]
        public IActionResult BookmarkExists([FromQuery(Name = "id")] string serieId)
        {
            if (!string.IsNullOrEmpty(serieId))
            {
                return Ok(bookmarkedSeriesProvider.SerieBookmarkExists(serieId));
            }
            else
                return BadRequest();
        }

        [HttpGet("episodes/{serieId}/{seasonNumber}")]
        public async Task<IEnumerable<EpisodeDto>> GetEpisodes(string serieId, int seasonNumber)
        {
            return await searchersProvider.SeriesSearcher.GetEpisodes(serieId, seasonNumber);
        }
    }
}
