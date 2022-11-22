using MonoTorrent.Client;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public class TorrentVideoStreamProvider : ITorrentVideoStreamProvider
    {

        List<TorrentVideoStream> torrentVideoStreams;
        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        ClientEngine clientEngine;
        public TorrentVideoStreamProvider()
        {
            clientEngine = CreateEngine();
            torrentVideoStreams = new List<TorrentVideoStream>();
        }
        private ClientEngine CreateEngine()
        {
            EngineSettingsBuilder settingsBuilder = new EngineSettingsBuilder();
            settingsBuilder.AllowedEncryption.Add(EncryptionType.PlainText | EncryptionType.RC4Full | EncryptionType.RC4Header);

            return new ClientEngine(settingsBuilder.ToSettings());
        }
        public DownloadingState GetStreamDownloadingState(string torrentUri)
        {
            var torrentVideoStream = torrentVideoStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (torrentVideoStream == null)
                return new DownloadingState(null);

            return torrentVideoStream.Status;
        }
        public async Task<StreamDto> GetStreamAsync(string torrentUri, int offset, string videoExtension)
        {
            var torrentVideoStream = await GetOrCreateTorrentVideoStreamAsync(torrentUri);

            await PauseInactiveDownloads(torrentVideoStream.TorrentUri);

            if (!torrentVideoStream.IsInitialized)
                await torrentVideoStream.InitAsync();

            return await torrentVideoStream.GetStreamAsync(filePath => MatchVideoFormat(filePath, videoExtension), offset);
        }

        private async Task<TorrentVideoStream> GetOrCreateTorrentVideoStreamAsync(string torrentUri)
        {
            await semaphoreSlim.WaitAsync();
            var torrentVideoStream = torrentVideoStreams.SingleOrDefault(t => t.TorrentUri == torrentUri);

            if (torrentVideoStream == null)
            {
                torrentVideoStream = new TorrentVideoStream(torrentUri, clientEngine);
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

        private bool MatchVideoFormat(string fileName, string videoFormat)
        {
            if (videoFormat == "*")
                return fileName.EndsWith(".mp4") || fileName.EndsWith(".avi") || fileName.EndsWith(".mkv");
            else
                return fileName.EndsWith(videoFormat);
        }

    }
}
