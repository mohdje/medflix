using MoviesAPI.Services.Content.Dtos;
using System.Collections.Generic;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public class BookmarkedSeriesProvider : BookmarkedMediaProvider, IBookmarkedSeriesProvider
    {
        protected override string FilePath => AppFiles.BookmarkedSeries;

        public void AddSerieBookmark(LiteContentDto serieToBookmark)
        {
            AddBookmark(serieToBookmark);
        }

        public void DeleteSerieBookmark(string serieId)
        {
            DeleteBookmark(serieId);
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
