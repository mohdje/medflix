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
            SearchResources,
            PrepareDownload,
            DownloadWillStart,
            DownloadStarted,
            DownloadInProgress,
            DownloadStopped,
            NoValidFileFound
        }
        public string TorrentUri { get; }

        private TorrentManager torrentManager;

        private CancellationTokenSource streamCancellationTokenSource;

        private CancellationTokenSource downloadMetaDataCancellationTokenSource;

        private ITorrentFileInfo movieFileInfo;

        private static HttpClient client;
        private string torrentDownloadDirectory => Path.Combine(Helpers.AppFolders.TorrentsFolder, TorrentUri.ToMD5Hash());
        private string torrentFilePath => Path.Combine(torrentDownloadDirectory, "torrent");

        private DownloadingState downloadingState;

        private bool watchDownloadProgress;

        #region private

        private MovieStream(string torrentUri)
        {
            TorrentUri = torrentUri;
        }

        private async void StartAsync(ClientEngine clientEngine, string videoFormat)
        {
            downloadingState = DownloadingState.SearchResources;

            if (downloadMetaDataCancellationTokenSource != null)
                downloadMetaDataCancellationTokenSource.Cancel();

            downloadMetaDataCancellationTokenSource = new CancellationTokenSource();

            await DownloadTorrentFileAsync(clientEngine, downloadMetaDataCancellationTokenSource.Token).ContinueWith(t =>
            {
                torrentManager = clientEngine.AddStreamingAsync(torrentFilePath, torrentDownloadDirectory).Result;
                movieFileInfo = GetMovieFile(torrentManager, videoFormat);

                if(movieFileInfo != null)
                {
                    ListenTorrentManagerEvents();
                    torrentManager.SetFilePriorityAsync(movieFileInfo, Priority.Highest);
                    downloadingState = DownloadingState.PrepareDownload;
                }
                else
                    downloadingState =  DownloadingState.NoValidFileFound;
            });
        }

        private ITorrentFileInfo GetMovieFile(TorrentManager torrentManager, string videoFormat)
        {
            foreach (var torrentFile in torrentManager.Files)
                torrentManager.SetFilePriorityAsync(torrentFile, Priority.DoNotDownload);

            return torrentManager.Files.FirstOrDefault(f => f.FullPath.MatchVideoFormat(videoFormat));
        }

        private async Task DownloadTorrentFileAsync(ClientEngine clientEngine, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(torrentDownloadDirectory))
                Directory.CreateDirectory(torrentDownloadDirectory);


            if (client == null)
                client = new HttpClient();

            await Task.Run(() =>
            {
                lock (client)
                {
                    if (!File.Exists(torrentFilePath))
                    {
                        var bytes = TorrentUri.IsMagnetLink() ? clientEngine.DownloadMetadataAsync(MagnetLink.Parse(TorrentUri), cancellationToken).Result : client.GetByteArrayAsync(TorrentUri, cancellationToken).Result;
                        File.WriteAllBytes(torrentFilePath, bytes);
                    }
                }
            }).ContinueWith(t =>
            {
                client.Dispose();
                client = null;
                if (!TorrentHelper.IsValidTorrentFile(torrentFilePath))
                    TorrentHelper.FixTorrentFile(torrentFilePath);
            });
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

        #region public
        public static MovieStream CreateMovieStream(ClientEngine clientEngine, string torrentUri, string videoFormat)
        {
            var movieStream = new MovieStream(torrentUri);
            movieStream.StartAsync(clientEngine, videoFormat);

            return movieStream;
        }

        public async void PauseDownloadAsync()
        {
            watchDownloadProgress = false;
            downloadingState = DownloadingState.DownloadStopped;
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
                case DownloadingState.DownloadStopped:
                    return null;
                default:
                    state.Message = "Searching resources for download (step 1/5)";
                    break;
            }

            return state;
        }

        public StreamDto GetStream(int offset)
        {
            while (downloadingState == DownloadingState.SearchResources)
            {
                Task.Delay(2000).Wait();
            }

            if (movieFileInfo == null)
                return null;

            if (torrentManager.State == TorrentState.Paused || torrentManager.State == TorrentState.Stopped)
            {
                ListenTorrentManagerEvents();
                torrentManager.StartAsync().Wait();
            }

            if (streamCancellationTokenSource != null)
                streamCancellationTokenSource.Cancel();

            streamCancellationTokenSource = new CancellationTokenSource();

            if (torrentManager.StreamProvider.ActiveStream == null || torrentManager.StreamProvider.ActiveStream.Disposed)
                torrentManager.StreamProvider.CreateStreamAsync(movieFileInfo, streamCancellationTokenSource.Token, offset).Wait();
            else if (offset > 0)
            {
                torrentManager.StreamProvider.ActiveStream.Seek(offset, SeekOrigin.Begin);
                torrentManager.StreamProvider.ActiveStream.ReadAsync(new byte[1], 0, 1, streamCancellationTokenSource.Token).Wait();
            }

            return new StreamDto(torrentManager.StreamProvider.ActiveStream, Path.GetExtension(movieFileInfo.FullPath));
        }

        #endregion
    }
}
