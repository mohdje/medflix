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
        IRecommandationsProvider recommandationsProvider;
        public MoviesController(
            ISearchersProvider searchersProvider,
            IWatchedMediaProvider watchedMediaProvider,
            IBookmarkedMediaProvider bookmarkedMediaProvider,
            IRecommandationsProvider recommandationsProvider)
        {
            this.searchersProvider = searchersProvider;
            this.watchedMediaProvider = watchedMediaProvider;
            this.bookmarkedMediaProvider = bookmarkedMediaProvider;
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
        public async Task<IEnumerable<WatchedMediaDto>> GetWatchedMovies()
        {
            var movies = await watchedMediaProvider.GetWatchedMoviesAsync();
            return movies?.Reverse().Take(30);
        }

        [HttpGet("watchedmedia/{id}")]
        public async Task<WatchedMediaDto> GetWatchedMovie(int id)
        {
            var movies = await watchedMediaProvider.GetWatchedMoviesAsync();
            return movies?.SingleOrDefault(m => m.Media.Id == id.ToString());
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
        public async Task<IActionResult> DeleteBookmarkMovie([FromQuery(Name = "id")] string movieId)
        {
            try
            {
                await bookmarkedMediaProvider.DeleteMovieBookmarkAsync(movieId);
            }
            catch (Exception)
            {

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpGet("bookmarks/exists")]
        public async Task<IActionResult> BookmarkExists([FromQuery(Name = "id")] string movieId)
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
