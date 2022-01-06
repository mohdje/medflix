using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public abstract class BookmarkProvider : IBookmarkProvider
    {
        private string FilePath => GetFilePath();

        protected abstract string GetFilePath();

        public void DeleteMovieBookmark(string movieId, int serviceId)
        {
            var movieBookmarks = GetMovieBookmarks();

            if (movieBookmarks != null)
            {
                movieBookmarks = movieBookmarks.Where(m => !(m.ServiceId == serviceId && m.Movie.Id == movieId));
                JsonHelper.SerializeToFileAsync(FilePath, movieBookmarks);
            }
        }

        public IEnumerable<MovieBookmark> GetMovieBookmarks()
        {
            var filePath = FilePath;
            if (!System.IO.File.Exists(filePath))
                return null;

            try
            {
                return JsonHelper.DeserializeFromFile<MovieBookmark[]>(filePath);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool MovieBookmarkExists(string movieId, int serviceId)
        {
            var movieBookmarks = GetMovieBookmarks();

            if (movieBookmarks != null)
                return movieBookmarks.Any(m => m.Movie.Id == movieId && m.ServiceId == serviceId);

            return false;
        }

        public void SaveMovieBookmark(MovieBookmark movieBookmark, int? maxLimit = null)
        {
            var movieBookmarks = GetMovieBookmarks();

            if (movieBookmarks != null)
            {
                if (movieBookmarks.Any(m => m.Movie.Id == movieBookmark.Movie.Id && m.ServiceId == movieBookmark.ServiceId))
                    return;

                var movieBookmarksList = movieBookmarks.ToList();
                if (maxLimit.HasValue && movieBookmarks.Count() == maxLimit.Value)
                    movieBookmarksList.RemoveAt(0);

                movieBookmarksList.Add(movieBookmark);
                movieBookmarks = movieBookmarksList.ToArray();
            }
            else
                movieBookmarks = new MovieBookmark[] { movieBookmark };

            JsonHelper.SerializeToFileAsync(FilePath, movieBookmarks);
        }
    }
}
