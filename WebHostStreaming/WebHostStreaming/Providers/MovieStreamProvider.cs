using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;

namespace WebHostStreaming.Providers
{
    public class MovieStreamProvider : IMovieStreamProvider
    {
        string torrentDownloadDirectory;
        ClientEngine clientEngine;
        StreamProvider streamProvider;
        Stream stream;
        Torrent torrent;
        TorrentFile movieFile;
        string torrentUri;
        bool streamProviderIsReady;

        public MovieStreamProvider()
        {
            clientEngine = CreateEngine();
        }
        public async Task<Stream> GetMovieStreamAsync(string torrentUri, long offset)
        {
            if (this.torrentUri != torrentUri)
                InitializeStreamProvider(torrentUri);

            while (!streamProviderIsReady)           
                await Task.Delay(1000);
  
            lock (streamProvider)
            {
                if (stream != null)
                    stream.Dispose();

                stream = streamProvider.CreateStreamAsync(movieFile).Result;
            }

            if (stream != null && offset > 0)
                stream.Seek(offset, SeekOrigin.Begin);

            return stream;
        }
        private void InitializeStreamProvider(string torrentUri)
        {
            streamProviderIsReady = false;

            if (stream != null)
            {
                stream.Dispose();
                if (streamProvider.Active)
                    streamProvider.StopAsync().Wait();
            }

            this.torrentUri = torrentUri;

            torrent = Torrent.Load(GetTorrentFile(torrentUri));

            foreach (var file in torrent.Files)            
                file.Priority = file.FullPath.IsMP4File() ? Priority.Highest : Priority.DoNotDownload;

            movieFile = torrent.Files.FirstOrDefault(f => f.FullPath.IsMP4File());

            streamProvider = new StreamProvider(clientEngine, torrentDownloadDirectory, torrent);

            streamProvider.StartAsync().Wait();

            streamProviderIsReady = true;
        }

        private string GetTorrentFile(string torrentUri)
        {
            torrentDownloadDirectory = Path.Combine(Helpers.AppFolders.TorrentsFolder, torrentUri.ToMD5Hash());

            if (!Directory.Exists(torrentDownloadDirectory))
                Directory.CreateDirectory(torrentDownloadDirectory);

            var fileName = Path.Combine(torrentDownloadDirectory, "torrent");

            if (!File.Exists(fileName))
            {
                using (var client = new System.Net.WebClient())
                {
                    client.Headers.Add("User-Agent", "Mozilla / 5.0(Windows NT 6.3; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 87.0.4280.88 Safari / 537.36");
                    client.DownloadFile(torrentUri, fileName);
                }
            }

            return fileName;
        }

        private ClientEngine CreateEngine()
        {
            EngineSettings settings = new EngineSettings();
            settings.AllowedEncryption = ChooseEncryption();

            // If both encrypted and unencrypted connections are supported, an encrypted connection will be attempted
            // first if this is true. Otherwise an unencrypted connection will be attempted first.
            settings.PreferEncryption = true;

            // Torrents will be downloaded here by default when they are registered with the engine
            settings.SavePath = torrentDownloadDirectory;

            return new ClientEngine(settings);
        }

        private EncryptionTypes ChooseEncryption()
        {
            return EncryptionTypes.PlainText | EncryptionTypes.RC4Full | EncryptionTypes.RC4Header;
        }
    }
}
