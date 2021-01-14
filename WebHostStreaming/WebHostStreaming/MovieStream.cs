using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming
{
    public class MovieStream
    {
        string torrentDownloadDirectory = Helpers.AppFolders.TorrentsFolder;
        ClientEngine clientEngine;
        StreamProvider streamProvider;
        Stream stream;
        Torrent torrent;
        TorrentFile movieFile;
        string torrentUri;

        static MovieStream instance;
        public static MovieStream Instance { get
            {
                if (instance == null)
                    instance = new MovieStream();

                return instance;
            }
        }

        private MovieStream()
        {
            clientEngine = CreateEngine();
        }

        public async Task<Stream> StreamMovie(string torrentUri, long offset)
        {
            if(this.torrentUri != torrentUri)
            {
                this.torrentUri = torrentUri;
                torrent = Torrent.Load(GetTorrentFile(torrentUri));

                foreach(var file in torrent.Files)
                   file.Priority = Priority.DoNotDownload;

                movieFile = torrent.Files.FirstOrDefault(f => f.FullPath.EndsWith(".mp4"));
                movieFile.Priority = Priority.Highest;

                streamProvider = new StreamProvider(clientEngine, torrentDownloadDirectory, torrent);
              
                await streamProvider.StartAsync();
            }

            if (stream != null)
                stream.Dispose();

            stream = await streamProvider.CreateStreamAsync(movieFile);

            if (offset > 0)
                stream.Seek(offset, SeekOrigin.Begin);

            return stream;
        }

        private string GetTorrentFile(string torrentUri)
        {
            var fileName = Path.Combine(Helpers.AppFolders.TorrentsFolder, torrentUri.GetHashCode().ToString());

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
