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
        public event EventHandler<VideoInfo> OnVideoDownloadCompleted;
        public DateTime LastDownloadProgressDateTime { get; private set; }
        public bool IsDownloadComplete => currentDownloadingFile?.BitField.PercentComplete >= 99;


        private ClientEngine clientEngine;
        private TorrentManager currentTorrentManager;
        private TorrentMetadataDownloader torrentFileDownloader;
        private string currentTorrentUrl;
        private ITorrentManagerFile currentDownloadingFile;
        private TorrentStream currentTorrentStream;
        private VideoInfo currentVideoInfo;
        private DownloadingState currentDownloadingState;
        private bool fileDownloadCompleteEventFired;

        private CancellationTokenSource torrentStreamCancellationTokenSource;
        private CancellationTokenSource downloadTorrentCancellationTokenSource;
        private readonly SemaphoreSlim torrentManagerCreationLocker = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim streamCreationLocker = new SemaphoreSlim(1, 1);


        public TorrentClient(string clientAppIdentifier)
        {
            this.clientEngine = BuildClientEngine();
            ClientAppIdentifier = clientAppIdentifier;
        }

        #region Public
        public void Dispose()
        {
            ReleaseTorrentManager();
            clientEngine.Dispose();

            currentTorrentManager = null;
            torrentFileDownloader = null;
        }

        public async Task<bool> StartDownloadTorrentMediaAsync(TorrentRequest torrentRequest)
        {
            return await StartDownloadingMediaAsync(torrentRequest.TorrentUrl, torrentRequest.VideoInfo, false);
        }

        public async Task<TorrentStream> GetTorrentStreamAsync(TorrentRequest torrentRequest)
        {
            var downloadStarted = await StartDownloadingMediaAsync(torrentRequest.TorrentUrl, torrentRequest.VideoInfo, true);

            if (!downloadStarted)
                return null;

            try
            {
                var fileToStream = GetFileFromTorrent(torrentRequest.VideoInfo);

                if (currentTorrentStream?.MediaFullPath != fileToStream.FullPath)
                {
                    ReleaseCurrentStream();

                    await streamCreationLocker.WaitAsync();

                    AppLogger.LogInfo(ClientAppIdentifier, $"Create stream for file : {fileToStream.Path}");

                    currentTorrentStream = new TorrentStream(currentTorrentManager, fileToStream);
                    torrentStreamCancellationTokenSource = new CancellationTokenSource();
                    await currentTorrentStream.StartAsync(torrentStreamCancellationTokenSource.Token);
                }
            }
            catch (Exception ex)
            {
                if (torrentStreamCancellationTokenSource.IsCancellationRequested)
                    AppLogger.LogInfo(ClientAppIdentifier, $"Stream creation aborted for TorrentManager : {currentTorrentStream.MediaFileName}");
                else
                {
                    AppLogger.LogInfo(ClientAppIdentifier, $"Stream creation failed for TorrentManager : {currentTorrentStream.MediaFileName}");
                    AppLogger.LogError(ClientAppIdentifier, "GetTorrentStreamAsync", ex);

                    currentDownloadingState = DownloadingState.TorrentFileOpeningFailed;
                }

                return null;
            }
            finally
            {
                streamCreationLocker.Release();
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

        #endregion

        #region Core Functions
        private ClientEngine BuildClientEngine()
        {
            EngineSettingsBuilder settingsBuilder = new EngineSettingsBuilder();
            settingsBuilder.AllowedEncryption.Add(EncryptionType.PlainText | EncryptionType.RC4Full | EncryptionType.RC4Header);

            return new ClientEngine(settingsBuilder.ToSettings());
        }

        private async Task<bool> StartDownloadingMediaAsync(string torrentUrl, VideoInfo videoFileToDownload, bool streamingMode)
        {
            await CreateTorrentManagerIfNeededAsync(torrentUrl, streamingMode);
            if (currentTorrentManager == null)
                return false;

            var fileToDownload = GetFileFromTorrent(videoFileToDownload);
            if (fileToDownload.BitField.PercentComplete < 99)
            {
                await SetFileToDownload(videoFileToDownload);

                if (currentDownloadingFile == null)
                {
                    currentDownloadingState = DownloadingState.NoMediaFileFoundInTorrent;
                    AppLogger.LogInfo(ClientAppIdentifier, $"No video file found to download for TorrentManager : {currentTorrentManager.Name}");
                    return false;
                }
            }

            await StartTorrentManagerIfNeededAsync();

            return true;
        }

        private async Task CreateTorrentManagerIfNeededAsync(string torrentUrl, bool streamingMode)
        {
            if (currentTorrentUrl != torrentUrl)
            {
                ReleaseTorrentManager();
                currentTorrentUrl = torrentUrl;
                currentDownloadingState = DownloadingState.Loading;
                LastDownloadProgressDateTime = DateTime.UtcNow;
            }

            await torrentManagerCreationLocker.WaitAsync();

            try
            {
                if (currentTorrentManager != null && currentTorrentUrl == torrentUrl)
                    return;

                torrentFileDownloader = new TorrentMetadataDownloader(ClientAppIdentifier, torrentUrl, clientEngine);
                currentDownloadingState = DownloadingState.DownloadingTorrentFile;

                this.downloadTorrentCancellationTokenSource = new CancellationTokenSource();
                var torrentFilePath = await torrentFileDownloader.DownloadAsync(downloadTorrentCancellationTokenSource.Token);

                if (torrentFileDownloader.Status == TorrentDownloaderStatus.DownloadAborted)
                    return;
                else if (torrentFileDownloader.Status != TorrentDownloaderStatus.DownloadCompleted)
                {
                    currentDownloadingState = DownloadingState.TorrentFileDownloadFailed;
                    return;
                }

                currentDownloadingState = DownloadingState.TorrentFileDownloaded;

                //if torrent manager for this torrentFilePath already registered in ClientEngine, we need to wait for it to stop and to be removed before creating a new one
                while (clientEngine.Torrents.Any(tm => tm.SavePath == torrentFilePath))
                {
                    AppLogger.LogInfo(ClientAppIdentifier, $"Waiting for existing TorrentManager to be removed for torrent file : {torrentFilePath}");
                    await Task.Delay(1000);
                }

                AppLogger.LogInfo(ClientAppIdentifier, $"Create TorrentManager from torrent file : {torrentFilePath}");

                currentTorrentManager = streamingMode ?
                    await clientEngine.AddStreamingAsync(torrentFilePath, Path.GetDirectoryName(torrentFilePath)) :
                    await clientEngine.AddAsync(torrentFilePath, Path.GetDirectoryName(torrentFilePath));

                if (currentTorrentManager != null)
                {
                    currentTorrentManager.PeersFound += OnPeersFound;
                    currentTorrentManager.PieceHashed += OnPieceHashed;
                }
            }
            catch (Exception ex)
            {
                if (downloadTorrentCancellationTokenSource.IsCancellationRequested)
                    AppLogger.LogInfo(ClientAppIdentifier, $"Create TorrentManager aborted for Torrent url : {torrentUrl}");
                else
                {
                    AppLogger.LogError(ClientAppIdentifier, "GetTorrentManagerAsync", ex);
                    currentDownloadingState = DownloadingState.TorrentFileOpeningFailed;
                }

                return;
            }
            finally
            {
                torrentManagerCreationLocker.Release();
            }
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

        #endregion

        #region Torrent Events
        private async void OnPieceHashed(object sender, PieceHashedEventArgs e)
        {
            var percentComplete = currentDownloadingFile?.BitField.PercentComplete;
            if (percentComplete.GetValueOrDefault(0) > 0 && percentComplete < 100)
            {
                LastDownloadProgressDateTime = DateTime.UtcNow;
                AppLogger.LogInfo(ClientAppIdentifier, $"Download progress for {currentDownloadingFile.Path}: {percentComplete:0.00}%");

                if (percentComplete >= 99 && !fileDownloadCompleteEventFired)
                {
                    fileDownloadCompleteEventFired = true;
                    OnVideoDownloadCompleted?.Invoke(this, currentVideoInfo);

                    //If serie, set next episode to download if available in same torrent
                    if (currentVideoInfo.IsSerie)
                    {
                        await SetFileToDownload(new VideoInfo
                        {
                            MediaId = currentVideoInfo.MediaId,
                            Language = currentVideoInfo.Language,
                            SeasonNumber = currentVideoInfo.SeasonNumber,
                            EpisodeNumber = currentVideoInfo.EpisodeNumber + 1,
                            Quality = currentVideoInfo.Quality
                        });
                    }
                }
                else if (percentComplete >= 0.5)
                    currentDownloadingState = DownloadingState.ReadyToPlaySoon;
                else
                    currentDownloadingState = DownloadingState.MediaDownloadStarted;
            }
        }


        private void OnPeersFound(object sender, PeersAddedEventArgs e)
        {
            if (currentTorrentManager?.PartialProgress == 0)
                currentDownloadingState = DownloadingState.MediaDownloadAboutToStart;
        }
        #endregion

        #region File Utils

        private async Task SetFileToDownload(VideoInfo videoFileToDownload)
        {
            var mediaFile = GetFileFromTorrent(videoFileToDownload);

            if (mediaFile != null && this.currentDownloadingFile != mediaFile)
            {
                this.currentDownloadingFile = mediaFile;

                this.currentVideoInfo = videoFileToDownload;
                this.currentVideoInfo.FilePath = mediaFile.FullPath;

                fileDownloadCompleteEventFired = false;
                LastDownloadProgressDateTime = DateTime.UtcNow;

                foreach (var file in currentTorrentManager.Files.Where(f => mediaFile.FullPath != f.FullPath))
                    await currentTorrentManager.SetFilePriorityAsync(file, Priority.DoNotDownload);

                await currentTorrentManager.SetFilePriorityAsync(mediaFile, Priority.Highest);

                AppLogger.LogInfo(ClientAppIdentifier, $"File to download selected : {this.currentDownloadingFile.Path}");
            }
        }

        private ITorrentManagerFile GetFileFromTorrent(VideoInfo videoFileToSelect)
        {
            var videoFiles = currentTorrentManager.Files
                .Where(f => f.FullPath.EndsWith(".mp4") || f.FullPath.EndsWith(".avi") || f.FullPath.EndsWith(".mkv"));

            if (videoFileToSelect.IsSerie)
            {
                var episodeId = $"S{videoFileToSelect.SeasonNumber.ToString("00")}E{videoFileToSelect.EpisodeNumber.ToString("00")}";
                return videoFiles.FirstOrDefault(f => Path.GetFileName(f.FullPath).Contains(episodeId, System.StringComparison.OrdinalIgnoreCase));
            }
            else
                return videoFiles.FirstOrDefault();
        }

        #endregion

        #region Release Resources
        private void ReleaseTorrentManager()
        {
            ReleaseCurrentStream();

            downloadTorrentCancellationTokenSource?.Cancel();

            if (currentTorrentManager != null)
            {
                currentTorrentManager.PeersFound -= OnPeersFound;
                currentTorrentManager.PieceHashed -= OnPieceHashed;

                var torrentManagerToRelease = clientEngine.Torrents.FirstOrDefault(tm => tm.Name == currentTorrentManager.Name);
                currentTorrentManager = null;

                AppLogger.LogInfo(ClientAppIdentifier, $"Stop TorrentManager : {torrentManagerToRelease.Name}");

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
            currentTorrentStream?.Dispose();
            currentTorrentStream = null;
        }
        #endregion
    }
}
