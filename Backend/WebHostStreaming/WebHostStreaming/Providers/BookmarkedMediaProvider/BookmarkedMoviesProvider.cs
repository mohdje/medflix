using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using WebHostStreaming.Helpers;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public class BookmarkedMoviesProvider : BookmarkedMediaProvider, IBookmarkedMoviesProvider
    {
        protected override string FilePath => AppFiles.BookmarkedMovies;

        public event EventHandler MovieBookmarkAdded;
        public event EventHandler MovieBookmarkDeleted;

        public IEnumerable<LiteContentDto> GetBookmarkedMovies()
        {
            return GetBookmarks();
        }

        public void AddMovieBookmark(LiteContentDto movieToBookmark)
        {
            AddBookmark(movieToBookmark);
            MovieBookmarkAdded?.Invoke(this, EventArgs.Empty);
        }

        public void DeleteMovieBookmark(string movieId)
        {
            DeleteBookmark(movieId);
            MovieBookmarkDeleted?.Invoke(this, EventArgs.Empty);
        }

        public bool MovieBookmarkExists(string movieId)
        {
            return BookmarkExists(movieId);
        }
    }
}
