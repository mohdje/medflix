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
        IWatchedMoviesProvider watchedMoviesProvider;
        IBookmarkedMoviesProvider bookmarkedMoviesProvider;
        public MoviesController(
            ISearchersProvider searchersProvider,
            ITorrentContentProvider torrentVideoStreamProvider,
            IWatchedMoviesProvider watchedMoviesProvider,
            IBookmarkedMoviesProvider bookmarkedMoviesProvider)
        {
            this.searchersProvider = searchersProvider;
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

        [HttpGet("similar/{id}")]
        public async Task<IEnumerable<LiteMovieDto>> GetSimilarMovies(string id)
        {
            return await searchersProvider.MovieSearcher.GetRecommandationsAsync(id);
        }

        [HttpGet("recommandations")]
        public async Task<IEnumerable<LiteMovieDto>> GetRecommandations([FromQuery]string id)
        {
            var movieId = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                var movies = await watchedMoviesProvider.GetWatchedMoviesAsync();
                if (movies == null || !movies.Any())
                    return null;
                else
                    movieId = movies.Last().Id;
            }
            else
                movieId = id;

            return await searchersProvider.MovieSearcher.GetRecommandationsAsync(movieId);
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


        [HttpGet("watchedmovies")]
        public async Task<IEnumerable<WatchedMovieDto>> GetWatchedMovies()
        {
            var movies = await watchedMoviesProvider.GetWatchedMoviesAsync();
            return movies?.Reverse();
        }

        [HttpGet("watchedmovies/{id}")]
        public async Task<WatchedMovieDto> GetWatchedMovie(int id)
        {
            var movies = await watchedMoviesProvider.GetWatchedMoviesAsync();
            return movies?.SingleOrDefault(m => m.Id == id.ToString());
        }

        [HttpPut("watchedmovies")]
        public async Task<IActionResult> SaveWatchedMovie([FromBody] WatchedMovieDto watchedMovie)
        {
            try
            {
                await watchedMoviesProvider.SaveWatchedMovieAsync(watchedMovie);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks")]
        public async Task<IEnumerable<LiteMovieDto>> GetBookmarkedMovies()
        {
            var movies = await bookmarkedMoviesProvider.GetBookmarkedMoviesAsync();
            return movies?.Reverse();
        }



        [HttpPut("bookmarks")]
        public async Task<IActionResult> BookmarkMovie([FromBody] LiteMovieDto movieToBookmark)
        {
            try
            {
                await bookmarkedMoviesProvider.SaveMovieBookmarkAsync(movieToBookmark);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("bookmarks")]
        public async Task<IActionResult> DeleteBookmarkMovie([FromBody] LiteMovieDto movieBookmarkToDelete)
        {
            try
            {
                await bookmarkedMoviesProvider.DeleteMovieBookmarkAsync(movieBookmarkToDelete.Id);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks/exists")]
        public async Task<IActionResult> MovieBookmarkExists([FromQuery] string movieId)
        {
            if (!string.IsNullOrEmpty(movieId))
            {
                return Ok(await bookmarkedMoviesProvider.MovieBookmarkExistsAsync(movieId));
            }
            else
                return BadRequest();
        }
    }
}
