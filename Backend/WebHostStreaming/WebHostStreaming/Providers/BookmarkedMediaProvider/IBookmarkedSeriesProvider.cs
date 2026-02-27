
using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;

namespace WebHostStreaming.Providers
{
    public interface IBookmarkedSeriesProvider
    {
        IEnumerable<LiteContentDto> GetBookmarkedSeries();
        void AddSerieBookmark(LiteContentDto serieToBookmark);
        void DeleteSerieBookmark(string serieId);
        bool SerieBookmarkExists(string serieId);
        event EventHandler<LiteContentDto> SerieBookmarkAdded;
        event EventHandler<string> SerieBookmarkDeleted;
    }
}
