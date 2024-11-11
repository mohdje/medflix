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
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Torrent
{
    public enum TorrentDownloaderStatus
    {
        DownloadHasStarted,
        DownloadFailed,
        DownloadCompleted,
        DownloadAborted
    }
    public class TorrentDownloader
    {
        private string torrentUri;
        private CancellationTokenSource cancellationTokenSource;
        private static HttpClient client = new HttpClient();

        private const string TORRENT_HEXA_SIGNATURE = "64383a616e6e6f756e6365";

        private bool IsMagnetLink => torrentUri.StartsWith("magnet:?xt=urn:btih");

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public string TorrentDownloadDirectory => torrentUri?.ToTorrentFolderPath();
        public string TorrentFilePath => Path.Combine(TorrentDownloadDirectory, "torrent");

        public TorrentDownloaderStatus Status { get; private set; }

        public async Task<string> DownloadTorrentFileAsync(string torrentUri, ClientEngine clientEngine)
        {
            this.cancellationTokenSource?.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();

            this.torrentUri = torrentUri;

            if (File.Exists(TorrentFilePath))
            {
                Status = TorrentDownloaderStatus.DownloadCompleted;
                return TorrentFilePath;
            }

            if (!Directory.Exists(TorrentDownloadDirectory))
                Directory.CreateDirectory(TorrentDownloadDirectory);

            byte[] bytes = null;
            try
            {
                AppLogger.LogInfo($"Downloading torrent : {torrentUri}");
                Status = TorrentDownloaderStatus.DownloadHasStarted;

                if (IsMagnetLink)
                {
                    var metadata = await clientEngine.DownloadMetadataAsync(MagnetLink.Parse(torrentUri), cancellationTokenSource.Token);
                    bytes = metadata.ToArray();
                }
                else
                    bytes = await client.GetByteArrayAsync(torrentUri, cancellationTokenSource.Token);

                if (bytes != null && bytes.Any())
                {
                    File.WriteAllBytes(TorrentFilePath, bytes);

                    if (!IsTorrentValid())
                        FixTorrentFile();

                    Status = TorrentDownloaderStatus.DownloadCompleted;

                    AppLogger.LogInfo($"Torrent successfully downloaded: {torrentUri}");

                    return TorrentFilePath;
                }
                else
                {
                    AppLogger.LogInfo($"Torrent download failed: {torrentUri}");

                    Status = TorrentDownloaderStatus.DownloadFailed;
                    return null;
                }
            }
            catch (Exception ex)
            {
                if (!this.cancellationTokenSource.Token.IsCancellationRequested)
                    Status = TorrentDownloaderStatus.DownloadFailed;
                else
                    AppLogger.LogInfo($"Torrent download aborted: {torrentUri}");

                return null;
            }
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
