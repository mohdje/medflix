using MoviesAPI.Services.Content.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace WebHostStreaming.Providers
{
    public abstract class BookmarkedMediaProvider : DataStoreProvider<LiteContentDto> 
    {
        protected override int MaxLimit => 30;

        protected LiteContentDto DeleteBookmark(string mediaId)
        {
            var mediaToDelete = Data.FirstOrDefault(m => m.Id == mediaId);

            if (mediaToDelete != null)
                RemoveData(mediaToDelete);

            return mediaToDelete;   
        }

        protected IEnumerable<LiteContentDto> GetBookmarks()
        {
            return Data.Reverse().Take(MaxLimit);
        }

        protected bool BookmarkExists(string mediaId)
        {
            return Data.Any(m => m.Id == mediaId);
        }

        protected void AddBookmark(LiteContentDto movieToBookmark)
        {
            if(Data.Any(m => m.Id == movieToBookmark.Id))
                return;

            AddData(movieToBookmark);
        }
    }
}
