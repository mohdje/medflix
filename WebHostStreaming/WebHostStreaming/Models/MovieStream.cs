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

namespace WebHostStreaming.Models
{
    public class MovieStream 
    {
        public string TorrentUri { get; }

        private TorrentManager torrentManager;

        private CancellationTokenSource cancellationTokenSource;

        private ITorrentFileInfo movieFileInfo;

        private static WebClient client;
        private string torrentDownloadDirectory => Path.Combine(Helpers.AppFolders.TorrentsFolder, TorrentUri.ToMD5Hash());

        public MovieStream(ClientEngine clientEngine, string torrentUri)
        {
            if(client == null)
            {
                client = new WebClient();
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36");
            }
           
            TorrentUri = torrentUri;
            torrentManager = clientEngine.AddStreamingAsync(GetTorrentFile(TorrentUri), torrentDownloadDirectory).Result;
            movieFileInfo = GetMovieFile(torrentManager);      
        }

        public async void PauseDownloadAsync()
        {
            await torrentManager.PauseAsync();
        }

        public Stream GetStream(int offset)
        {
            if (movieFileInfo == null)
                return null;

            if (torrentManager.State == TorrentState.Paused || torrentManager.State == TorrentState.Stopped)
                torrentManager.StartAsync().Wait();

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
            var defaultStep = "Searching resources for download (step 1/5)";

            if (torrentManager == null)
                return defaultStep;

            if (torrentManager.PartialProgress > 0.3)
                return "Download in progress, video will be ready to play soon (step 5/5)";

            if (torrentManager.PartialProgress > 0)
                return "Download has started (step 4/5)";

            if(Directory.Exists(torrentDownloadDirectory) && Directory.GetFiles(torrentDownloadDirectory, Path.GetFileName(movieFileInfo.FullPath), SearchOption.AllDirectories).Any())
                return "Download will start soon (step 3/5)";

            if (File.Exists(Path.Combine(torrentDownloadDirectory, "torrent")))
                return "Preparing download (step 2/5)";

            return defaultStep;
        }

        private string GetTorrentFile(string torrentUri)
        {
            if (!Directory.Exists(torrentDownloadDirectory))
                Directory.CreateDirectory(torrentDownloadDirectory);

            var fileName = Path.Combine(torrentDownloadDirectory, "torrent");

            lock (client)
            {
                if (!File.Exists(fileName))
                {        
                    client.DownloadFile(torrentUri, fileName);
                }
            }

            return fileName;
        }

        private ITorrentFileInfo GetMovieFile(TorrentManager torrentManager)
        {
            ITorrentFileInfo torrentFileInfo = null;
            foreach (var torrentFile in torrentManager.Files)           
               torrentManager.SetFilePriorityAsync(torrentFile, Priority.DoNotDownload);
            
            torrentFileInfo = torrentManager.Files.FirstOrDefault(f => f.FullPath.IsMP4File());
            if (torrentFileInfo != null)
                torrentManager.SetFilePriorityAsync(torrentFileInfo, Priority.Highest);

            return torrentFileInfo;
        }
    }
}
