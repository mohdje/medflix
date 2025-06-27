using MoviesAPI.Services.Content.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public class BookmarkedMediaProvider : DataProvider, IBookmarkedMediaProvider 
    {
        private readonly string BookmarkedMoviesFile = AppFiles.BookmarkedMovies;
        private readonly string BookmarkedSeriesFile = AppFiles.BookmarkedSeries;

        ITorrentAutoDownloader torrentAutoDownloader;
        public BookmarkedMediaProvider(ITorrentAutoDownloader torrentAutoDownloader)
        {
            this.torrentAutoDownloader = torrentAutoDownloader;
        }

        protected override int MaxLimit()
        {
            return 30;
        }

        public async Task InitDownloadBookmarkedMoviesAsync()
        {
            var bookmarkedMovies = await GetBookmarkedMoviesAsync();

            if (bookmarkedMovies == null || !bookmarkedMovies.Any())
                return;

            foreach (var movie in bookmarkedMovies)
                torrentAutoDownloader.AddToDownloadList(movie);
        }

        public async Task DeleteMovieBookmarkAsync(string movieId)
        {
            var movieBookmarks = await GetBookmarkedMoviesAsync();

            if (movieBookmarks != null)
            {
                JsonHelper.SerializeToFile(BookmarkedMoviesFile, movieBookmarks.Where(m => m.Id != movieId));
                torrentAutoDownloader.RemoveFromDownloadList(movieBookmarks.FirstOrDefault(m => m.Id == movieId));
            }
        }

        public async Task<IEnumerable<LiteContentDto>> GetBookmarkedMoviesAsync()
        {
            return await GetDataAsync<LiteContentDto>(BookmarkedMoviesFile);
        }

        public async Task<bool> MovieBookmarkExistsAsync(string movieId)
        {
            var movieBookmarks = await GetBookmarkedMoviesAsync();

            if (movieBookmarks != null)
                return movieBookmarks.Any(m => m.Id == movieId);

            return false;
        }

        public async Task SaveMovieBookmarkAsync(LiteContentDto movieToBookmark)
        {
            torrentAutoDownloader.AddToDownloadList(movieToBookmark);
            await SaveDataAsync(BookmarkedMoviesFile, movieToBookmark, (m1, m2) => m1.Id == m2.Id);
        }

        public async Task SaveSerieBookmarkAsync(LiteContentDto serieToBookmark)
        {
            await SaveDataAsync(BookmarkedSeriesFile, serieToBookmark, (m1, m2) => m1.Id == m2.Id);
        }

        public async Task DeleteSerieBookmarkAsync(string serieId)
        {
            var serieBookmarks = await GetBookmarkedSeriesAsync();

            if (serieBookmarks != null)
            {
                serieBookmarks = serieBookmarks.Where(m => m.Id != serieId);
                JsonHelper.SerializeToFile(BookmarkedSeriesFile, serieBookmarks);
            }
        }

        public async Task<bool> SerieBookmarkExistsAsync(string serieId)
        {
            var serieBookmarks = await GetBookmarkedSeriesAsync();

            if (serieBookmarks != null)
                return serieBookmarks.Any(m => m.Id == serieId);

            return false;
        }

        public async Task<IEnumerable<LiteContentDto>> GetBookmarkedSeriesAsync()
        {
            return await GetDataAsync<LiteContentDto>(BookmarkedSeriesFile);
        }
    }
}
