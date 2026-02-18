using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public class TorrentContentProvider : ITorrentContentProvider
    {
        private List<TorrentClient> torrentClients;
        private Dictionary<string, DateTime> lastAccessToTorrentClient;
        private Timer torrentClientsWatcher;
        private IVideoInfoProvider videoInfoProvider;
        private IWatchedMoviesProvider watchedMoviesProvider;
        private IWatchedSeriesProvider watchedSeriesProvider;

        public event EventHandler OnNoActiveTorrentClient;

        public TorrentContentProvider(
            IVideoInfoProvider videoInfoProvider,
            IWatchedMoviesProvider watchedMoviesProvider,
            IWatchedSeriesProvider watchedSeriesProvider)
        {
            torrentClients = new List<TorrentClient>();
            lastAccessToTorrentClient = new Dictionary<string, DateTime>();
            this.videoInfoProvider = videoInfoProvider;
            this.watchedMoviesProvider = watchedMoviesProvider;
            this.watchedSeriesProvider = watchedSeriesProvider;
        }

        public async Task<TorrentStream> GetTorrentStreamAsync(TorrentRequest torrentRequest)
        {
            var torrentClient = GetOrCreateTorrentClient(torrentRequest.ClientAppId);
            return await torrentClient.GetTorrentStreamAsync(torrentRequest);
        }

        public async Task<bool> DownloadTorrentMediaAsync(TorrentRequest torrentRequest, CancellationToken cancellationToken)
        {
            AppLogger.LogInfo(torrentRequest.ClientAppId, $"Start download media from {torrentRequest.TorrentUrl}");

            var torrentClient = GetOrCreateTorrentClient(torrentRequest.ClientAppId);
            cancellationToken.Register(() =>
            {
                torrentClient.Dispose();
            });

            var downloadStarted = await torrentClient.StartDownloadTorrentMediaAsync(torrentRequest);

            if (!downloadStarted)
            {
                AppLogger.LogInfo(torrentRequest.ClientAppId, $"Start download media from {torrentRequest.TorrentUrl} FAILED");
                return false;
            }

            AppLogger.LogInfo(torrentRequest.ClientAppId, $"Start download media from {torrentRequest.TorrentUrl} SUCCESS");

            DateTime lastDownloadProgressDateTime;
            try
            {
                do
                {
                    lastDownloadProgressDateTime = torrentClient.LastDownloadProgressDateTime;
                    AppLogger.LogInfo(torrentRequest.ClientAppId, $"Waiting downloading progresss for {torrentRequest.TorrentUrl}");

                    await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);//wait for the download to progress during 3 minutes
                }
                while (!torrentClient.IsDownloadComplete && lastDownloadProgressDateTime < torrentClient.LastDownloadProgressDateTime);

                return torrentClient.IsDownloadComplete;
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                    AppLogger.LogInfo(torrentRequest.ClientAppId, $"Download aborted for {torrentRequest.TorrentUrl}");

                return false;
            }
        }

        public async Task<DownloadingState> GetDownloadingStateAsync(string clientAppIdentifier, string torrentUrl)
        {
            var torrentClient = torrentClients.FirstOrDefault(torrentClient => torrentClient.ClientAppIdentifier == clientAppIdentifier);
            if (torrentClient == null)
                return null;

            return await torrentClient.GetDownloadingStateAync(torrentUrl);
        }

        private TorrentClient GetOrCreateTorrentClient(string clientAppIdentifier)
        {
            var torrentClient = torrentClients.FirstOrDefault(torrentClient => torrentClient.ClientAppIdentifier == clientAppIdentifier);
            if (torrentClient == null)
                torrentClient = CreateTorrentClient(clientAppIdentifier);

            if (lastAccessToTorrentClient.ContainsKey(clientAppIdentifier))
                lastAccessToTorrentClient[clientAppIdentifier] = DateTime.Now;
            else
            {
                lastAccessToTorrentClient.Add(clientAppIdentifier, DateTime.Now);
                StartTorrentClientsWatcherIfNeeded();
            }

            return torrentClient;
        }

        private TorrentClient CreateTorrentClient(string clientAppIdentifier)
        {
            AppLogger.LogInfo(clientAppIdentifier, $"Create TorrentClient");

            TorrentClient torrentClient = new TorrentClient(clientAppIdentifier);
            torrentClient.OnVideoDownloadCompleted += (s, videoInfo) =>
            {
                try
                {
                    videoInfoProvider.AddVideoInfo(videoInfo);
                    AppLogger.LogInfo($"Register file as complete with success : {videoInfo.FilePath}");
                }
                catch (Exception ex)
                {
                    AppLogger.LogError(clientAppIdentifier, $"Register file as complete : {videoInfo.FilePath}", ex);
                }
            };
            torrentClients.Add(torrentClient);
            return torrentClient;
        }

        private void StartTorrentClientsWatcherIfNeeded()
        {
            var inactivePeriodInMinutes = 30;
            if (torrentClientsWatcher == null)
            {
                AppLogger.LogInfo("TorrentClientProvider: Start torrent clients watcher");
                torrentClientsWatcher = new Timer((state) =>
                {
                    var now = DateTime.Now;
                    AppLogger.LogInfo("TorrentClientProvider: Check torrent clients to dispose");

                    if (torrentClients.Count == 0)
                    {
                        AppLogger.LogInfo("TorrentClientProvider: Stop torrent clients watcher");
                        torrentClientsWatcher.Change(Timeout.Infinite, Timeout.Infinite);
                        torrentClientsWatcher.Dispose();
                        torrentClientsWatcher = null;
                        OnNoActiveTorrentClient?.Invoke(this, EventArgs.Empty);
                    }
                    //if only one torrent client and it's active, do not dispose it
                    else if (torrentClients.Count == 1 && torrentClients[0].LastDownloadProgressDateTime > now.AddMinutes(-5))
                        return;

                    var clientAppIdsToRemove = new List<string>();

                    foreach (var torrentClient in torrentClients)
                    {
                        if (lastAccessToTorrentClient.TryGetValue(torrentClient.ClientAppIdentifier, out DateTime lastAccess))
                        {
                            if ((now - lastAccess).TotalMinutes >= inactivePeriodInMinutes)
                            {
                                torrentClient.Dispose();
                                clientAppIdsToRemove.Add(torrentClient.ClientAppIdentifier);
                            }
                        }
                    }

                    foreach (var clientAppId in clientAppIdsToRemove)
                    {
                        var torrentClient = torrentClients.FirstOrDefault(torrentClient => torrentClient.ClientAppIdentifier == clientAppId);
                        torrentClients.Remove(torrentClient);
                        lastAccessToTorrentClient.Remove(clientAppId);
                        torrentClient = null;
                        CleanUnusedResources(clientAppId);
                        AppLogger.LogInfo($"TorrentClientProvider: Torrent client disposed for {clientAppId}");
                    }
                }, null, TimeSpan.FromMinutes(inactivePeriodInMinutes), TimeSpan.FromMinutes(inactivePeriodInMinutes));
            }
        }

        private void CleanUnusedResources(string clientAppIdentifier)
        {
            var torrentFolderContainer = Path.Combine(AppFolders.TorrentsFolder, clientAppIdentifier.ToMD5Hash());

            if (!Directory.Exists(torrentFolderContainer))
                return;

            var watchedMovies = watchedMoviesProvider.GetWatchedMovies();
            var watchedSeries = watchedSeriesProvider.GetWatchedSeries();
            var videoExtensions = new string[] { ".mp4", ".avi", ".mkv" };

            foreach (var torrentFolder in Directory.GetDirectories(torrentFolderContainer))
            {
                var shouldDeleteFolder = true;
                var torrentUrlMd5Hash = Path.GetFileName(torrentFolder);

                var videoFiles = Directory.GetFiles(torrentFolder, "*.*", SearchOption.AllDirectories).Where(f => videoExtensions.Contains(Path.GetExtension(f)));

                foreach (var videofilePath in videoFiles)
                {
                    if (videoInfoProvider.AllVideosInfos.Any(v => v.FilePath == videofilePath)
                        || (watchedMovies != null && watchedMovies.Any(m => m.VideoSource.ToMD5Hash() == torrentUrlMd5Hash))
                        || (watchedSeries != null && watchedSeries.Any(s => s.VideoSource.ToMD5Hash() == torrentUrlMd5Hash)))
                    {
                        shouldDeleteFolder = false;
                    }
                }

                if (shouldDeleteFolder)
                {
                    try
                    {
                        Directory.Delete(torrentFolder, true);

                        AppLogger.LogInfo($"Deleted folder {torrentFolder}");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogInfo($"Error occured trying to delete folder {torrentFolder}: {ex.Message}");
                    }
                }
            }
        }
    }
}
