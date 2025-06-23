using System.Threading.Tasks;
using WebHostStreaming.Providers;
using WebHostStreaming.Providers.AvailableVideosListProvider;
using MoviesAPI.Services.Content.Dtos;
using System.Linq;
using System;
using WebHostStreaming.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using System.Collections.Generic;
using System.Threading;


namespace WebHostStreaming.Torrent
{
    public class TorrentAutoDownloader : ITorrentAutoDownloader
    {
        IAvailableVideosListProvider availableVideosListProvider;
        ISearchersProvider searchersProvider;
        ITorrentContentProvider torrentContentProvider;

        readonly string[] qualitiesRank = { "2160p", "1080p", "720p", "WEBRIP" };
        const string TorrentAutoDownloaderIdentifier = "Auto-Downloader";
        bool running;
        List<LiteContentDto> moviesToDownload = new List<LiteContentDto>();
        CancellationTokenSource downloadCancellationTokenSource;

        public TorrentAutoDownloader(
            IAvailableVideosListProvider availableVideosListProvider,
            ISearchersProvider searchersProvider,
            ITorrentContentProvider torrentContentProvider)
        {
            this.availableVideosListProvider = availableVideosListProvider;
            this.searchersProvider = searchersProvider;
            this.torrentContentProvider = torrentContentProvider;
            torrentContentProvider.OnNoActiveTorrentClient += (s, e) => DownloadMoviesAsync();
        }

        public void AddToDownloadList(LiteContentDto movieToDownload)
        {
            AppLogger.LogInfo($"TorrentAutoDownloader: Add to the list {movieToDownload.Title} {movieToDownload.Year}");

            if (!moviesToDownload.Any(movie => movie.Id == movieToDownload.Id))
                moviesToDownload.Add(movieToDownload);

            DownloadMoviesAsync();
        }

        public void RemoveFromDownloadList(LiteContentDto movieToDownload)
        {
            AppLogger.LogInfo($"TorrentAutoDownloader: Remove from the list {movieToDownload.Title} {movieToDownload.Year}");

            moviesToDownload.Remove(movieToDownload);
        }

        public void StopDownload()
        {
            downloadCancellationTokenSource?.Cancel();
        }

        private async Task DownloadMoviesAsync()
        {
            AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Starts");

            if (running)
                return;

            running = true;

            var failedDownloadMovies = new List<LiteContentDto>();

            downloadCancellationTokenSource = new CancellationTokenSource();

            while (moviesToDownload.Any() && !downloadCancellationTokenSource.IsCancellationRequested)
            {
                var movieToDownload = moviesToDownload.First();
                var voDownloadSuccess = await DownloadOriginalVersion(movieToDownload);
                var vfDownloadSuccess = await DownloadFrenchVersion(movieToDownload);

                moviesToDownload.Remove(movieToDownload);

                if (voDownloadSuccess)
                    AppLogger.LogInfo($"TorrentAutoDownloader: VO ready for {movieToDownload.Title} {movieToDownload.Year}");

                if (vfDownloadSuccess)
                    AppLogger.LogInfo($"TorrentAutoDownloader: VF ready for {movieToDownload.Title} {movieToDownload.Year}");

                if (!voDownloadSuccess || !vfDownloadSuccess)
                    failedDownloadMovies.Add(movieToDownload);
            }

            moviesToDownload.AddRange(failedDownloadMovies.Where(fm => !moviesToDownload.Any(m => m.Id == fm.Id)));

            if (moviesToDownload.Any() && !downloadCancellationTokenSource.IsCancellationRequested)
            {
                AppLogger.LogInfo($"TorrentAutoDownloader.DownloadMoviesAsync(): {failedDownloadMovies.Count} movies failed to download, retry in 1 hour");
                var timer = new Timer(_ =>
                {
                    DownloadMoviesAsync();
                }, null, TimeSpan.FromHours(1), Timeout.InfiniteTimeSpan);
            }

            running = false;

            AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Ends");
        }

        private async Task<bool> DownloadOriginalVersion(LiteContentDto movie)
        {
            if (downloadCancellationTokenSource.IsCancellationRequested)
                return false;

            var videoPath = availableVideosListProvider.GetVoMovieSource(movie.Title, movie.Year);
            if (!string.IsNullOrEmpty(videoPath))
                return true;

            AppLogger.LogInfo($"TorrentAutoDownloader: search VO torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(movie.Title, movie.Year);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VO torrents for {movie.Title} {movie.Year}");

            return await DownloadBestQuality(torrents);
        }

        private async Task<bool> DownloadFrenchVersion(LiteContentDto movie)
        {
            if (downloadCancellationTokenSource.IsCancellationRequested)
                return false;

            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(movie.Id);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = movie.Title;

            var videoPath = availableVideosListProvider.GetVfMovieSource(movie.Title, frenchTitle, movie.Year);
            if (!string.IsNullOrEmpty(videoPath))
                return true;

            AppLogger.LogInfo($"TorrentAutoDownloader: search VF torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitle, movie.Year);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VF torrents for {movie.Title} {movie.Year}");

            return await DownloadBestQuality(torrents);
        }

        private async Task<bool> DownloadBestQuality(IEnumerable<MediaTorrent> torrents)
        {
            if (torrents == null || !torrents.Any())
                return false;

            var sortedTorrentsByQuality = torrents.OrderBy(t => Array.IndexOf(qualitiesRank, t.Quality));

            foreach (var torrent in sortedTorrentsByQuality)
            {
                if (downloadCancellationTokenSource.IsCancellationRequested)
                    return false;

                AppLogger.LogInfo($"TorrentAutoDownloader: try to download {torrent.DownloadUrl} ({torrent.Quality})");

                var downloadSuccess = await torrentContentProvider.DownloadTorrentMediaAsync(torrent.DownloadUrl, TorrentAutoDownloaderIdentifier, new VideoTorrentFileSelector(), downloadCancellationTokenSource.Token);
                if (downloadSuccess)
                    return true;
                else
                    AppLogger.LogInfo($"TorrentAutoDownloader: download failed for {torrent.DownloadUrl}");
            }

            return false;
        }
    }
}
