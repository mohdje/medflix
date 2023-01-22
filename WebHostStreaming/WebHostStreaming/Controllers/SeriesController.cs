using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Services;
using WebHostStreaming.Providers;
using System.IO;
using WebHostStreaming.Models;
using WebHostStreaming.Helpers;
using System.Net;
using MoviesAPI.Services.Torrent.Dtos;
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
            var movies = await searchersProvider.SeriesSearcher.GetSeriesOfTodayAsync();

            return movies.Where(m => !string.IsNullOrEmpty(m.LogoImageUrl)).Take(5);
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
            var movieId = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                var movies = await watchedMediaProvider.GetWatchedSeriesAsync();
                if (movies == null || !movies.Any())
                    return null;
                else
                    movieId = movies.Last().Id;
            }
            else
                movieId = id;

            return await searchersProvider.SeriesSearcher.GetRecommandationsAsync(movieId);
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
        public async Task<ContentDto> GetMovieDetails(string id)
        {
            return await searchersProvider.SeriesSearcher.GetSerieDetailsAsync(id);
        }

        [HttpGet("watchedmedia")]
        public async Task<IEnumerable<WatchedMediaDto>> GetWatchedSeries()
        {
            var movies = await watchedMediaProvider.GetWatchedSeriesAsync();
            return movies?.Reverse();
        }

        [HttpGet("watchedmedia/{id}")]
        public async Task<WatchedMediaDto> GetWatchedSerie(int id)
        {
            var movies = await watchedMediaProvider.GetWatchedSeriesAsync();
            return movies?.SingleOrDefault(m => m.Id == id.ToString());
        }

        [HttpPut("watchedmedia")]
        public async Task<IActionResult> SaveWatchedMovie([FromBody] WatchedMediaDto watchedSerie)
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
            var movies = await bookmarkedMediaProvider.GetBookmarkedSeriesAsync();
            return movies?.Reverse();
        }



        [HttpPut("bookmarks")]
        public async Task<IActionResult> BookmarkMovie([FromBody] LiteContentDto serieToBookmark)
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
        public async Task<IActionResult> DeleteBookmarkMovie([FromBody] LiteContentDto serieBookmarkToDelete)
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
        public async Task<IActionResult> MovieBookmarkExists([FromQuery] string serieId)
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
            try
            {
                return await searchersProvider.SeriesSearcher.GetEpisodes(serieId, seasonNumber);
            }
            catch (Exception)
            {

                return new EpisodeDto[0];
            }
            
        }
    }
}
