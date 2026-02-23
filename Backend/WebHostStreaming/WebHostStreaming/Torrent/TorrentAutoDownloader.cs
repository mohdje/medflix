using System.Threading.Tasks;
using WebHostStreaming.Providers;
using MoviesAPI.Services.Content.Dtos;
using System.Linq;
using System;
using WebHostStreaming.Helpers;
using System.Collections.Generic;
using System.Threading;
using WebHostStreaming.Models;

namespace WebHostStreaming.Torrent
{
    public class TorrentAutoDownloader : ITorrentAutoDownloader
    {
        readonly IVideoInfoProvider videoInfoProvider;
        readonly ISearchersProvider searchersProvider;
        readonly ITorrentContentProvider torrentContentProvider;
        readonly IBookmarkedMoviesProvider bookmarkedMoviesProvider;
        readonly Timer retryTimer;


        readonly string[] qualitiesRank = ["1080p", "720p", "BDRIP", "WEBRIP", "DVDRIP"];
        const string TorrentAutoDownloaderIdentifier = "Auto-Downloader";
        readonly TimeSpan timeSpanBeforeRetry = TimeSpan.FromSeconds(40);

        readonly List<LiteContentDto> voMoviesToDownload = [];
        readonly List<LiteContentDto> vfMoviesToDownload = [];


        bool started;
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
            this.bookmarkedMoviesProvider.MovieBookmarkAdded += async (s, e) =>
            {
                if (started)
                    UpdateMoviesToDownloadList();
                else
                    await StartAsync();
            };
            this.bookmarkedMoviesProvider.MovieBookmarkDeleted += (s, e) =>
            {
                if (started)
                    UpdateMoviesToDownloadList();
            };
            retryTimer = new Timer(async _ => await StartAsync(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public async Task StartAsync()
        {
            if (started)
            {
                AppLogger.LogInfo("TorrentAutoDownloader.StartAsync(): Already running, skip");
                return;
            }

            AppLogger.LogInfo("TorrentAutoDownloader.StartAsync(): Starts");

            retryTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            started = true;
            var success = await DownloadMediasAsync();

            if (!success)
            {
                retryTimer.Change(timeSpanBeforeRetry, Timeout.InfiniteTimeSpan);
                AppLogger.LogInfo($"TorrentAutoDownloader.StartAsync(): movies still need to be downloaded, will retry in {timeSpanBeforeRetry.TotalHours} hours");
            }

            started = false;
        }

        public void Stop()
        {
            downloadCancellationTokenSource?.Cancel();
            AppLogger.LogInfo("TorrentAutoDownloader.Stop(): Stopped");
        }

        private async Task<bool> DownloadMediasAsync()
        {
            UpdateMoviesToDownloadList();
            return await DownloadMoviesAsync();
        }

        private void UpdateMoviesToDownloadList()
        {
            var bookmarkedMovies = bookmarkedMoviesProvider.GetBookmarkedMovies();

            lock (voMoviesToDownload)
            {
                voMoviesToDownload.RemoveAll(m => !bookmarkedMovies.Any(bm => bm.Id == m.Id));
                foreach (var movie in bookmarkedMovies)
                {
                    var orignalVersion = videoInfoProvider.GetMovieVideoInfo(movie.Id, LanguageVersion.Original);
                    if (orignalVersion == null)
                    {
                        if (!voMoviesToDownload.Any(m => m.Id == movie.Id))
                        {
                            voMoviesToDownload.Add(movie);
                            AppLogger.LogInfo($"TorrentAutoDownloader.UpdateMoviesToDownloadList(): {movie.Title} {movie.Year} added to VO download list");
                        }
                    }
                }
            }

            lock (vfMoviesToDownload)
            {
                vfMoviesToDownload.RemoveAll(m => !bookmarkedMovies.Any(bm => bm.Id == m.Id));
                foreach (var movie in bookmarkedMovies)
                {
                    var frenchVersion = videoInfoProvider.GetMovieVideoInfo(movie.Id, LanguageVersion.French);

                    if (frenchVersion == null)
                    {
                        if (!vfMoviesToDownload.Any(m => m.Id == movie.Id))
                        {
                            vfMoviesToDownload.Add(movie);
                            AppLogger.LogInfo($"TorrentAutoDownloader.UpdateMoviesToDownloadList(): {movie.Title} {movie.Year} added to VF download list");
                        }
                    }
                }
            }
        }

        private async Task<bool> DownloadMoviesAsync()
        {
            AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Starts");

            try
            {
                (int voFailed, int vfFailed) = (0, 0);

                downloadCancellationTokenSource = new CancellationTokenSource();

                while (voMoviesToDownload.Count > 0 || vfMoviesToDownload.Count > 0)
                {
                    if (voMoviesToDownload.Count > 0)
                    {
                        var movie = voMoviesToDownload[0];
                        var voDownloadSuccess = await DownloadOriginalVersionAsync(movie);

                        if (!voDownloadSuccess)
                            voFailed++;

                        lock (voMoviesToDownload)
                        {
                            voMoviesToDownload.Remove(movie);
                        }
                    }

                    if (vfMoviesToDownload.Count > 0)
                    {
                        var movie = vfMoviesToDownload[0];
                        var vfDownloadSuccess = await DownloadFrenchVersionAsync(movie);

                        if (!vfDownloadSuccess)
                            vfFailed++;

                        lock (vfMoviesToDownload)
                        {
                            vfMoviesToDownload.Remove(movie);
                        }
                    }
                }

                var (voMessage, vfMessage) = (voFailed > 0 ? $"{voFailed} VO movies failed to download" : "", vfFailed > 0 ? $"{vfFailed} VF movies failed to download" : "");
                var logMessage = string.IsNullOrEmpty(voMessage) && string.IsNullOrEmpty(vfMessage)
                    ? "No movies left to download"
                    : $"{voMessage} {vfMessage}";
                AppLogger.LogInfo($"TorrentAutoDownloader.DownloadMoviesAsync(): {logMessage}");

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

            var sortedRequestsByQuality = torrentRequests.Where(tr => qualitiesRank.Contains(tr.VideoInfo.Quality, StringComparer.OrdinalIgnoreCase))
                                        .OrderBy(request => Array.IndexOf(qualitiesRank.Select(q => q.ToLower()).ToArray(), request.VideoInfo.Quality.ToLower()));

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
