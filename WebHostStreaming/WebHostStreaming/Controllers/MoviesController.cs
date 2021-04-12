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
            return await movieService.GetMoviesDetailsAsync(id);
        }

        [HttpGet("stream")]
        public async Task<FileStreamResult> GetStream([FromQuery(Name = "url")] string url)
        {
            var rangeHeaderValue = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();

            long offset = 0;
            long.TryParse(rangeHeaderValue.Replace("bytes=", string.Empty).Split("-")[0], out offset);

            return File(await movieStreamProvider.GetMovieStreamAsync(url, offset), "video/mp4", true);

        }

        [HttpGet("lastseenmovies")]
        public IEnumerable<MovieBookmark> GetLastSeenMovies()
        {
            var lastSeenMovies = movieBookmarkProvider.GetMovieBookmarks(AppFiles.LastSeenMovies);
            return lastSeenMovies.Reverse();
        }

        [HttpPut("lastseenmovies")]
        public void SaveLastSeenMovie([FromBody] MovieDto movie)
        {
            var movieBookmark = new MovieBookmark()
            {
                Movie = movie,
                ServiceName = movieServiceProvider.GetActiveServiceTypeName()
            };

            movieBookmarkProvider.SaveMovieBookmark(movieBookmark, AppFiles.LastSeenMovies);          
        }

       
    }
}
