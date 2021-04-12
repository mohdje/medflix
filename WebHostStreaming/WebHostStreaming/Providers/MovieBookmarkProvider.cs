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

        public void SaveMovieBookmark(MovieBookmark movieBookmark, string filePath)
        {
            var movieBookmarks = GetMovieBookmarks(filePath);

            if (movieBookmarks != null)
            {
                if (movieBookmarks.Any(m => m.Movie.Id == movieBookmark.Movie.Id && m.ServiceName == movieBookmark.ServiceName))
                    return;

                var movieBookmarksList = movieBookmarks.ToList();
                movieBookmarksList.Add(movieBookmark);
                movieBookmarks = movieBookmarksList.ToArray();
            }
            else
                movieBookmarks = new MovieBookmark[] { movieBookmark };

           JsonHelper.SerializeToFileAsync(AppFiles.LastSeenMovies, movieBookmarks);
        }
    }
}
