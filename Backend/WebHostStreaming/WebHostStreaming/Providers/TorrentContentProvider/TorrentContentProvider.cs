using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public class TorrentContentProvider : ITorrentContentProvider
    {

        List<TorrentVideoStream> torrentVideoStreams;
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public TorrentContentProvider()
        {
            torrentVideoStreams = new List<TorrentVideoStream>();
        }
       
        public DownloadingState GetStreamDownloadingState(string torrentUri)
        {
            var torrentVideoStream = torrentVideoStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (torrentVideoStream == null)
                return new DownloadingState(null);

            return torrentVideoStream.Status;
        }
        public async Task<StreamDto> GetStreamAsync(string torrentUri, int offset, ITorrentFileSelector torrentFileSelector)
        {
            var torrentVideoStream = await GetOrCreateTorrentVideoStreamAsync(torrentUri);

            await PauseInactiveDownloads(torrentVideoStream.TorrentUri);

            if (!torrentVideoStream.IsInitialized)
                await torrentVideoStream.InitAsync();

            return await torrentVideoStream.GetStreamAsync(torrentFileSelector, offset);
        }

        public async Task<IEnumerable<string>> GetTorrentFilesAsync(string torrentUri)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var torrentDownloader = new TorrentDownloader(torrentUri);

            await torrentDownloader.DownloadTorrentFileAsync(cancellationTokenSource.Token);

            if (torrentDownloader.Status != TorrentDownloaderStatus.DownloadCompleted)
                return null;

            var torrentManager = await TorrentClientEngine.Instance.GetTorrentManagerAsync(torrentDownloader.TorrentFilePath, torrentDownloader.TorrentDownloadDirectory);

            return torrentManager?.Files.Select(f => f.Path);
        }

        private async Task<TorrentVideoStream> GetOrCreateTorrentVideoStreamAsync(string torrentUri)
        {
            await semaphoreSlim.WaitAsync();
            var torrentVideoStream = torrentVideoStreams.SingleOrDefault(t => t.TorrentUri == torrentUri);

            if (torrentVideoStream == null)
            {
                torrentVideoStream = new TorrentVideoStream(torrentUri);
                torrentVideoStreams.Add(torrentVideoStream);
            }

            semaphoreSlim.Release();

            return torrentVideoStream;
        }


        private async Task PauseInactiveDownloads(string activeTorrentUri)
        {
            var tasks = new List<Task>();
            foreach (var stream in torrentVideoStreams.Where(t => t.TorrentUri != activeTorrentUri))
                tasks.Add(stream.PauseAsync());

            await Task.WhenAll(tasks);
        }
    }
}
