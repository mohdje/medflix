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
        private TorrentManager torrentManager;
        private TorrentDownloadStatusWatcher torrentDownloadStatusWatcher;

        public IEnumerable<ITorrentFileInfo> Files => torrentManager.Files;

        public ITorrentFileInfo DownloadingFile { get; private set; }

        public bool FoundPeers => torrentDownloadStatusWatcher.FoundPeers;
        public bool DownloadStarted => torrentDownloadStatusWatcher.DownloadStarted;
        public bool DownloadInProgress => torrentDownloadStatusWatcher.DownloadInProgress;

        public static async Task<TorrentManagerWrapper> BuildTorrentManagerWrapperAsync(string torrentFilePath, string torrentDownloadDirectory)
        {
            var torrentManagerWrapper = new TorrentManagerWrapper();
            await torrentManagerWrapper.InitTorrentManagerAsync(torrentFilePath, torrentDownloadDirectory);

            return torrentManagerWrapper;
        }

        private async Task InitTorrentManagerAsync(string torrentFilePath, string torrentDownloadDirectory)
        {
            if (torrentManager == null)
                torrentManager = await TorrentClientEngine.Instance.GetTorrentManagerAsync(torrentFilePath, torrentDownloadDirectory);

            if (torrentManager != null)
                torrentDownloadStatusWatcher = new TorrentDownloadStatusWatcher(torrentManager);            
        } 

        public async Task StartDownloadFileAsync(ITorrentFileSelector torrentFileSelector)
        {
            var currentDownloadingFilePath = DownloadingFile?.FullPath;
            DownloadingFile = torrentFileSelector.SelectTorrentFileInfo(torrentManager.Files);

            if(DownloadingFile != null)
            {             
                if(currentDownloadingFilePath != DownloadingFile.FullPath)
                {
                    foreach (var file in torrentManager.Files.Where(f => DownloadingFile.FullPath != f.FullPath))
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
