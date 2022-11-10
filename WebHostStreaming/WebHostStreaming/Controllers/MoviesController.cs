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
using MoviesAPI.Services.Movies.Dtos;
using MoviesAPI.Services.Torrent.Dtos;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        ISearchersProvider searchersProvider;

        IMovieStreamProvider movieStreamProvider;
        IWatchedMoviesProvider watchedMoviesProvider;
        IBookmarkedMoviesProvider bookmarkedMoviesProvider;
        public MoviesController(
            ISearchersProvider searchersProvider,
            IMovieStreamProvider movieStreamProvider,
            IWatchedMoviesProvider watchedMoviesProvider,
            IBookmarkedMoviesProvider bookmarkedMoviesProvider)
        {
            this.searchersProvider = searchersProvider;

            this.movieStreamProvider = movieStreamProvider;
            this.watchedMoviesProvider = watchedMoviesProvider;
            this.bookmarkedMoviesProvider = bookmarkedMoviesProvider;
        }
        [HttpGet("genres")]
        public async Task<IEnumerable<Genre>> GetMoviesGenres()
        {
            return await searchersProvider.MovieSearcher.GetGenresAsync();
        }

        [HttpGet("moviesoftoday")]
        public async Task<IEnumerable<LiteMovieDto>> GetMoviesOfToday()
        {
            var movies = await searchersProvider.MovieSearcher.GetMoviesOfTodayAsync();

            return movies.Where(m => !string.IsNullOrEmpty(m.LogoImageUrl)).Take(5);
        }

        [HttpGet("netflix")]
        public async Task<IEnumerable<LiteMovieDto>> GetPopularNetflixMovies()
        {
            return await searchersProvider.MovieSearcher.GetTopNetflixMoviesAsync();
        }

        [HttpGet("disneyplus")]
        public async Task<IEnumerable<LiteMovieDto>> GetPopularDisneyPlusMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularDisneyPlusMoviesAsync();
        }

        [HttpGet("amazonprime")]
        public async Task<IEnumerable<LiteMovieDto>> GetPopularAmazonPrimeMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularAmazonPrimeMoviesAsync();
        }

        [HttpGet("popular")]
        public async Task<IEnumerable<LiteMovieDto>> GetPopularMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularMoviesAsync();
        }

        [HttpGet("recommandations")]
        public async Task<IEnumerable<LiteMovieDto>> GetRecommandations()
        {
            var movies = watchedMoviesProvider.GetWatchedMovies();
            if (movies == null || !movies.Any())
                return null;

            return await searchersProvider.MovieSearcher.GetRecommandationsAsync(movies.Last().Id);
        }

        [HttpGet("genre/{genreId}")]
        public async Task<IEnumerable<LiteMovieDto>> GetPopularMoviesByGenre(int genreId)
        {
            return await searchersProvider.MovieSearcher.GetPopularMoviesByGenreAsync(genreId);
        }

        [HttpGet("genre/{genreId}/{page}")]
        public async Task<IEnumerable<LiteMovieDto>> GetMoviesByGenre(int genreId, int page)
        {
            return await searchersProvider.MovieSearcher.GetMoviesByGenreAsync(genreId, page);
        }

        [HttpGet("search/{text}")]
        public async Task<IEnumerable<LiteMovieDto>> SearchMovies(string text)
        {
            return await searchersProvider.MovieSearcher.SearchMoviesAsync(text);
        }

        [HttpGet("details/{id}")]
        public async Task<MovieDto> GetMovieDetails(string id)
        {
            return await searchersProvider.MovieSearcher.GetMovieDetailsAsync(id);
        }

        [HttpGet("topnetflix")]
        public async Task<IEnumerable<LiteMovieDto>> GetTopNetflixMovies()
        {
            return await searchersProvider.MovieSearcher.GetTopNetflixMoviesAsync();
        }

        [HttpGet("vf")]
        public  async Task<IEnumerable<MovieTorrent>> SearchVFTorrents([FromQuery(Name = "movieId")] string movieId, [FromQuery(Name = "originalTitle")] string originalTitle, [FromQuery(Name = "year")] int year)
        {
            var frenchTitle = await searchersProvider.MovieSearcher.GetFrenchTitleAsync(movieId);

            return await searchersProvider.TorrentSearchManager.SearchVfTorrentsAsync(string.IsNullOrEmpty(frenchTitle) ? originalTitle : frenchTitle, year);
        }

        [HttpGet("vo")]
        public async Task<IEnumerable<MovieTorrent>> SearchVOTorrents([FromQuery(Name = "title")] string title, [FromQuery(Name = "year")] int year)
        {
            return await searchersProvider.TorrentSearchManager.SearchVoTorrentsAsync(title, year); ;
        }

        [HttpGet("stream")]
        public IActionResult GetStream([FromQuery(Name = "url")] string url)
        {
            var rangeHeaderValue = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();
            var userAgent = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "User-Agent").Value.FirstOrDefault();

            int offset = 0;
            if (!string.IsNullOrEmpty(rangeHeaderValue))
                int.TryParse(rangeHeaderValue.Replace("bytes=", string.Empty).Split("-")[0], out offset);

            try
            {
                var acceptedFormat = userAgent.StartsWith("VLC") ? "*" : ".mp4";
                var streamDto = movieStreamProvider.GetStream(url, offset, acceptedFormat);
                
                if (streamDto != null)
                    return File(streamDto.Stream, streamDto.ContentType, true); 
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }

        [HttpGet("streamdownloadstate")]
        public IActionResult GetStreamDownloadState([FromQuery(Name = "torrentUrl")] string torrentUrl)
        {
            var state = movieStreamProvider.GetStreamDownloadingState(torrentUrl);

            if (state == null)
                return BadRequest();
            else
                return Ok(state);
        }

        [HttpGet("watchedmovies")]
        public IEnumerable<LiteMovieDto> GetWatchedMovies()
        {
            var movies = watchedMoviesProvider.GetWatchedMovies();
            return movies?.Reverse();
        }

        [HttpPut("watchedmovies")]
        public async Task<IActionResult> SaveWatchedMovie([FromBody] LiteMovieDto movie)
        {
            try
            {
                watchedMoviesProvider.SaveWatchedMovie(movie);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks")]
        public IEnumerable<LiteMovieDto> GetBookmarkedMovies()
        {
            var movies = bookmarkedMoviesProvider.GetBookmarkedMovies();
            return movies?.Reverse();
        }

        [HttpPut("bookmarks")]
        public async Task<IActionResult> BookmarkMovie([FromBody] LiteMovieDto movieToBookmark)
        {
            try
            {
                bookmarkedMoviesProvider.SaveMovieBookmark(movieToBookmark);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("bookmarks")]
        public IActionResult DeleteBookmarkMovie([FromBody] LiteMovieDto movieBookmarkToDelete)
        {
            try
            {
                bookmarkedMoviesProvider.DeleteMovieBookmark(movieBookmarkToDelete.Id);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks/exists")]
        public IActionResult MovieBookmarkExists([FromQuery] string movieId)
        {
            if (!string.IsNullOrEmpty(movieId))
            {
                return Ok(bookmarkedMoviesProvider.MovieBookmarkExists(movieId));
            }
            else
                return BadRequest();
        }
    }
}
