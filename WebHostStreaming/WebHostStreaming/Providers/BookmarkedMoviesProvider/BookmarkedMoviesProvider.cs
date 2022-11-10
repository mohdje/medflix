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
    public class BookmarkedMoviesProvider : IBookmarkedMoviesProvider
    {
        private const int MaxLimit = 30;

        private string FilePath = AppFiles.BookmarkedMovies;

        public void DeleteMovieBookmark(string movieId)
        {
            var movieBookmarks = GetBookmarkedMovies();

            if (movieBookmarks != null)
            {
                movieBookmarks = movieBookmarks.Where(m => m.Id != movieId);
                JsonHelper.SerializeToFileAsync(FilePath, movieBookmarks);
            }
        }

        public IEnumerable<LiteMovieDto> GetBookmarkedMovies()
        {
            var filePath = FilePath;
            if (!System.IO.File.Exists(filePath))
                return null;

            try
            {
                return JsonHelper.DeserializeFromFile<LiteMovieDto[]>(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool MovieBookmarkExists(string movieId)
        {
            var movieBookmarks = GetBookmarkedMovies();

            if (movieBookmarks != null)
                return movieBookmarks.Any(m => m.Id == movieId);

            return false;
        }

        public void SaveMovieBookmark(LiteMovieDto movieToBookmark)
        {
            var movieBookmarks = GetBookmarkedMovies();

            if (movieBookmarks != null)
            {
                if (movieBookmarks.Any(m => m.Id == movieToBookmark.Id))
                    return;

                var movieBookmarksList = movieBookmarks.ToList();
                if (movieBookmarks.Count() == MaxLimit)
                    movieBookmarksList.RemoveAt(0);

                movieBookmarksList.Add(movieToBookmark);
                movieBookmarks = movieBookmarksList.ToArray();
            }
            else
                movieBookmarks = new LiteMovieDto[] { movieToBookmark };

            if (!Directory.Exists(AppFolders.DataFolder))
                Directory.CreateDirectory(AppFolders.DataFolder);

            JsonHelper.SerializeToFileAsync(FilePath, movieBookmarks);
        }
    }
}
