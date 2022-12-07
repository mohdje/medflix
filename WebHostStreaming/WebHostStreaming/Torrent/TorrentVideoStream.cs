using MonoTorrent.Client;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Torrent
{
    public class TorrentVideoStream
    {
        private readonly TorrentDownloader torrentDownloader;
        private TorrentManagerWrapper torrentManagerWrapper;
        private CancellationTokenSource cancellationTokenSource;

        public string TorrentUri { get; }
        public DownloadingState Status => GetStatus();
        public bool IsInitialized => torrentManagerWrapper != null;
        public TorrentVideoStream(string torrentUri)
        {
            torrentDownloader = new TorrentDownloader(torrentUri);
            TorrentUri = torrentUri;
        }

        public async Task InitAsync()
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                if (torrentDownloader.Status != TorrentDownloaderStatus.DownloadCompleted)
                    await torrentDownloader.DownloadTorrentFileAsync(cancellationTokenSource.Token);

                if (torrentDownloader.Status == TorrentDownloaderStatus.DownloadCompleted && torrentManagerWrapper == null)
                    torrentManagerWrapper = await TorrentManagerWrapper.BuildTorrentManagerWrapperAsync(torrentDownloader.TorrentFilePath, torrentDownloader.TorrentDownloadDirectory);
            }
            catch(Exception ex)
            {
                torrentManagerWrapper = null;
            }
        }

        public async Task PauseAsync()
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            if (torrentManagerWrapper != null)
            {
                await torrentManagerWrapper.PauseDownloadAsync();
            }
        }

        public async Task<StreamDto> GetStreamAsync(Func<string, bool> filePathSelectionPredicate, int offset)
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            if (torrentManagerWrapper == null)
                return null;

            await torrentManagerWrapper.StartDownloadFileAsync(f => filePathSelectionPredicate(f?.FullPath));

            if (torrentManagerWrapper.DownloadingFile == null)
                return null;

            var stream = await torrentManagerWrapper.GetStreamAsync(torrentManagerWrapper.DownloadingFile, offset, cancellationTokenSource.Token);
            return new StreamDto(stream, Path.GetExtension(torrentManagerWrapper.DownloadingFile.FullPath));
        }

        private DownloadingState GetStatus()
        {
            if (torrentManagerWrapper != null && torrentManagerWrapper.DownloadInProgress)
                return new DownloadingState("Download in progress, video ready to play soon (5/5)");
            else if (torrentManagerWrapper != null && torrentManagerWrapper.DownloadStarted)
                return new DownloadingState("Download has started (4/5)");
            else if (torrentManagerWrapper != null && torrentManagerWrapper.FoundPeers)
                return new DownloadingState("Download is about to start (3/5)");
            else if (torrentManagerWrapper != null && torrentManagerWrapper.DownloadingFile == null)
                return new DownloadingState("No supported video file found", true);
            else if (torrentManagerWrapper != null)
                return new DownloadingState("Resources found (2/5)");
            else if (torrentDownloader.Status == TorrentDownloaderStatus.DownloadHasStarted)
                return new DownloadingState("Searching resources (1/5)");
            else if (torrentDownloader.Status == TorrentDownloaderStatus.DownloadFailed)
                return new DownloadingState("Downloading resources failed", true);
            else if (torrentManagerWrapper == null)
                return new DownloadingState("Error trying to open resources", true);
            else
                return new DownloadingState("Loading");
        }


    }
}
