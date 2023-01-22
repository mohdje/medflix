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
    public class MoviesController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        IWatchedMediaProvider watchedMediaProvider;
        IBookmarkedMediaProvider bookmarkedMediaProvider;
        public MoviesController(
            ISearchersProvider searchersProvider,
            IWatchedMediaProvider watchedMediaProvider,
            IBookmarkedMediaProvider bookmarkedMediaProvider)
        {
            this.searchersProvider = searchersProvider;
            this.watchedMediaProvider = watchedMediaProvider;
            this.bookmarkedMediaProvider = bookmarkedMediaProvider;
        }
        [HttpGet("genres")]
        public async Task<IEnumerable<Genre>> GetMoviesGenres()
        {
            return await searchersProvider.MovieSearcher.GetMovieGenresAsync();
        }

        [HttpGet("mediasoftoday")]
        public async Task<IEnumerable<LiteContentDto>> GetMoviesOfToday()
        {
            var movies = await searchersProvider.MovieSearcher.GetMoviesOfTodayAsync();

            return movies.Where(m => !string.IsNullOrEmpty(m.LogoImageUrl)).Take(5);
        }

        [HttpGet("netflix")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularNetflixMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularNetflixMoviesAsync();
        }

        [HttpGet("disneyplus")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularDisneyPlusMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularDisneyPlusMoviesAsync();
        }

        [HttpGet("amazonprime")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularAmazonPrimeMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularAmazonPrimeMoviesAsync();
        }

        [HttpGet("popular")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularMoviesAsync();
        }

        [HttpGet("similar/{id}")]
        public async Task<IEnumerable<LiteContentDto>> GetSimilarMovies(string id)
        {
            return await searchersProvider.MovieSearcher.GetRecommandationsAsync(id);
        }

        [HttpGet("recommandations")]
        public async Task<IEnumerable<LiteContentDto>> GetRecommandations([FromQuery] string id)
        {
            var movieId = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                var movies = await watchedMediaProvider.GetWatchedMoviesAsync();
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
        public async Task<IEnumerable<LiteContentDto>> GetPopularMoviesByGenre(int genreId)
        {
            return await searchersProvider.MovieSearcher.GetPopularMoviesByGenreAsync(genreId);
        }

        [HttpGet("genre/{genreId}/{page}")]
        public async Task<IEnumerable<LiteContentDto>> GetMoviesByGenre(int genreId, int page)
        {
            return await searchersProvider.MovieSearcher.GetMoviesByGenreAsync(genreId, page);
        }

        [HttpGet("search")]
        public async Task<IEnumerable<LiteContentDto>> SearchMovies([FromQuery(Name ="t")] string text)
        {
            return await searchersProvider.MovieSearcher.SearchMoviesAsync(text);
        }

        [HttpGet("details/{id}")]
        public async Task<ContentDto> GetMovieDetails(string id)
        {
            return await searchersProvider.MovieSearcher.GetMovieDetailsAsync(id);
        }

        [HttpGet("watchedmedia")]
        public async Task<IEnumerable<WatchedMediaDto>> GetWatchedMovies()
        {
            var movies = await watchedMediaProvider.GetWatchedMoviesAsync();
            return movies?.Reverse();
        }

        [HttpGet("watchedmedia/{id}")]
        public async Task<WatchedMediaDto> GetWatchedMovie(int id)
        {
            var movies = await watchedMediaProvider.GetWatchedMoviesAsync();
            return movies?.SingleOrDefault(m => m.Id == id.ToString());
        }

        [HttpPut("watchedmedia")]
        public async Task<IActionResult> SaveWatchedMovie([FromBody] WatchedMediaDto watchedMovie)
        {
            try
            {
                await watchedMediaProvider.SaveWatchedMovieAsync(watchedMovie);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks")]
        public async Task<IEnumerable<LiteContentDto>> GetBookmarkedMovies()
        {
            var movies = await bookmarkedMediaProvider.GetBookmarkedMoviesAsync();
            return movies?.Reverse();
        }



        [HttpPut("bookmarks")]
        public async Task<IActionResult> BookmarkMovie([FromBody] LiteContentDto movieToBookmark)
        {
            try
            {
                await bookmarkedMediaProvider.SaveMovieBookmarkAsync(movieToBookmark);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("bookmarks")]
        public async Task<IActionResult> DeleteBookmarkMovie([FromBody] LiteContentDto movieBookmarkToDelete)
        {
            try
            {
                await bookmarkedMediaProvider.DeleteMovieBookmarkAsync(movieBookmarkToDelete.Id);
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
                return Ok(await bookmarkedMediaProvider.MovieBookmarkExistsAsync(movieId));
            }
            else
                return BadRequest();
        }
    }
}
