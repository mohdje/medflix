using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Streaming;
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Models
{
    public class MovieStream
    {
        private enum DownloadingState
        {
            Init,
            ErrorDownloadTorrent,
            NoValidFileFound,
            SearchResources,
            PrepareDownload,
            DownloadWillStart,
            DownloadStarted,
            DownloadInProgress,

        }
        public string TorrentUri { get; }


        private TorrentManager torrentManager;

        private CancellationTokenSource cancellationTokenSource;

        private ITorrentFileInfo movieFileInfo;

        private static HttpClient client = new HttpClient();
        private string torrentDownloadDirectory => Path.Combine(Helpers.AppFolders.TorrentsFolder, TorrentUri.ToMD5Hash());
        private string torrentFilePath => Path.Combine(torrentDownloadDirectory, "torrent");

        private DownloadingState downloadingState;

        private bool watchDownloadProgress;

        private string videoFormat;

        public string VideoFormat => videoFormat;

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly ClientEngine clientEngine;

        public MovieStream(string torrentUri, string videoFormat, ClientEngine clientEngine)
        {
            TorrentUri = torrentUri;
            this.clientEngine = clientEngine;
            this.videoFormat = videoFormat;
            downloadingState = DownloadingState.Init;
        }

        #region public
        public async Task<StreamDto> GetStreamAsync(int offset)
        {
            cancellationTokenSource = new CancellationTokenSource();

            if (downloadingState < DownloadingState.PrepareDownload)
            {
                await StartDownloadAsync();
            }

            if (torrentManager == null || movieFileInfo == null)
                return null;

            if (torrentManager.State == TorrentState.Paused
                || torrentManager.State == TorrentState.Stopped
                || torrentManager.State == TorrentState.Error)
            {
                ListenTorrentManagerEvents();
                await torrentManager.StartAsync();
            }

            if (torrentManager.StreamProvider.ActiveStream == null || torrentManager.StreamProvider.ActiveStream.Disposed)
                await torrentManager.StreamProvider.CreateStreamAsync(movieFileInfo, cancellationTokenSource.Token, offset);
            else if (offset > 0)
            {
                torrentManager.StreamProvider.ActiveStream.Seek(offset, SeekOrigin.Begin);
                await torrentManager.StreamProvider.ActiveStream.ReadAsync(new byte[1], 0, 1, cancellationTokenSource.Token);
            }

            return new StreamDto(torrentManager.StreamProvider.ActiveStream, Path.GetExtension(movieFileInfo.FullPath));
        }

        public async Task PauseDownloadAsync()
        {
            watchDownloadProgress = false;

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            if (torrentManager != null)
                await torrentManager.PauseAsync();
        }

        public Models.DownloadingState GetDownloadingState()
        {
            var state = new Models.DownloadingState();
            switch (downloadingState)
            {
                case DownloadingState.SearchResources:
                    state.Message = "Searching resources for download (step 1/5)";
                    break;
                case DownloadingState.PrepareDownload:
                    state.Message = "Preparing download (step 2/5)";
                    break;
                case DownloadingState.DownloadWillStart:
                    state.Message = "Download will start soon (step 3/5)";
                    break;
                case DownloadingState.DownloadStarted:
                    state.Message = "Download has started (step 4/5)";
                    break;
                case DownloadingState.DownloadInProgress:
                    state.Message = "Download in progress, video will be ready to play soon (step 5/5)";
                    break;
                case DownloadingState.NoValidFileFound:
                    state.Message = "Format invalid. Try with VLC Player";
                    state.Error = true;
                    break;
                case DownloadingState.ErrorDownloadTorrent:
                    state.Message = "Error trying to download torrent";
                    state.Error = true;
                    break;
                default:
                    state.Message = "Searching resources for download (step 1/5)";
                    break;
            }

            return state;
        }

        public void UpdateVideoFormat(string videoFormat)
        {
            this.videoFormat = videoFormat;
            movieFileInfo = null;
            downloadingState = DownloadingState.Init;
        }


        #endregion

        #region private
        private async Task StartDownloadAsync()
        {
            downloadingState = DownloadingState.SearchResources;

            if (!File.Exists(torrentFilePath))
            {
                await DownloadTorrentFileAsync(clientEngine, cancellationTokenSource.Token);
            }

            if (cancellationTokenSource.IsCancellationRequested)
                return;

            if (File.Exists(torrentFilePath))
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    if (torrentManager == null)
                    {
                        torrentManager = await clientEngine.AddStreamingAsync(torrentFilePath, torrentDownloadDirectory);
                    }
                }
                finally
                {
                    semaphoreSlim.Release();
                }

                movieFileInfo = await GetMovieFileAsync(torrentManager);

                if (movieFileInfo != null)
                {
                    await torrentManager.SetFilePriorityAsync(movieFileInfo, Priority.Highest);
                    downloadingState = DownloadingState.PrepareDownload;

                    if (cancellationTokenSource.IsCancellationRequested)
                        await torrentManager.PauseAsync();
                    else
                        ListenTorrentManagerEvents();
                }
                else
                    downloadingState = DownloadingState.NoValidFileFound;
            }
            else
            {
                downloadingState = DownloadingState.ErrorDownloadTorrent;
            }
        }

        private async Task<ITorrentFileInfo> GetMovieFileAsync(TorrentManager torrentManager)
        {
            if (torrentManager?.Files == null)
                return null;

            foreach (var torrentFile in torrentManager.Files)
                await torrentManager.SetFilePriorityAsync(torrentFile, Priority.DoNotDownload);

            return torrentManager.Files.FirstOrDefault(f => f.FullPath.MatchVideoFormat(videoFormat));
        }

        private async Task DownloadTorrentFileAsync(ClientEngine clientEngine, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(torrentDownloadDirectory))
                Directory.CreateDirectory(torrentDownloadDirectory);

            byte[] bytes = null;
            try
            {
                if (TorrentUri.IsMagnetLink())
                    bytes = await clientEngine.DownloadMetadataAsync(MagnetLink.Parse(TorrentUri), cancellationToken);
                else
                    bytes = await client.GetByteArrayAsync(TorrentUri, cancellationToken);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                    downloadingState = DownloadingState.ErrorDownloadTorrent;
            }

            if (bytes != null && bytes.Any())
            {
                File.WriteAllBytes(torrentFilePath, bytes);
                if (File.Exists(torrentFilePath) && !TorrentHelper.IsValidTorrentFile(torrentFilePath))
                    TorrentHelper.FixTorrentFile(torrentFilePath);
            }
        }

        private void ListenTorrentManagerEvents()
        {
            watchDownloadProgress = true;
            torrentManager.PeersFound += TorrentManager_PeersFound;
            torrentManager.TorrentStateChanged += TorrentManager_TorrentStateChanged;
        }
        private void TorrentManager_PeersFound(object sender, PeersAddedEventArgs e)
        {
            torrentManager.PeersFound -= TorrentManager_PeersFound;
            downloadingState = DownloadingState.DownloadWillStart;
        }

        private void TorrentManager_TorrentStateChanged(object sender, TorrentStateChangedEventArgs e)
        {
            if (e.NewState == TorrentState.Downloading)
            {
                torrentManager.TorrentStateChanged -= TorrentManager_TorrentStateChanged;

                while (watchDownloadProgress)
                {
                    if (e.TorrentManager.PartialProgress > 0.5)
                    {
                        downloadingState = DownloadingState.DownloadInProgress;
                        watchDownloadProgress = false;
                    }
                    else if (e.TorrentManager.PartialProgress > 0)
                        downloadingState = DownloadingState.DownloadStarted;

                    Task.Delay(3000).Wait();
                }
            }
        }


        #endregion


    }
}
