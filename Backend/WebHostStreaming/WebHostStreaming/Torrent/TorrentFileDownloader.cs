using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
        DownloadNotStarted,
        DownloadHasStarted,
        DownloadFailed,
        DownloadCompleted,
        DownloadAborted
    }
    public class TorrentFileDownloader
    {
        private readonly ClientEngine clientEngine;
        private readonly string clientAppIdentifier;

        public string TorrentUri { get; }

        private const string TORRENT_HEXA_SIGNATURE = "64383a616e6e6f756e6365";
        private bool IsMagnetLink => TorrentUri.StartsWith("magnet:?xt=urn:btih");
        private string TorrentFilePath => Path.Combine(TorrentDownloadDirectory, "torrent");

        public string TorrentDownloadDirectory => TorrentUri?.ToTorrentFolderPath(clientAppIdentifier);

        public TorrentDownloaderStatus Status { get; private set; }

        public TorrentFileDownloader(string clientAppIdentifier, string torrentUri, ClientEngine clientEngine)
        {
            TorrentUri = torrentUri;
            this.clientEngine = clientEngine;
            this.clientAppIdentifier = clientAppIdentifier;
            Status = TorrentDownloaderStatus.DownloadNotStarted;
        }


        public async Task<string> DownloadAsync(CancellationToken cancellationToken)
        {
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
                AppLogger.LogInfo(clientAppIdentifier, $"Downloading torrent : {TorrentUri}");
                Status = TorrentDownloaderStatus.DownloadHasStarted;

                if (IsMagnetLink)
                {
                    var metadata = await clientEngine.DownloadMetadataAsync(MagnetLink.Parse(TorrentUri), cancellationToken);
                    bytes = metadata.ToArray();
                }
                else
                {
                    using (var httpClient = new HttpClient())
                        bytes = await httpClient.GetByteArrayAsync(TorrentUri, cancellationToken);
                }

                if (bytes != null && bytes.Any())
                {
                    File.WriteAllBytes(TorrentFilePath, bytes);

                    if (!IsTorrentValid())
                        FixTorrentFile();

                    Status = TorrentDownloaderStatus.DownloadCompleted;

                    AppLogger.LogInfo(clientAppIdentifier, $"Torrent successfully downloaded: {TorrentUri}");

                    return TorrentFilePath;
                }
                else
                {
                    AppLogger.LogInfo(clientAppIdentifier, $"Torrent download failed: {TorrentUri}");

                    Status = TorrentDownloaderStatus.DownloadFailed;

                    if (File.Exists(TorrentFilePath))
                        File.Delete(TorrentFilePath);

                    return null;
                }
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                    AppLogger.LogInfo(clientAppIdentifier, $"Torrent download aborted: {TorrentUri}");
                else
                {
                    AppLogger.LogInfo(clientAppIdentifier, $"Torrent download failed: {TorrentUri}");
                    Status = TorrentDownloaderStatus.DownloadFailed;
                }

                if (File.Exists(TorrentFilePath))
                    File.Delete(TorrentFilePath);

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
