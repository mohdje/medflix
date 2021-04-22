using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class MovieBookmarkProvider : IMovieBookmarkProvider
    {
        public void DeleteMovieBookmark(string movieId, string serviceName, string filePath)
        {
            var movieBookmarks = GetMovieBookmarks(filePath);

            if (movieBookmarks != null)
            {
                movieBookmarks = movieBookmarks.Where(m => !(m.ServiceName == serviceName && m.Movie.Id == movieId));
                JsonHelper.SerializeToFileAsync(filePath, movieBookmarks);
            }
        }

        public IEnumerable<MovieBookmark> GetMovieBookmarks(string filePath)
        {        
           if(!System.IO.File.Exists(filePath))
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

        public bool MovieBookmarkExists(string movieId, string serviceName, string filePath)
        {
            var movieBookmarks = GetMovieBookmarks(filePath);

            if (movieBookmarks != null)
                return movieBookmarks.Any(m => m.Movie.Id == movieId && m.ServiceName == serviceName);

            return false;
        }

        public void SaveMovieBookmark(MovieBookmark movieBookmark, string filePath, int? maxLimit = null)
        {
            var movieBookmarks = GetMovieBookmarks(filePath);

            if (movieBookmarks != null)
            {
                if (movieBookmarks.Any(m => m.Movie.Id == movieBookmark.Movie.Id && m.ServiceName == movieBookmark.ServiceName))
                    return;

                var movieBookmarksList = movieBookmarks.ToList();
                if (maxLimit.HasValue && movieBookmarks.Count() == maxLimit.Value)
                    movieBookmarksList.RemoveAt(0);

                movieBookmarksList.Add(movieBookmark);
                movieBookmarks = movieBookmarksList.ToArray();
            }
            else
                movieBookmarks = new MovieBookmark[] { movieBookmark };

           JsonHelper.SerializeToFileAsync(filePath, movieBookmarks);
        }
    }
}
