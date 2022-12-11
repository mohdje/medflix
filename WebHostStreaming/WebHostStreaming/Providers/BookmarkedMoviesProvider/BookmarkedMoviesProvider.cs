using MoviesAPI.Services.Movies.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class BookmarkedMoviesProvider : DataProvider, IBookmarkedMoviesProvider 
    {
        protected override string FilePath()
        {
            return AppFiles.BookmarkedMovies; 
        }

        protected override int MaxLimit()
        {
            return 30;
        }

        public async Task DeleteMovieBookmarkAsync(string movieId)
        {
            var movieBookmarks = await GetBookmarkedMoviesAsync();

            if (movieBookmarks != null)
            {
                movieBookmarks = movieBookmarks.Where(m => m.Id != movieId);
                JsonHelper.SerializeToFileAsync(FilePath(), movieBookmarks);
            }
        }

        public async Task<IEnumerable<LiteMovieDto>> GetBookmarkedMoviesAsync()
        {
            return await GetDataAsync<LiteMovieDto>();
        }

        public async Task<bool> MovieBookmarkExistsAsync(string movieId)
        {
            var movieBookmarks = await GetBookmarkedMoviesAsync();

            if (movieBookmarks != null)
                return movieBookmarks.Any(m => m.Id == movieId);

            return false;
        }

        public async Task SaveMovieBookmarkAsync(LiteMovieDto movieToBookmark)
        {
            await SaveDataAsync(movieToBookmark, (m1, m2) => m1.Id == m2.Id);
        }

       
    }
}
