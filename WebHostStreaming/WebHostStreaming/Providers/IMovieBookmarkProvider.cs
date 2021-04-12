using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IMovieBookmarkProvider
    {
        IEnumerable<MovieBookmark> GetMovieBookmarks(string filePath);

        void SaveMovieBookmark(MovieBookmark movieBookmark, string filePath);
    }
}
