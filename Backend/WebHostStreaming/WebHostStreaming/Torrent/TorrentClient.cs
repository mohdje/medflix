using MonoTorrent.Client;
using System.IO;
using System.Threading.Tasks;
using System;
using WebHostStreaming.Helpers;
using MonoTorrent;
using System.Linq;
using System.Threading;
using WebHostStreaming.Models;
using MonoTorrent.Connections;

namespace WebHostStreaming.Torrent
{
    public class TorrentClient : IDisposable
    {
        public string ClientAppIdentifier { get; }
        public event EventHandler<string> OnFileDownloadComplete;

        private ClientEngine clientEngine;
        private TorrentManager currentTorrentManager;
        private TorrentFileDownloader torrentFileDownloader;
        private string currentTorrentUrl;
        private ITorrentManagerFile currentDownloadingFile;
        private TorrentStream currentTorrentStream;
        private DownloadingState currentDownloadingState;

        private CancellationTokenSource torrentStreamCancellationTokenSource;
        private CancellationTokenSource downloadTorrentCancellationTokenSource;

        public TorrentClient(string clientAppIdentifier)
        {
            this.clientEngine = BuildClientEngine();
            ClientAppIdentifier = clientAppIdentifier;
        }

        public void Dispose()
        {
            ReleaseCurrentStream();
            ReleaseTorrentManager();
            clientEngine.Dispose();

            currentTorrentManager = null;
            torrentFileDownloader = null;
        }

        public async Task<TorrentStream> GetTorrentStreamAsync(string torrentUrl, ITorrentFileSelector torrentFileSelector)
        {
            if (currentTorrentUrl != torrentUrl)
            {
                currentTorrentUrl = torrentUrl;
                currentDownloadingState = DownloadingState.Loading;

                ReleaseTorrentManager();

                var torrentManager = await CreateTorrentManagerAsync(torrentUrl);
                if (torrentManager != null)
                {
                    currentTorrentManager = torrentManager;
                    currentTorrentManager.PeersFound += OnPeersFound;
                    currentTorrentManager.PieceHashed += OnPieceHashed;
                }
                else
                    return null;
            }

            if (FileToDownloadChanged(torrentFileSelector))
            {
                ReleaseCurrentStream();

                await SetFileToDownload(torrentFileSelector);
            }

            if (currentDownloadingFile == null)
            {
                currentDownloadingState = DownloadingState.NoMediaFileFoundInTorrent;
                AppLogger.LogInfo(ClientAppIdentifier, $"No video file found to download for TorrentManager : {currentTorrentManager.Name}");
                return null;
            }


            await StartTorrentManagerIfNeededAsync();

            try
            {
                if (currentTorrentStream == null)
                {
                    AppLogger.LogInfo(ClientAppIdentifier, $"Create stream for TorrentManager : {currentTorrentManager.Name}");

                    currentTorrentStream = new TorrentStream(currentTorrentManager, currentDownloadingFile);
                    torrentStreamCancellationTokenSource = new CancellationTokenSource();
                    await currentTorrentStream.StartAsync(torrentStreamCancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                if (torrentStreamCancellationTokenSource.IsCancellationRequested)
                    AppLogger.LogInfo(ClientAppIdentifier, $"Stream creation aborted for TorrentManager : {currentTorrentManager.Name}");
                else
                {
                    AppLogger.LogInfo(ClientAppIdentifier, $"Stream creation failed for TorrentManager : {currentTorrentManager.Name}");
                    AppLogger.LogError(ClientAppIdentifier, "GetTorrentStreamAsync", ex);

                    currentTorrentManager.PeersFound -= OnPeersFound;
                    currentTorrentManager.PieceHashed -= OnPieceHashed;
                    currentDownloadingState = DownloadingState.TorrentFileOpeningFailed;
                }

                return null;
            }

            return currentTorrentStream;
        }

        public async Task<DownloadingState> GetDownloadingStateAync(string torrentUrl) 
        {
            var tryCounter = 0;
            while (currentTorrentUrl != torrentUrl && tryCounter < 3)
            {
                await Task.Delay(2000);
                tryCounter++;
            }

            return currentTorrentUrl == torrentUrl ? currentDownloadingState : DownloadingState.NotFound;
        }

        private void OnPieceHashed(object sender, PieceHashedEventArgs e)
        {
            if (currentDownloadingFile != null)
            {
                if (currentTorrentManager.PartialProgress >= 97)
                    OnFileDownloadComplete?.Invoke(this, currentDownloadingFile.FullPath);
                else if (currentTorrentManager.PartialProgress > 0)
                {
                    AppLogger.LogInfo(ClientAppIdentifier, $"Download progress for {currentDownloadingFile.Path}: {currentTorrentManager.PartialProgress}");
                    currentDownloadingState = currentTorrentManager.PartialProgress >= 0.5 ? DownloadingState.ReadyToPlaySoon : DownloadingState.MediaDownloadStarted;
                }
            }
        }

        private void OnPeersFound(object sender, PeersAddedEventArgs e)
        {
            if (currentTorrentManager?.PartialProgress == 0)
                currentDownloadingState = DownloadingState.MediaDownloadAboutToStart;
        }

        private ClientEngine BuildClientEngine()
        {
            EngineSettingsBuilder settingsBuilder = new EngineSettingsBuilder();
            settingsBuilder.AllowedEncryption.Add(EncryptionType.PlainText | EncryptionType.RC4Full | EncryptionType.RC4Header);

            return new ClientEngine(settingsBuilder.ToSettings());
        }

        private async Task StartTorrentManagerIfNeededAsync()
        {
            while (currentTorrentManager.State == TorrentState.Stopping)
                await Task.Delay(1000);

            if (currentTorrentManager.State == TorrentState.Paused
            || currentTorrentManager.State == TorrentState.Stopped
            || currentTorrentManager.State == TorrentState.Error)
            {
                currentDownloadingState = DownloadingState.WaitMediaDownloadToStart;

                AppLogger.LogInfo(ClientAppIdentifier, $"Start TorrentManager : {currentTorrentManager.Name}");

                await currentTorrentManager.StartAsync();
            }
        }

        private async Task<TorrentManager> CreateTorrentManagerAsync(string torrentUrl)
        {
            try
            {
                currentDownloadingState = DownloadingState.DownloadingTorrentFile;

                this.downloadTorrentCancellationTokenSource?.Cancel();
                this.downloadTorrentCancellationTokenSource = new CancellationTokenSource();

                torrentFileDownloader = new TorrentFileDownloader(ClientAppIdentifier, torrentUrl, clientEngine);
                var torrentFilePath = await torrentFileDownloader.DownloadAsync(downloadTorrentCancellationTokenSource.Token);

                if (string.IsNullOrEmpty(torrentFilePath))
                {
                    currentDownloadingState = DownloadingState.TorrentFileDownloadFailed;
                    return null;
                }

                currentDownloadingState = DownloadingState.TorrentFileDownloaded;

                var existingTorrentManager = clientEngine.Torrents.FirstOrDefault(tm => tm.SavePath == torrentFilePath);

                //if torrent manager for this torrentFilePath already registered in ClientEngine, we need to wait for it to stop and to be removed before creating a new one
                while (existingTorrentManager?.State == TorrentState.Stopping)
                    await Task.Delay(1000);

                AppLogger.LogInfo(ClientAppIdentifier, $"Create TorrentManager from torrent file : {torrentFilePath}");

                return await clientEngine.AddStreamingAsync(torrentFilePath, Path.GetDirectoryName(torrentFilePath));
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ClientAppIdentifier, "GetTorrentManagerAsync", ex);
                currentDownloadingState = DownloadingState.TorrentFileOpeningFailed;
                return null;
            }
        }

        private bool FileToDownloadChanged(ITorrentFileSelector torrentFileSelector)
        {
            var fileToDownload = torrentFileSelector.SelectTorrentFileInfo(currentTorrentManager.Files);
            return this.currentDownloadingFile != fileToDownload;
        }

        private async Task SetFileToDownload(ITorrentFileSelector torrentFileSelector)
        {
            var fileToDownload = torrentFileSelector.SelectTorrentFileInfo(currentTorrentManager.Files);

            if (fileToDownload != null)
            {
                foreach (var file in currentTorrentManager.Files.Where(f => fileToDownload.FullPath != f.FullPath))
                    await currentTorrentManager.SetFilePriorityAsync(file, Priority.DoNotDownload);

                await currentTorrentManager.SetFilePriorityAsync(fileToDownload, Priority.Highest);
                this.currentDownloadingFile = fileToDownload;

                AppLogger.LogInfo(ClientAppIdentifier, $"File to download selected : {this.currentDownloadingFile.Path}");
            }
        }

        private void ReleaseTorrentManager()
        {
            if (currentTorrentManager != null)
            {
                var torrentManagerToRelease = currentTorrentManager;
                AppLogger.LogInfo(ClientAppIdentifier, $"Stop TorrentManager : {torrentManagerToRelease.Name}");

                currentTorrentManager.PeersFound -= OnPeersFound;
                currentTorrentManager.PieceHashed -= OnPieceHashed;

                //stop can be a long operation, so we do it in a separate thread
                torrentManagerToRelease.StopAsync().ContinueWith(async t =>
                {
                    AppLogger.LogInfo(ClientAppIdentifier, $"Remove TorrentManager : {torrentManagerToRelease.Name}");
                    await clientEngine.RemoveAsync(torrentManagerToRelease, RemoveMode.KeepAllData);
                });
            }
        }

        private void ReleaseCurrentStream()
        {
            torrentStreamCancellationTokenSource?.Cancel();

            if (currentTorrentStream != null)
            {
                currentTorrentStream.Dispose();
                currentTorrentStream = null;
            }
        }

    }
}
