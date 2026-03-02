using System.Threading.Tasks;
using WebHostStreaming.Providers;
using System;
using WebHostStreaming.Helpers;
using System.Threading;
using System.Collections.Generic;

namespace WebHostStreaming.Torrent
{
    public class TorrentAutoDownloader : ITorrentAutoDownloader
    {
        Timer retryTimer;
        private readonly TorrentMovieDownloader torrentMovieDownloader;
        private readonly TorrentEpisodeDownloader torrentEpisodeDownloader;
        const string TorrentAutoDownloaderIdentifier = "Auto-Downloader";

        readonly TimeSpan timeSpanBeforeRetry = TimeSpan.FromHours(3);

        bool started;
        readonly SemaphoreSlim startSemaphore = new(1, 1);
        CancellationTokenSource downloadCancellationTokenSource;

        public TorrentAutoDownloader(
            IVideoInfoProvider videoInfoProvider,
            ISearchersProvider searchersProvider,
            ITorrentContentProvider torrentContentProvider,
            IBookmarkedMoviesProvider bookmarkedMoviesProvider,
            IBookmarkedSeriesProvider bookmarkedSeriesProvider,
            IWatchedSeriesProvider watchedSeriesProvider)
        {
            torrentMovieDownloader = new TorrentMovieDownloader(torrentContentProvider, videoInfoProvider, searchersProvider, bookmarkedMoviesProvider);
            torrentEpisodeDownloader = new TorrentEpisodeDownloader(torrentContentProvider, videoInfoProvider, searchersProvider, watchedSeriesProvider, bookmarkedSeriesProvider);
            torrentMovieDownloader.MediaAddedToDownloadList += OnDownloadListUpdated;
            torrentEpisodeDownloader.MediaAddedToDownloadList += OnDownloadListUpdated;
        }

        public async Task StartAsync()
        {
            await startSemaphore.WaitAsync();

            if (started)
            {
                AppLogger.LogInfo("TorrentAutoDownloader.StartAsync(): Already running, skip");
                return;
            }

            AppLogger.LogInfo("TorrentAutoDownloader.StartAsync(): Starts");

            retryTimer?.Dispose();

            started = true;

            startSemaphore.Release();

            downloadCancellationTokenSource = new CancellationTokenSource();

            var movieSuccess = await torrentMovieDownloader.DownloadAsync(TorrentAutoDownloaderIdentifier, downloadCancellationTokenSource.Token);
            var episodeSuccess = false;
            if (downloadCancellationTokenSource?.Token.IsCancellationRequested == false)
                episodeSuccess = await torrentEpisodeDownloader.DownloadAsync(TorrentAutoDownloaderIdentifier, downloadCancellationTokenSource.Token);

            if (!movieSuccess || !episodeSuccess || torrentMovieDownloader.HasMediasToDownload || torrentEpisodeDownloader.HasMediasToDownload)
            {
                retryTimer = new Timer(async _ => await StartAsync(), null, timeSpanBeforeRetry, Timeout.InfiniteTimeSpan);
                var typesToDownloadList = new List<string>();
                if (torrentMovieDownloader.HasMediasToDownload)
                    typesToDownloadList.Add("movies");
                if (torrentEpisodeDownloader.HasMediasToDownload)
                    typesToDownloadList.Add("episodes");

                var typesToDownload = string.Join(" and ", typesToDownloadList);
                AppLogger.LogInfo($"TorrentAutoDownloader.StartAsync(): {typesToDownload} still need to be downloaded, will retry in {timeSpanBeforeRetry.TotalHours} hours");
            }

            started = false;
        }

        public void RetryLater()
        {
            if (downloadCancellationTokenSource != null)
            {
                downloadCancellationTokenSource.Cancel();
                downloadCancellationTokenSource.Dispose();
                downloadCancellationTokenSource = null;
            }

            AppLogger.LogInfo("TorrentAutoDownloader.RetryLater()");
        }

        private void OnDownloadListUpdated(object sender, EventArgs e)
        {
            if (!started)
                StartAsync();
        }
    }
}
