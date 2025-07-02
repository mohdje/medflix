using MoviesAPI.Services.Content.Dtos;
using System.Collections.Generic;
using System.Linq;
using WebHostStreaming.Helpers;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public class BookmarkedMoviesProvider : BookmarkedMediaProvider, IBookmarkedMoviesProvider
    {
        ITorrentAutoDownloader torrentAutoDownloader;

        protected override string FilePath => AppFiles.BookmarkedMovies;

        public BookmarkedMoviesProvider(ITorrentAutoDownloader torrentAutoDownloader)
        {
            this.torrentAutoDownloader = torrentAutoDownloader;
        }

        public void InitDownloadBookmarkedMovies()
        {
            if (!Data.Any())
                return;

            torrentAutoDownloader.AddToDownloadList(Data);
        }

        public IEnumerable<LiteContentDto> GetBookmarkedMovies()
        {
            return GetBookmarks();
        }

        public void AddMovieBookmark(LiteContentDto movieToBookmark)
        {
            AddBookmark(movieToBookmark);
            torrentAutoDownloader.AddToDownloadList(movieToBookmark);
        }

        public void DeleteMovieBookmark(string movieId)
        {
            var deletedMovieBookmark = DeleteBookmark(movieId);
            if (deletedMovieBookmark != null)
                torrentAutoDownloader.RemoveFromDownloadList(deletedMovieBookmark);
        }

        public bool MovieBookmarkExists(string movieId)
        {
            return BookmarkExists(movieId);
        }
    }
}
