using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Providers.AvailableVideosListProvider;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers.TorrentContentProvider
{
    public class TorrentClientProvider : ITorrentClientProvider
    {
        private ClientEngine ClientEngine;
        private TorrentDownloader torrentDownloader;
        private Dictionary<string, Stream> torrentStreams;
        private Timer downloadWatcher;
        private IAvailableVideosListProvider availableVideosListProvider;

        public TorrentClientProvider(IAvailableVideosListProvider availableVideosListProvider)
        {
            ClientEngine = BuildClientEngine();
            torrentStreams = new Dictionary<string, Stream>();
            torrentDownloader = new TorrentDownloader();
            this.availableVideosListProvider = availableVideosListProvider;
        }

        private ClientEngine BuildClientEngine()
        {
            EngineSettingsBuilder settingsBuilder = new EngineSettingsBuilder();
            settingsBuilder.AllowedEncryption.Add(EncryptionType.PlainText | EncryptionType.RC4Full | EncryptionType.RC4Header);

            return new ClientEngine(settingsBuilder.ToSettings());
        }

        public async Task<Stream> GetTorrentStreamAsync(string torrentMetadaPath, ITorrentFileSelector torrentFileSelector)
        {
            var torrentManager = await GetTorrentManagerAsync(torrentMetadaPath);
            if (torrentManager == null)
                return null;

            var fileToDownload = await SelectFileToDownload(torrentManager, torrentFileSelector);
            if (fileToDownload == null)
                return null;

            if (torrentManager.State == TorrentState.Paused
            || torrentManager.State == TorrentState.Stopped
            || torrentManager.State == TorrentState.Error)
            {
                AppLogger.LogInfo($"Start TorrentManager : {torrentManager.Name}");

                await torrentManager.StartAsync();
            }

            if (!torrentStreams.ContainsKey(torrentManager.SavePath))
            {
                AppLogger.LogInfo($"Create stream for TorrentManager : {torrentManager.Name}");

                torrentStreams.Add(torrentManager.SavePath, null);
                var stream = await torrentManager.StreamProvider.CreateStreamAsync(fileToDownload, true);
                if (torrentStreams.ContainsKey(torrentManager.SavePath) && stream != null)
                {
                    AppLogger.LogInfo($"Stream retrieved for TorrentManager : {torrentManager.Name}");

                    torrentStreams[torrentManager.SavePath] = stream;
                    return stream;
                }
                else
                {
                    AppLogger.LogInfo($"Stream creation failed for TorrentManager : {torrentManager.Name}");

                    torrentStreams.Remove(torrentManager.SavePath);

                    return null;
                }
            }
            else if (torrentStreams[torrentManager.SavePath] == null) //it means the torrent creation is ongoing
            {
                AppLogger.LogInfo($"Waiting stream creation for TorrentManager : {torrentManager.Name}");

                while (torrentStreams.ContainsKey(torrentManager.SavePath) && torrentStreams[torrentManager.SavePath] == null)
                {
                    await Task.Delay(1000);
                }

                if (torrentStreams.ContainsKey(torrentManager.SavePath))
                {
                    AppLogger.LogInfo($"Stream retrieved for TorrentManager : {torrentManager.Name}");
                    return torrentStreams[torrentManager.SavePath];
                }
                else
                {
                    AppLogger.LogInfo($"Stream creation aborted for TorrentManager : {torrentManager.Name}");
                    return null;
                }
            }
            else
            {
                try
                {
                    torrentStreams[torrentManager.SavePath].Position = 0;
                }
                catch (ObjectDisposedException)
                {
                    torrentStreams[torrentManager.SavePath] = null;
                    torrentStreams[torrentManager.SavePath] = await torrentManager.StreamProvider.CreateStreamAsync(fileToDownload, true);
                }

                AppLogger.LogInfo($"Stream retrieved for TorrentManager : {torrentManager.Name}");

                return torrentStreams[torrentManager.SavePath];
            }
        }

        public async Task<TorrentManager> GetTorrentManagerAsync(string torrentMetadaPath)
        {
            try
            {
                var torrentFilePath = await torrentDownloader.DownloadTorrentFileAsync(torrentMetadaPath, ClientEngine);

                if (string.IsNullOrEmpty(torrentFilePath))
                    return null;

                var torrentDirectory = Path.GetDirectoryName(torrentFilePath);

                var torrentManager = ClientEngine.Torrents.SingleOrDefault(t => t.SavePath == torrentDirectory);

                if (torrentManager == null)
                {
                    AppLogger.LogInfo($"Create TorrentManager : {torrentMetadaPath}");

                    torrentManager = await CreateTorrentManagerAsync(torrentFilePath, torrentDirectory);
                }
                else if (torrentManager != null && torrentManager.State == TorrentState.Stopping)
                {
                    AppLogger.LogInfo($"Wait before re creating TorrentManager : {torrentMetadaPath}");

                    //wait for the torrent to be removed
                    while (ClientEngine.Torrents.Any(t => t.SavePath == torrentDirectory))
                    {
                        await Task.Delay(1000);
                    }

                    AppLogger.LogInfo($"Create TorrentManager : {torrentMetadaPath}");

                    torrentManager = await CreateTorrentManagerAsync(torrentFilePath, torrentDirectory);
                }

                return torrentManager;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<TorrentManager> CreateTorrentManagerAsync(string torrentFilePath, string torrentDirectory)
        {
            var torrentManager = await ClientEngine.AddStreamingAsync(torrentFilePath, torrentDirectory);
            ReleaseUnusedTorrentManagersAsync(torrentDirectory);

            InitDoawnloadWatcher();

            return torrentManager;
        }

        private void InitDoawnloadWatcher()
        {
            if (downloadWatcher == null)
            {
                AppLogger.LogInfo($"Init Download Watcher");

                downloadWatcher = new Timer(async (e) =>
                {
                    var torrents = ClientEngine.Torrents.Where(t => t.State != TorrentState.Stopped && t.State != TorrentState.Stopping);

                    if (torrents.Any())
                    {
                        foreach (var torrent in torrents)
                        {
                            AppLogger.LogInfo($"Download progress for {torrent.SavePath}: {torrent.PartialProgress}");
                            if(torrent.PartialProgress > 97)
                            {
                                var file = torrent.Files.SingleOrDefault(f => f.Priority == Priority.Highest);
                                if (file != null)
                                {
                                    var success = await availableVideosListProvider.AddMediaSource(file.FullPath);

                                    if(success)
                                        AppLogger.LogInfo($"Register file as complete : {torrent.Name}");
                                }
                            }
                        }
                    }
                    else
                    {
                        AppLogger.LogInfo($"Stop Download Watcher");

                        downloadWatcher.Change(Timeout.Infinite, Timeout.Infinite);
                        downloadWatcher.Dispose();
                        downloadWatcher = null;
                    }

                }, null, 30000, 30000);
            }
        }

        private async Task<ITorrentManagerFile> SelectFileToDownload(TorrentManager torrentManager, ITorrentFileSelector torrentFileSelector)
        {
            var fileToDownload = torrentFileSelector.SelectTorrentFileInfo(torrentManager.Files);

            var currentDownloadingFile = torrentManager.Files.SingleOrDefault(f => f.Priority == Priority.Highest);

            if (fileToDownload != null && currentDownloadingFile?.FullPath != fileToDownload.FullPath)
            {
                foreach (var file in torrentManager.Files.Where(f => fileToDownload.FullPath != f.FullPath))
                    await torrentManager.SetFilePriorityAsync(file, Priority.DoNotDownload);

                await torrentManager.SetFilePriorityAsync(fileToDownload, Priority.Highest);

                AppLogger.LogInfo($"File to download selected : {fileToDownload.Path}");
            }

            return fileToDownload;
        }

        private void ReleaseUnusedTorrentManagersAsync(string usedTorrentDirectory)
        {
            var tasks = new List<Task>();

            var torrentsToRelease = ClientEngine.Torrents.Where(t => t.SavePath != usedTorrentDirectory);
            foreach (var torrentManager in torrentsToRelease)
            {
                AppLogger.LogInfo($"Stop TorrentManager : {torrentManager.Name}");

                tasks.Add(torrentManager.StopAsync());
            }

            Task.WhenAll(tasks).ContinueWith(async t =>
            {
                foreach (var torrent in torrentsToRelease)
                {
                    AppLogger.LogInfo($"Remove TorrentManager : {torrent.Name}");

                    await ClientEngine.RemoveAsync(torrent, RemoveMode.KeepAllData);
                    torrentStreams.Remove(torrent.SavePath);
                }
            });
        }

        public DownloadingState GetStreamDownloadingState(string torrentUri)
        {
            var torrentDirectory = torrentUri.ToTorrentFolderPath();

            var torrentManager = ClientEngine.Torrents.SingleOrDefault(t => t.SavePath == torrentDirectory);

            if(torrentManager != null)
            {
                if(torrentManager.PartialProgress > 0.5)
                    return new DownloadingState("Download in progress, ready to play soon (6/6)");
                else if(torrentManager.PartialProgress > 0)
                    return new DownloadingState("Download has started (5/6)");
                else if (torrentManager.Peers.Available > 0)
                    return new DownloadingState("Download is about to start (4/6)");
                else if (!torrentManager.Files.Any(f => f.Priority == Priority.Highest))
                    return new DownloadingState("No supported video file found", true);
                else
                    return new DownloadingState("Download initialization (3/6)");
            }
            else if (torrentDownloader.TorrentDownloadDirectory == torrentDirectory)
            {
                if (torrentDownloader.Status == TorrentDownloaderStatus.DownloadCompleted)
                    return new DownloadingState("Resources found (2/6)");
                else if (torrentDownloader.Status == TorrentDownloaderStatus.DownloadHasStarted)
                    return new DownloadingState("Searching resources (1/6)");
                else if (torrentDownloader.Status == TorrentDownloaderStatus.DownloadFailed)
                    return new DownloadingState("Downloading resources failed", true);
                else if (torrentDownloader.Status == TorrentDownloaderStatus.DownloadAborted)
                    return new DownloadingState("Downloading resources aborted", true);
                else
                    return new DownloadingState("Loading");
            }
            else
            {
                return new DownloadingState("Unable to get downloading status", true);
            }
        }

        public string GetContentType(string torrentUri)
        {
            var torrentDirectory = torrentUri.ToTorrentFolderPath();

            var torrentManager = ClientEngine.Torrents.SingleOrDefault(t => t.SavePath == torrentDirectory);

            if (torrentManager == null)
                return string.Empty;

            var file = torrentManager.Files.SingleOrDefault(f => f.Priority == Priority.Highest);

            if (file == null)
                return null;

            return file.FullPath.GetContentType();
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
