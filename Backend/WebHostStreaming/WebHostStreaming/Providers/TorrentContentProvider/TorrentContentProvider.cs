using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Providers.AvailableVideosListProvider;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers.TorrentContentProvider
{
    public class TorrentContentProvider : ITorrentContentProvider
    {
        private List<TorrentClient> torrentClients;
        private Dictionary<string, DateTime> lastAccessToTorrentClient;
        private Timer torrentClientsWatcher;
        private IAvailableVideosListProvider availableVideosListProvider;
        public event EventHandler OnNoActiveTorrentClient;

        public TorrentContentProvider(IAvailableVideosListProvider availableVideosListProvider)
        {
            torrentClients = new List<TorrentClient>();
            lastAccessToTorrentClient = new Dictionary<string, DateTime>();
            this.availableVideosListProvider = availableVideosListProvider;
        }

        public async Task<TorrentStream> GetTorrentStreamAsync(string clientAppIdentifier, string torrentUri, ITorrentFileSelector torrentFileSelector)
        {
            var torrentClient = GetOrCreateTorrentClient(clientAppIdentifier);
            return await torrentClient.GetTorrentStreamAsync(torrentUri, torrentFileSelector);
        }

        public async Task<bool> DownloadTorrentMediaAsync(string torrentUri, string clientAppIdentifier, ITorrentFileSelector torrentFileSelector, CancellationToken cancellationToken)
        {
            AppLogger.LogInfo(clientAppIdentifier, $"Start download media from {torrentUri}");

            var torrentClient = GetOrCreateTorrentClient(clientAppIdentifier);
            cancellationToken.Register(() =>
            {
                torrentClient.Dispose();
            });

            var downloadStarted = await torrentClient.StartDownloadTorrentMediaAsync(torrentUri, torrentFileSelector);

            if (!downloadStarted)
            {
                AppLogger.LogInfo(clientAppIdentifier, $"Start download media from {torrentUri} FAILED");
                return false;
            }

            AppLogger.LogInfo(clientAppIdentifier, $"Start download media from {torrentUri} SUCCESS");

            double? progress = null;
            double lastProgress = 0;
            try
            {
                do
                {
                    lastProgress = progress.GetValueOrDefault(0);

                    AppLogger.LogInfo(clientAppIdentifier, $"Waiting next progress for {torrentUri}");

                    await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);//wait for the download to progress during 3 minutes
                    torrentClient = GetOrCreateTorrentClient(clientAppIdentifier);//recall GetOrCreateTorrentClient to update last access datetime
                    progress = await torrentClient.GetDownloadingProgressAync(torrentUri);
                }
                while (progress.HasValue && progress.Value > lastProgress && progress.Value < 100);

                return progress.HasValue && progress.Value >= 100;
            }
            catch (Exception ex)
            {
                if(cancellationToken.IsCancellationRequested)
                    AppLogger.LogInfo(clientAppIdentifier, $"Download aborted for {torrentUri}");

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
            torrentClient.OnFileDownloadComplete += async (s, fileFullPath) =>
            {
                var success = await availableVideosListProvider.AddMediaSource(fileFullPath);

                if (success.HasValue)
                {
                    if (success.Value)
                        AppLogger.LogInfo($"Register file as complete with success : {fileFullPath}");
                    else
                        AppLogger.LogInfo($"Fail to register file as complete : {fileFullPath}");
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

                    if (!torrentClients.Any())
                    {
                        AppLogger.LogInfo("TorrentClientProvider: Stop torrent clients watcher");
                        torrentClientsWatcher.Change(Timeout.Infinite, Timeout.Infinite);
                        torrentClientsWatcher.Dispose();
                        torrentClientsWatcher = null;
                        OnNoActiveTorrentClient?.Invoke(this, EventArgs.Empty);
                    }

                    var clientAppIdsToRemove = new List<string>();

                    foreach (var torrentClient in torrentClients)
                    {
                        if (lastAccessToTorrentClient.ContainsKey(torrentClient.ClientAppIdentifier))
                        {
                            var lastAccess = lastAccessToTorrentClient[torrentClient.ClientAppIdentifier];

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
                        AppLogger.LogInfo($"TorrentClientProvider: Torrent client disposed for {clientAppId}");
                    }
                }, null, TimeSpan.FromMinutes(inactivePeriodInMinutes), TimeSpan.FromMinutes(inactivePeriodInMinutes));
            }
        }

        public Task<IEnumerable<string>> GetTorrentFilesAsync(string torrentUri)
        {
            throw new NotImplementedException();

            ////var torrentDownloader = new TorrentDownloader(torrentUri);

            ////await torrentDownloader.DownloadTorrentFileAsync(cancellationTokenSource.Token);

            //if (torrentDownloader.Status != TorrentDownloaderStatus.DownloadCompleted)
            //    return null;

            //var c = new CancellationTokenSource();


            //var torrentManager = await TorrentClientEngine.Instance.GetTorrentManagerAsync(torrentDownloader.TorrentFilePath, null, c.Token);
            //var files = torrentManager?.Files.Select(f => f.Path);

            //await torrentManager.StopAsync();
            //await torrentManager.Engine.RemoveAsync(torrentManager);

            //return torrentManager?.Files.Select(f => f.Path);
        }

      
    }
}
