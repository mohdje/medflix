using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;

namespace WebHostStreaming.Torrent
{
    public class TorrentStream : IDisposable
    {
        private TorrentManager torrentManager;
        private ITorrentManagerFile mediaFileToStream;
        private Stream stream;

        private bool streamCreationOnGoing;
        public string MediaFileContentType => mediaFileToStream.FullPath.GetContentType();
        public string MediaFileName => Path.GetFileName(mediaFileToStream.FullPath);
        public TorrentStream(TorrentManager torrentManager, ITorrentManagerFile mediaFileToStream)
        {
            this.torrentManager = torrentManager;
            this.mediaFileToStream = mediaFileToStream;
        }
        public void Dispose()
        {
            stream?.Close();
            stream?.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (mediaFileToStream == null)
                throw new ArgumentNullException("Media file to stream is not set");

            if (stream == null)
            {
                streamCreationOnGoing = true;
                stream = await torrentManager.StreamProvider.CreateStreamAsync(mediaFileToStream, true, cancellationToken);
                streamCreationOnGoing = false;
            }
        }

        public async Task<Stream> GetStreamAsync()
        {
            if (stream == null && !streamCreationOnGoing)
                throw new Exception("TorrentStream not started");

            while(streamCreationOnGoing)
                await Task.Delay(1000);

            //try to seek because ObjectDisposedException is thwrown sometimes 
            try
            {
                stream.Position = 0;
            }
            catch (ObjectDisposedException)
            {
                stream = null;
                await StartAsync(CancellationToken.None);
            }

            return stream;
        }
    }
}
       

