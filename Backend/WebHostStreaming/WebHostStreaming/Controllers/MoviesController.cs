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
    public class MoviesController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        IWatchedMoviesProvider watchedMoviesProvider;
        IBookmarkedMoviesProvider bookmarkedMoviesProvider;
        IRecommandationsProvider recommandationsProvider;
        public MoviesController(
            ISearchersProvider searchersProvider,
            IWatchedMoviesProvider watchedMoviesProvider,
            IBookmarkedMoviesProvider bookmarkedMoviesProvider,
            IRecommandationsProvider recommandationsProvider)
        {
            this.searchersProvider = searchersProvider;
            this.watchedMoviesProvider = watchedMoviesProvider;
            this.bookmarkedMoviesProvider = bookmarkedMoviesProvider;
            this.recommandationsProvider = recommandationsProvider;
        }

        [HttpGet("genres")]
        public async Task<IEnumerable<Genre>> GetMoviesGenres()
        {
            return await searchersProvider.MovieSearcher.GetMovieGenresAsync();
        }

        [HttpGet("platforms")]
        public async Task<IEnumerable<Platform>> GetMoviesPlatforms()
        {
            return await searchersProvider.MovieSearcher.GetMoviePlatformsAsync();
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


        [HttpGet("appletv")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularAppleTvMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularAppleTvMoviesAsync();
        }

        [HttpGet("popular")]
        public async Task<IEnumerable<LiteContentDto>> GetPopularMovies()
        {
            return await searchersProvider.MovieSearcher.GetPopularMoviesAsync();
        }

        [HttpGet("similar/{id}")]
        public async Task<IEnumerable<LiteContentDto>> GetSimilarMovies(string id)
        {
            return await searchersProvider.MovieSearcher.GetSimilarMoviesAsync(id);
        }

        [HttpGet("recommandations")]
        public async Task<IEnumerable<LiteContentDto>> GetRecommandations()
        {
            return await recommandationsProvider.GetMoviesRecommandationsAsync();
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

        [HttpGet("platform/{platformId}/{page}")]
        public async Task<IEnumerable<LiteContentDto>> GetMoviesByPlatform(int platformId, int page)
        {
            return await searchersProvider.MovieSearcher.GetMoviesByPlatformAsync(platformId, page);
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
        public IEnumerable<WatchedMediaDto> GetWatchedMovies()
        {
            return watchedMoviesProvider.GetWatchedMovies();
        }

        [HttpGet("watchedmedia/{id}")]
        public WatchedMediaDto GetWatchedMovie(int id)
        {
            return watchedMoviesProvider.GetWatchedMovie(id);
        }

        [HttpPut("watchedmedia")]
        public ActionResult SaveWatchedMovie([FromBody] WatchedMediaDto watchedMovie)
        {
            try
            {
                watchedMoviesProvider.SaveWatchedMovie(watchedMovie);
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks")]
        public IEnumerable<LiteContentDto> GetBookmarkedMovies()
        {
            return bookmarkedMoviesProvider.GetBookmarkedMovies();
        }

        [HttpPut("bookmarks")]
        public IActionResult BookmarkMovie([FromBody] LiteContentDto movieToBookmark)
        {
            try
            {
                bookmarkedMoviesProvider.AddMovieBookmark(movieToBookmark);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpDelete("bookmarks")]
        public IActionResult DeleteBookmarkMovie([FromQuery(Name = "id")] string movieId)
        {
            try
            {
                bookmarkedMoviesProvider.DeleteMovieBookmark(movieId);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks/exists")]
        public IActionResult BookmarkExists([FromQuery(Name = "id")] string movieId)
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
