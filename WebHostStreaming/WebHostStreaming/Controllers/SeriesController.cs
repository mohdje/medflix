using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;
using System.IO;
using WebHostStreaming.Models;
using System.Net;
using MoviesAPI.Services.Content.Dtos;
using MoviesAPI.Services.Tmdb.Dtos;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        IWatchedMediaProvider watchedMediaProvider;
        IBookmarkedMediaProvider bookmarkedMediaProvider;
        public SeriesController(
            ISearchersProvider searchersProvider,
            IWatchedMediaProvider watchedMediaProvider,
            IBookmarkedMediaProvider bookmarkedMediaProvider)
        {
            this.searchersProvider = searchersProvider;
            this.watchedMediaProvider = watchedMediaProvider;
            this.bookmarkedMediaProvider = bookmarkedMediaProvider;
        }
        [HttpGet("genres")]
        public async Task<IEnumerable<Genre>> GetSeriesGenres()
        {
            return await searchersProvider.SeriesSearcher.GetSerieGenresAsync();
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

        [HttpGet("popular")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularSeries()
        {
            return await searchersProvider.SeriesSearcher.GetPopularSeriesAsync();
        }

        [HttpGet("similar/{id}")]
        public async Task<IEnumerable<LiteContentDto>> GetSimilarSeries(string id)
        {
            return await searchersProvider.SeriesSearcher.GetRecommandationsAsync(id);
        }

        [HttpGet("recommandations")]
        public async Task<IEnumerable<LiteContentDto>> GetRecommandations([FromQuery]string id)
        {
            var serieId = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                var series = await watchedMediaProvider.GetWatchedSeriesAsync();
                if (series == null || !series.Any())
                    return null;
                else
                    serieId = series.Last().Id;
            }
            else
                serieId = id;

            return await searchersProvider.SeriesSearcher.GetRecommandationsAsync(serieId);
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
        public async Task<IEnumerable<WatchedMediaDto>> GetWatchedSeries()
        {
            var series = await watchedMediaProvider.GetWatchedSeriesAsync();
            return series?.Reverse();
        }

        [HttpGet("watchedmedia/{id}")]
        public async Task<WatchedMediaDto> GetWatchedSerie(int id)
        {
            var series = await watchedMediaProvider.GetWatchedSeriesAsync();
            return series?.SingleOrDefault(m => m.Id == id.ToString());
        }

        [HttpPut("watchedmedia")]
        public async Task<IActionResult> SaveWatchedSerie([FromBody] WatchedMediaDto watchedSerie)
        {
            try
            {
                await watchedMediaProvider.SaveWatchedSerieAsync(watchedSerie);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks")]
        public async Task<IEnumerable<LiteContentDto>> GetBookmarkedSeries()
        {
            var series = await bookmarkedMediaProvider.GetBookmarkedSeriesAsync();
            return series?.Reverse();
        }



        [HttpPut("bookmarks")]
        public async Task<IActionResult> BookmarkSerie([FromBody] LiteContentDto serieToBookmark)
        {
            try
            {
                await bookmarkedMediaProvider.SaveSerieBookmarkAsync(serieToBookmark);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("bookmarks")]
        public async Task<IActionResult> DeleteBookmarkserie([FromBody] LiteContentDto serieBookmarkToDelete)
        {
            try
            {
                await bookmarkedMediaProvider.DeleteSerieBookmarkAsync(serieBookmarkToDelete.Id);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks/exists")]
        public async Task<IActionResult> BookmarkExists([FromQuery(Name = "id")] string serieId)
        {
            if (!string.IsNullOrEmpty(serieId))
            {
                return Ok(await bookmarkedMediaProvider.SerieBookmarkExistsAsync(serieId));
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
