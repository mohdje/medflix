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
using MoviesAPI.Services.VOMovies;
using MoviesAPI.Services.VFMovies.VFMoviesSearchers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        IVOMovieSearcherProvider voMovieSearcherProvider;
        VOMovieSearcher voMovieSearcher;
        VFMoviesSearcher vfMovieSearcher;

        IMovieStreamProvider movieStreamProvider;
        IMovieBookmarkProvider movieBookmarkProvider;
        public MoviesController(
            IVOMovieSearcherProvider voMovieSearcherProvider,
            IVFMovieSearcherProvider vfMovieSearcherProvider,
            IMovieStreamProvider movieStreamProvider,
            IMovieBookmarkProvider movieBookmarkProvider)
        {
            this.voMovieSearcherProvider = voMovieSearcherProvider;
            this.voMovieSearcher = voMovieSearcherProvider.GetActiveVOMovieSearcher();

            this.vfMovieSearcher = vfMovieSearcherProvider.GetActiveVFMovieSearcher();

            this.movieStreamProvider = movieStreamProvider;
            this.movieBookmarkProvider = movieBookmarkProvider;
        }
        [HttpGet("genres")]
        public IEnumerable<string> GetMoviesGenres()
        {
            return voMovieSearcher.GetMovieGenres();
        }

        [HttpGet("suggested")]
        public async Task<IEnumerable<MovieDto>> GetSuggestedMovies()
        {
            return await voMovieSearcher.GetSuggestedMoviesAsync(5);
        }

        [HttpGet("genre/{genre}")]
        public async Task<IEnumerable<MovieDto>> GetLastMoviesByGenre(string genre)
        {
            return await voMovieSearcher.GetLastMoviesByGenreAsync(15, genre);
        }

        [HttpGet("genre/{genre}/{page}")]
        public async Task<IEnumerable<MovieDto>> GetLastMoviesByGenre(string genre, int page)
        {
            return await voMovieSearcher.GetMoviesByGenreAsync(genre, page);
        }

        [HttpGet("search/{text}")]
        public async Task<IEnumerable<MovieDto>> SearchMovies(string text)
        {
            return await voMovieSearcher.GetMoviesByNameAsync(text);
        }

        [HttpGet("details/{id}")]
        public async Task<MovieDto> GetMovieDetails(string id)
        {
            return await voMovieSearcher.GetMovieDetailsAsync(id);
        }

        [HttpGet("vf")]
        public async Task<IEnumerable<MovieTorrent>> SearchVF([FromQuery(Name = "title")] string title, [FromQuery(Name = "year")] string year)
        {
            var movies = await vfMovieSearcher.GetMovieTorrentsAsync(title, int.Parse(year), true);

            return movies;
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

        [HttpGet("lastseenmovies")]
        public IEnumerable<MovieBookmark> GetLastSeenMovies()
        {
            var lastSeenMovies = movieBookmarkProvider.GetMovieBookmarks(AppFiles.LastSeenMovies);
            return lastSeenMovies?.Reverse();
        }

        [HttpPut("lastseenmovies")]
        public void SaveLastSeenMovie([FromBody] MovieDto movie)
        {
            var serviceInfo = voMovieSearcherProvider.GetSelectedVOMoviesServiceInfo(false);

            var movieBookmark = new MovieBookmark()
            {
                Movie = movie,
                ServiceName = serviceInfo.Description,
                ServiceId = serviceInfo.Id
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
            var serviceInfo = voMovieSearcherProvider.GetSelectedVOMoviesServiceInfo(false);

            var movieBookmark = new MovieBookmark()
            {
                Movie = movie,
                ServiceName = serviceInfo.Description,
                ServiceId = serviceInfo.Id
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
