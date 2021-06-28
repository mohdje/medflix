using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services;
using WebHostStreaming.Providers;
using System.IO;
using WebHostStreaming.Models;
using WebHostStreaming.Helpers;
using System.Net;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        IMovieServiceProvider movieServiceProvider;
        IMovieService movieService;
        IMovieStreamProvider movieStreamProvider;
        IMovieBookmarkProvider movieBookmarkProvider;
        public MoviesController(IMovieServiceProvider movieServiceProvider, IMovieStreamProvider movieStreamProvider, IMovieBookmarkProvider movieBookmarkProvider)
        {
            this.movieServiceProvider = movieServiceProvider;
            this.movieService = movieServiceProvider.GetActiveMovieService();
            this.movieStreamProvider = movieStreamProvider;
            this.movieBookmarkProvider = movieBookmarkProvider;
        }
        [HttpGet("genres")]
        public IEnumerable<string> GetMoviesGenres()
        {
            return movieService.GetMovieGenres();
        }

        [HttpGet("suggested")]
        public async Task<IEnumerable<MovieDto>> GetSuggestedMovies()
        {
            return await movieService.GetSuggestedMoviesAsync(5);
        }

        [HttpGet("genre/{genre}")]
        public async Task<IEnumerable<MovieDto>> GetLastMoviesByGenre(string genre)
        {
            return await movieService.GetLastMoviesByGenreAsync(15, genre);
        }

        [HttpGet("genre/{genre}/{page}")]
        public async Task<IEnumerable<MovieDto>> GetLastMoviesByGenre(string genre, int page)
        {
            return await movieService.GetMoviesByGenreAsync(genre, page);
        }

        [HttpGet("search/{text}")]
        public async Task<IEnumerable<MovieDto>> SearchMovies(string text)
        {
            return await movieService.GetMoviesByNameAsync(text);
        }

        [HttpGet("details/{id}")]
        public async Task<MovieDto> GetMovieDetails(string id)
        {
            return await movieService.GetMovieDetailsAsync(id);
        }

        [HttpGet("stream")]
        public IActionResult GetStream([FromQuery(Name = "url")] string url)
        {
            var rangeHeaderValue = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();

            int offset = 0;
            if(!string.IsNullOrEmpty(rangeHeaderValue))
                int.TryParse(rangeHeaderValue.Replace("bytes=", string.Empty).Split("-")[0], out offset);

            try
            {
                var stream = movieStreamProvider.GetStream(url, offset);
              
                if (stream != null)
                    return File(stream, "video/mp4", true);
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

            if (string.IsNullOrEmpty(state))
                return NoContent();
            else
                return Ok(state);
        }

        [HttpGet("lastseenmovies")]
        public IEnumerable<MovieBookmark> GetLastSeenMovies()
        {
            var lastSeenMovies = movieBookmarkProvider.GetMovieBookmarks(AppFiles.LastSeenMovies);
            return lastSeenMovies?.Reverse();
        }

        [HttpPut("lastseenmovies")]
        public void SaveLastSeenMovie([FromBody] MovieDto movie)
        {
            var movieBookmark = new MovieBookmark()
            {
                Movie = movie,
                ServiceName = movieServiceProvider.GetActiveServiceTypeName()
            };

            movieBookmarkProvider.SaveMovieBookmark(movieBookmark, AppFiles.LastSeenMovies, 7);
        }

        [HttpGet("bookmarks")]
        public IEnumerable<MovieBookmark> GetBookmarkedMovies()
        {
            var lastSeenMovies = movieBookmarkProvider.GetMovieBookmarks(AppFiles.BookmarkedMovies);
            return lastSeenMovies?.Reverse();
        }

        [HttpPut("bookmarks")]
        public IActionResult BookmarkMovie([FromBody] MovieDto movie)
        {
            var movieBookmark = new MovieBookmark()
            {
                Movie = movie,
                ServiceName = movieServiceProvider.GetActiveServiceTypeName()
            };

            try
            {
                movieBookmarkProvider.SaveMovieBookmark(movieBookmark, AppFiles.BookmarkedMovies);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("bookmarks")]
        public IActionResult DeleteBookmarkMovie([FromBody] MovieBookmark movieBookmark)
        {
            try
            {
                movieBookmarkProvider.DeleteMovieBookmark(movieBookmark.Movie.Id, movieBookmark.ServiceName, AppFiles.BookmarkedMovies);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks/exists")]
        public bool MovieBookmarkExists([FromQuery] string movieId, [FromQuery] string serviceName)
        {
            return movieBookmarkProvider.MovieBookmarkExists(movieId, serviceName, AppFiles.BookmarkedMovies);
        }
    }
}
