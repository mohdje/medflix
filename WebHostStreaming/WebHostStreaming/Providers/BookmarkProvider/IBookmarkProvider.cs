using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IBookmarkProvider
    {
        IEnumerable<MovieBookmark> GetMovieBookmarks();
        void SaveMovieBookmark(MovieBookmark movieBookmark, int? maxLimit = null);
        void DeleteMovieBookmark(string movieId, int serviceId);
        bool MovieBookmarkExists(string movieId, int serviceId);
    }
}
