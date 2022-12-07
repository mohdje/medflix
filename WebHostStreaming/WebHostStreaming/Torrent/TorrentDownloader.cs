using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;

namespace WebHostStreaming.Torrent
{
    public enum TorrentDownloaderStatus
    {
        DownloadHasStarted,
        DownloadFailed,
        DownloadCompleted
    }
    public class TorrentDownloader
    {
        private readonly string torrentUri;
        private static HttpClient client = new HttpClient();

        private const string TORRENT_HEXA_SIGNATURE = "64383a616e6e6f756e6365";

        private bool IsMagnetLink => torrentUri.StartsWith("magnet:?xt=urn:btih");

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public string TorrentDownloadDirectory => Path.Combine(Helpers.AppFolders.TorrentsFolder, torrentUri.ToMD5Hash());
        public string TorrentFilePath => Path.Combine(TorrentDownloadDirectory, "torrent");

        public TorrentDownloaderStatus Status { get; private set; }

        public TorrentDownloader(string torrentUri)
        {
            this.torrentUri = torrentUri;
        }

        public async Task DownloadTorrentFileAsync(CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync();

            if (File.Exists(TorrentFilePath))
            {
                Status = TorrentDownloaderStatus.DownloadCompleted;
                semaphoreSlim.Release();
                return;
            }

            Status = TorrentDownloaderStatus.DownloadHasStarted;

            if (!Directory.Exists(TorrentDownloadDirectory))
                Directory.CreateDirectory(TorrentDownloadDirectory);

            byte[] bytes = null;
            try
            {
                if (IsMagnetLink)
                    bytes = await TorrentClientEngine.Instance.ClientEngine.DownloadMetadataAsync(MagnetLink.Parse(torrentUri), cancellationToken);
                else
                    bytes = await client.GetByteArrayAsync(torrentUri, cancellationToken);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                    Status = TorrentDownloaderStatus.DownloadFailed;

                return;
            }
            finally
            {
                semaphoreSlim.Release();
            }

            if (bytes != null && bytes.Any())
            {
                File.WriteAllBytes(TorrentFilePath, bytes);
                if (File.Exists(TorrentFilePath))
                {
                    if (!IsTorrentValid())
                        FixTorrentFile();

                    Status = TorrentDownloaderStatus.DownloadCompleted;
                }
            }
            else
                Status = TorrentDownloaderStatus.DownloadFailed;
        }

        private bool IsTorrentValid()
        {
            var bytes = File.ReadAllBytes(TorrentFilePath);
            var signature = new StringBuilder();
            for (int i = 0; i < 11; i++)
            {
                signature.Append(string.Format("{0:X2}", bytes[i]));
            }

            return TORRENT_HEXA_SIGNATURE.Equals(signature.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private void FixTorrentFile()
        {
            var bytes = File.ReadAllBytes(TorrentFilePath);
            var signature = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                signature.Append(string.Format("{0:X2}", bytes[i]));
            }

            var startIndex = signature.ToString().IndexOf(TORRENT_HEXA_SIGNATURE, StringComparison.OrdinalIgnoreCase);

            var newBytes = new byte[bytes.Length - (startIndex / 2)];
            Array.Copy(bytes, (startIndex / 2), newBytes, 0, newBytes.Length);
            File.WriteAllBytes(TorrentFilePath, newBytes);
        }
    }
}
