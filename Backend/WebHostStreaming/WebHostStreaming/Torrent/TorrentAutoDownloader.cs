using System.Threading.Tasks;
using WebHostStreaming.Providers;
using MoviesAPI.Services.Content.Dtos;
using System.Linq;
using System;
using WebHostStreaming.Helpers;
using System.Collections.Generic;
using System.Threading;
using WebHostStreaming.Models;
using System.Collections.Concurrent;

namespace WebHostStreaming.Torrent
{
    public class TorrentAutoDownloader : ITorrentAutoDownloader
    {
        readonly IVideoInfoProvider videoInfoProvider;
        readonly ISearchersProvider searchersProvider;
        readonly ITorrentContentProvider torrentContentProvider;
        readonly IBookmarkedMoviesProvider bookmarkedMoviesProvider;
        Timer retryTimer;

        const string TorrentAutoDownloaderIdentifier = "Auto-Downloader";

        readonly Dictionary<int, string> qualitiesRank = new()
            {
                { 0, "1080p" },
                { 1, "720p" },
                { 2, "BDRIP" },
                { 3, "WEBRIP" },
                { 4, "DVDRIP" }
            };
        readonly TimeSpan timeSpanBeforeRetry = TimeSpan.FromHours(3);

        readonly ConcurrentDictionary<string, LiteContentDto> voMoviesToDownload = [];
        readonly ConcurrentDictionary<string, LiteContentDto> vfMoviesToDownload = [];

        bool started;
        SemaphoreSlim startSemaphore = new(1, 1);
        CancellationTokenSource downloadCancellationTokenSource;

        public TorrentAutoDownloader(
            IVideoInfoProvider videoInfoProvider,
            ISearchersProvider searchersProvider,
            ITorrentContentProvider torrentContentProvider,
            IBookmarkedMoviesProvider bookmarkedMoviesProvider)
        {
            this.videoInfoProvider = videoInfoProvider;
            this.searchersProvider = searchersProvider;
            this.torrentContentProvider = torrentContentProvider;
            this.bookmarkedMoviesProvider = bookmarkedMoviesProvider;
            this.bookmarkedMoviesProvider.MovieBookmarkAdded += async (s, movieBookmark) =>
            {
                if (started)
                    TryAddMovieToDownloadList(movieBookmark);
                else
                    await StartAsync();
            };
            this.bookmarkedMoviesProvider.MovieBookmarkDeleted += (s, movieId) =>
            {
                voMoviesToDownload.TryRemove(movieId, out _);
                vfMoviesToDownload.TryRemove(movieId, out _);
            };
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

            BuildMoviesToDownloadList();
            var success = await DownloadMoviesAsync();

            if (!success)
            {
                retryTimer = new Timer(async _ => await StartAsync(), null, timeSpanBeforeRetry, Timeout.InfiniteTimeSpan);
                AppLogger.LogInfo($"TorrentAutoDownloader.StartAsync(): movies still need to be downloaded, will retry in {timeSpanBeforeRetry.TotalHours} hours");
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

        private void BuildMoviesToDownloadList()
        {
            var bookmarkedMovies = bookmarkedMoviesProvider.GetBookmarkedMovies();
            voMoviesToDownload.Clear();
            vfMoviesToDownload.Clear();

            foreach (var movie in bookmarkedMovies)
                TryAddMovieToDownloadList(movie);
        }

        private void TryAddMovieToDownloadList(LiteContentDto movie)
        {
            var originalVersion = videoInfoProvider.GetMovieVideoInfo(movie.Id, LanguageVersion.Original);
            if (originalVersion == null && !voMoviesToDownload.ContainsKey(movie.Id))
            {
                voMoviesToDownload.TryAdd(movie.Id, movie);
                AppLogger.LogInfo($"TorrentAutoDownloader.TryAddMovieToDownloadList(): {movie.Title} {movie.Year} added to VO download list");
            }

            var frenchVersion = videoInfoProvider.GetMovieVideoInfo(movie.Id, LanguageVersion.French);
            if (frenchVersion == null && !vfMoviesToDownload.ContainsKey(movie.Id))
            {
                vfMoviesToDownload.TryAdd(movie.Id, movie);
                AppLogger.LogInfo($"TorrentAutoDownloader.TryAddMovieToDownloadList(): {movie.Title} {movie.Year} added to VF download list");
            }
        }

        private async Task<bool> DownloadMoviesAsync()
        {
            AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Starts");

            try
            {
                (int voFailed, int vfFailed) = (0, 0);

                downloadCancellationTokenSource = new CancellationTokenSource();

                while (!voMoviesToDownload.IsEmpty || !vfMoviesToDownload.IsEmpty)
                {
                    if (!voMoviesToDownload.IsEmpty)
                    {
                        if (voMoviesToDownload.TryGetValue(voMoviesToDownload.Keys.First(), out var movieVO))
                        {
                            var voDownloadSuccess = await DownloadOriginalVersionAsync(movieVO);

                            if (!voDownloadSuccess)
                                voFailed++;

                            voMoviesToDownload.TryRemove(movieVO.Id, out _);
                        }
                    }

                    if (vfMoviesToDownload.TryGetValue(vfMoviesToDownload.Keys.First(), out var movieVF))
                    {
                        var vfDownloadSuccess = await DownloadFrenchVersionAsync(movieVF);

                        if (!vfDownloadSuccess)
                            vfFailed++;

                        vfMoviesToDownload.TryRemove(movieVF.Id, out _);
                    }
                }

                var (voMessage, vfMessage) = (voFailed > 0 ? $"{voFailed} VO movies failed to download" : "", vfFailed > 0 ? $"{vfFailed} VF movies failed to download" : "");
                var logMessage = string.IsNullOrEmpty(voMessage) && string.IsNullOrEmpty(vfMessage)
                    ? "No movies left to download"
                    : $"{voMessage} {vfMessage}";
                AppLogger.LogInfo($"TorrentAutoDownloader.DownloadMoviesAsync(): {logMessage}");

                downloadCancellationTokenSource.Dispose();

                return voFailed == 0 && vfFailed == 0;
            }
            catch (TaskCanceledException)
            {
                AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Download cancelled");
            }
            catch (Exception ex)
            {
                AppLogger.LogError("TorrentAutoDownloader.DownloadMoviesAsync", ex);
            }
            finally
            {
                AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Ends");
            }

            return false;
        }

        private async Task<bool> DownloadOriginalVersionAsync(LiteContentDto movie)
        {
            AppLogger.LogInfo($"TorrentAutoDownloader: search VO torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(movie.Title, movie.Year);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VO torrents for {movie.Title} {movie.Year}");

            var torrentRequests = torrents.Select(t => new TorrentRequest(TorrentAutoDownloaderIdentifier, t.DownloadUrl, movie.Id, t.Quality, LanguageVersion.Original));

            var downloadSuccess = await DownloadBestQualityAsync(torrentRequests);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VO successfully downloaded for {movie.Title} {movie.Year}");
            else
                AppLogger.LogInfo($"TorrentAutoDownloader: failed to download VO for {movie.Title} {movie.Year}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadFrenchVersionAsync(LiteContentDto movie)
        {
            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(movie.Id);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = movie.Title;

            AppLogger.LogInfo($"TorrentAutoDownloader: search VF torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitle, movie.Year);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VF torrents for {movie.Title} {movie.Year}");

            var torrentRequests = torrents.Select(t => new TorrentRequest(TorrentAutoDownloaderIdentifier, t.DownloadUrl, movie.Id, t.Quality, LanguageVersion.French));

            var downloadSuccess = await DownloadBestQualityAsync(torrentRequests);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VF successfully downloaded for {movie.Title} {movie.Year}");
            else
                AppLogger.LogInfo($"TorrentAutoDownloader: failed to download VF for {movie.Title} {movie.Year}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadBestQualityAsync(IEnumerable<TorrentRequest> torrentRequests)
        {
            if (torrentRequests == null || !torrentRequests.Any())
                return false;

            var sortedRequestsByQuality = torrentRequests.Where(tr => qualitiesRank.Values.Contains(tr.VideoInfo.Quality, StringComparer.OrdinalIgnoreCase))
                                        .OrderBy(request => qualitiesRank.FirstOrDefault(q => string.Equals(q.Value, request.VideoInfo.Quality, StringComparison.OrdinalIgnoreCase)).Key);

            foreach (var torrentRequest in sortedRequestsByQuality)
            {
                if (downloadCancellationTokenSource.IsCancellationRequested)
                    return false;

                AppLogger.LogInfo($"TorrentAutoDownloader: try to download {torrentRequest.TorrentUrl} ({torrentRequest.VideoInfo.Quality})");

                var downloadSuccess = await torrentContentProvider.DownloadTorrentMediaAsync(torrentRequest, downloadCancellationTokenSource.Token);
                if (downloadSuccess)
                    return true;
                else
                    AppLogger.LogInfo($"TorrentAutoDownloader: download failed for {torrentRequest.TorrentUrl}");
            }

            return false;
        }
    }
}
