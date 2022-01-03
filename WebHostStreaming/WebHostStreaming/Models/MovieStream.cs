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

        private CancellationTokenSource cancellationTokenSource;

        private ITorrentFileInfo movieFileInfo;

        private static HttpClient client;
        private string torrentDownloadDirectory => Path.Combine(Helpers.AppFolders.TorrentsFolder, TorrentUri.ToMD5Hash());
        private string torrentFileName => Path.Combine(torrentDownloadDirectory, "torrent");

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

            await DownloadTorrentFileAsync().ContinueWith(t =>
            {
                torrentManager = clientEngine.AddStreamingAsync(torrentFileName, torrentDownloadDirectory).Result;
                movieFileInfo = GetMovieFile(torrentManager, videoFormat);
                downloadingState = DownloadingState.PrepareDownload;
            });
        }

        private ITorrentFileInfo GetMovieFile(TorrentManager torrentManager, string videoFormat)
        {
            ITorrentFileInfo torrentFileInfo = null;

            foreach (var torrentFile in torrentManager.Files)
                torrentManager.SetFilePriorityAsync(torrentFile, Priority.DoNotDownload);

            torrentFileInfo = torrentManager.Files.FirstOrDefault(f => f.FullPath.MatchVideoFormat(videoFormat));
            if (torrentFileInfo != null)
            {
                ListenTorrentManagerEvents();
                torrentManager.SetFilePriorityAsync(torrentFileInfo, Priority.Highest);
            }
            else
                downloadingState = DownloadingState.NoValidFileFound;

            return torrentFileInfo;
        }

        private async Task DownloadTorrentFileAsync()
        {
            if (!Directory.Exists(torrentDownloadDirectory))
                Directory.CreateDirectory(torrentDownloadDirectory);

            if (client == null)
                client = new HttpClient();

            await Task.Run(() =>
                {
                    lock (client)
                    {
                        if (!File.Exists(torrentFileName))
                        {
                            var bytes = client.GetByteArrayAsync(TorrentUri).Result;
                            File.WriteAllBytes(torrentFileName, bytes);
                        }
                    }
                }).ContinueWith(t =>
                {
                    client.Dispose();
                    client = null;
                    if (!TorrentHelper.IsValidTorrentFile(torrentFileName))
                        TorrentHelper.FixTorrentFile(torrentFileName);
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

        public string GetDownloadingState()
        {
            switch (downloadingState)
            {
                case DownloadingState.SearchResources:
                    return "Searching resources for download (step 1/5)";
                case DownloadingState.PrepareDownload:
                    return "Preparing download (step 2/5)";
                case DownloadingState.DownloadWillStart:
                    return "Download will start soon (step 3/5)";
                case DownloadingState.DownloadStarted:
                    return "Download has started (step 4/5)";
                case DownloadingState.DownloadInProgress:
                    return "Download in progress, video will be ready to play soon (step 5/5)";
                case DownloadingState.NoValidFileFound:
                case DownloadingState.DownloadStopped:
                    return null;
                default:
                    break;
            }

            return null;
        }

        public StreamDto GetStream(int offset)
        {            
            while(downloadingState == DownloadingState.SearchResources)
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

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            if (torrentManager.StreamProvider.ActiveStream == null || torrentManager.StreamProvider.ActiveStream.Disposed)
                torrentManager.StreamProvider.CreateStreamAsync(movieFileInfo, cancellationTokenSource.Token, offset).Wait();
            else if (offset > 0)
            {
                torrentManager.StreamProvider.ActiveStream.Seek(offset, SeekOrigin.Begin);
                torrentManager.StreamProvider.ActiveStream.ReadAsync(new byte[1], 0, 1, cancellationTokenSource.Token).Wait();
            }

            return new StreamDto(torrentManager.StreamProvider.ActiveStream, Path.GetExtension(movieFileInfo.FullPath));
        }

        #endregion
    }
}
