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
using Chromium;

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
            NoValidFileFound
        }
        public string TorrentUri { get; }

        private TorrentManager torrentManager;

        private CancellationTokenSource cancellationTokenSource;

        private ITorrentFileInfo movieFileInfo;

        private static ChromiumWebClient client;
        private string torrentDownloadDirectory => Path.Combine(Helpers.AppFolders.TorrentsFolder, TorrentUri.ToMD5Hash());

        private DownloadingState downloadingState;

        private bool watchDownloadProgress;

        public MovieStream(ClientEngine clientEngine, string torrentUri)
        {
            TorrentUri = torrentUri;
            downloadingState = DownloadingState.SearchResources;
            torrentManager = clientEngine.AddStreamingAsync(GetTorrentFile(TorrentUri), torrentDownloadDirectory).Result;         
            movieFileInfo = GetMovieFile(torrentManager);      
        }

        public async void PauseDownloadAsync()
        {
            watchDownloadProgress = false;
            await torrentManager.PauseAsync();
        }

        public Stream GetStream(int offset)
        {
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

           return torrentManager.StreamProvider.ActiveStream; 
        }

        public string GetDownloadingState()
        {
            if (!watchDownloadProgress)
                return null;

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
                    return null;
                default:
                    break;
            }

            return null;
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

        private string GetTorrentFile(string torrentUri)
        {
            if (!Directory.Exists(torrentDownloadDirectory))
                Directory.CreateDirectory(torrentDownloadDirectory);

            var fileName = Path.Combine(torrentDownloadDirectory, "torrent");

            if (client == null)
                client = new ChromiumWebClient();

            lock (client)
            {              
                if (!File.Exists(fileName))
                {
                    client.DownloadFileAsync(torrentUri, fileName).Wait();
                    client.Dispose();
                    client = null;
                }
            }

            downloadingState = DownloadingState.PrepareDownload;

            return fileName;
        }

        private ITorrentFileInfo GetMovieFile(TorrentManager torrentManager)
        {
            ITorrentFileInfo torrentFileInfo = null;
            foreach (var torrentFile in torrentManager.Files)           
               torrentManager.SetFilePriorityAsync(torrentFile, Priority.DoNotDownload);
            
            torrentFileInfo = torrentManager.Files.FirstOrDefault(f => f.FullPath.IsMP4File());
            if (torrentFileInfo != null)
            {
                ListenTorrentManagerEvents();
                torrentManager.SetFilePriorityAsync(torrentFileInfo, Priority.Highest);
            }
            else
                downloadingState = DownloadingState.NoValidFileFound;

            return torrentFileInfo;
        }
    }
}
