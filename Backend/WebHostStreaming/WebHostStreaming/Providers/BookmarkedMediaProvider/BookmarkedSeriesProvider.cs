using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public class BookmarkedSeriesProvider : BookmarkedMediaProvider, IBookmarkedSeriesProvider
    {
        protected override string FilePath => AppFiles.BookmarkedSeries;

        public event EventHandler<LiteContentDto> SerieBookmarkAdded;
        public event EventHandler<string> SerieBookmarkDeleted;

        public void AddSerieBookmark(LiteContentDto serieToBookmark)
        {
            AddBookmark(serieToBookmark);
            SerieBookmarkAdded?.Invoke(this, serieToBookmark);
        }

        public void DeleteSerieBookmark(string serieId)
        {
            DeleteBookmark(serieId);
            SerieBookmarkDeleted?.Invoke(this, serieId);
        }

        public IEnumerable<LiteContentDto> GetBookmarkedSeries()
        {
            return GetBookmarks();
        }

        public bool SerieBookmarkExists(string serieId)
        {
            return BookmarkExists(serieId);
        }
    }
}
