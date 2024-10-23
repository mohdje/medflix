using MonoTorrent.Client;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebHostStreaming.Torrent
{
    public class TorrentClientEngine
    {
        private static TorrentClientEngine instance;

        public static TorrentClientEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new TorrentClientEngine();
                return instance;
            }
        }

        public ClientEngine ClientEngine { get; private set; }


        Dictionary<string, TorrentManager> torrentManagerCache;
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private TorrentClientEngine()
        {
            ClientEngine = BuildClientEngine();
            torrentManagerCache = new Dictionary<string, TorrentManager>();
        }

        private ClientEngine BuildClientEngine()
        {
            EngineSettingsBuilder settingsBuilder = new EngineSettingsBuilder();
            settingsBuilder.AllowedEncryption.Add(EncryptionType.PlainText | EncryptionType.RC4Full | EncryptionType.RC4Header);

            return new ClientEngine(settingsBuilder.ToSettings());
        }

        public async Task<TorrentManager> GetTorrentManagerAsync(string torrentFilePath, string torrentDownloadDirectory)
        {
            await semaphoreSlim.WaitAsync();
            if (!File.Exists(torrentFilePath))
                throw new FileNotFoundException($"{torrentFilePath} has not been found");

            try
            {
                if (!torrentManagerCache.ContainsKey(torrentFilePath))
                {
                    var torrentManager = await ClientEngine.AddStreamingAsync(torrentFilePath, torrentDownloadDirectory);
                    torrentManagerCache.Add(torrentFilePath, torrentManager);
                }

                return torrentManagerCache[torrentFilePath];
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

    }
}
