using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class MovieStreamProvider : IMovieStreamProvider
    {
        ClientEngine clientEngine;
        List<MovieStream> movieStreams;

        public MovieStreamProvider()
        {
            clientEngine = CreateEngine();
            movieStreams = new List<MovieStream>();
        }

        private ClientEngine CreateEngine()
        {
            EngineSettingsBuilder settingsBuilder = new EngineSettingsBuilder();
            settingsBuilder.AllowedEncryption.Add(EncryptionType.PlainText | EncryptionType.RC4Full | EncryptionType.RC4Header);

            return new ClientEngine(settingsBuilder.ToSettings());
        }

        public async Task<StreamDto> GetStreamAsync(string torrentUri, int offset, string videoFormat)
        {
            var movieStream = await GetMovieStreamAsync(torrentUri, videoFormat);

            await PauseInactiveDownloads(torrentUri);

            return await movieStream.GetStreamAsync(offset);
        }

        public DownloadingState GetStreamDownloadingState(string torrentUri)
        {
            var movieStream = movieStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (movieStream == null)
                return new DownloadingState();

            return movieStream.GetDownloadingState();
        }

        private async Task PauseInactiveDownloads(string activeTorrentUri)
        {
            var tasks = new List<Task>();
            foreach (var stream in movieStreams.Where(m => m.TorrentUri != activeTorrentUri))
                tasks.Add(stream.PauseDownloadAsync());

            await Task.WhenAll(tasks);
        }

        private async Task<MovieStream> GetMovieStreamAsync(string torrentUri, string videoFormat)
        {
            var movieStream = movieStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (movieStream == null)
            {
                movieStream = new MovieStream(torrentUri, videoFormat, clientEngine);
                movieStreams.Add(movieStream);
            }
            else if (movieStream.VideoFormat != videoFormat)
                movieStream.UpdateVideoFormat(videoFormat);

            return movieStream;
        }
    }
}
