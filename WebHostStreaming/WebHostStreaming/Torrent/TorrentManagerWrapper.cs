using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebHostStreaming.Torrent
{
    public class TorrentManagerWrapper
    {
        private readonly ClientEngine clientEngine;
        private TorrentManager torrentManager;
        private TorrentDownloadStatusWatcher torrentDownloadStatusWatcher;
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public IEnumerable<ITorrentFileInfo> Files => torrentManager.Files;

        public ITorrentFileInfo DownloadingFile { get; private set; }

        public bool FoundPeers => torrentDownloadStatusWatcher.FoundPeers;
        public bool DownloadStarted => torrentDownloadStatusWatcher.DownloadStarted;
        public bool DownloadInProgress => torrentDownloadStatusWatcher.DownloadInProgress;

        public static async Task<TorrentManagerWrapper> BuildTorrentManagerWrapperAsync(ClientEngine clientEngine, string torrentFilePath, string torrentDownloadDirectory)
        {
            var torrentManagerWrapper = new TorrentManagerWrapper(clientEngine);
            await torrentManagerWrapper.InitTorrentManagerAsync(torrentFilePath, torrentDownloadDirectory);

            return torrentManagerWrapper;
        }
        private TorrentManagerWrapper(ClientEngine clientEngine)
        {
            this.clientEngine = clientEngine;
        }

        private async Task InitTorrentManagerAsync(string torrentFilePath, string torrentDownloadDirectory)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (torrentManager == null)
                    torrentManager = await clientEngine.AddStreamingAsync(torrentFilePath, torrentDownloadDirectory);

                if (torrentManager != null)
                    torrentDownloadStatusWatcher = new TorrentDownloadStatusWatcher(torrentManager);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        } 

        public async Task StartDownloadFileAsync(Func<ITorrentFileInfo, bool> downloadFilePredicate)
        {
            var currentDownloadingFilePath = DownloadingFile?.FullPath;
            DownloadingFile = torrentManager.Files.FirstOrDefault(f => downloadFilePredicate(f));

            if(DownloadingFile != null)
            {             
                if(currentDownloadingFilePath != DownloadingFile.FullPath)
                {
                    foreach (var file in torrentManager.Files.Where(f => !downloadFilePredicate(f)))
                        await torrentManager.SetFilePriorityAsync(file, Priority.DoNotDownload);

                    await torrentManager.SetFilePriorityAsync(DownloadingFile, Priority.Highest);
                }

                if (torrentManager.State == TorrentState.Paused
                 || torrentManager.State == TorrentState.Stopped
                 || torrentManager.State == TorrentState.Error)
                {
                    await torrentManager.StartAsync();
                    torrentDownloadStatusWatcher.StartWatchingProgress();
                }
            }
        }

        public async Task PauseDownloadAsync()
        {
            torrentDownloadStatusWatcher.StopWatchingProgress();
            await torrentManager.PauseAsync();
        }

        public async Task<Stream> GetStreamAsync(ITorrentFileInfo file, int offset, CancellationToken cancellationToken)
        {
            if (torrentManager.StreamProvider.ActiveStream == null || torrentManager.StreamProvider.ActiveStream.Disposed)
                await torrentManager.StreamProvider.CreateStreamAsync(file, cancellationToken, offset);
            else if (offset > 0)
            {
                torrentManager.StreamProvider.ActiveStream.Seek(offset, SeekOrigin.Begin);
                await torrentManager.StreamProvider.ActiveStream.ReadAsync(new byte[1], 0, 1, cancellationToken);
            }

            return torrentManager.StreamProvider.ActiveStream;
        }

    }
}
