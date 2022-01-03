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

        public StreamDto GetStream(string torrentUri, int offset, string videoFormat)
        {
            var movieStream = GetMovieStream(torrentUri, videoFormat);

            return movieStream.GetStream(offset);
        }

        public string GetStreamDownloadingState(string torrentUri)
        {
            var movieStream = movieStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (movieStream == null)
                return string.Empty;

            return movieStream.GetDownloadingState();
        }

        private MovieStream GetMovieStream(string torrentUri, string videoFormat)
        {
            foreach (var stream in movieStreams.Where(m => m.TorrentUri != torrentUri))
                stream.PauseDownloadAsync();

            var movieStream = movieStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (movieStream == null)
            {
                movieStream = MovieStream.CreateMovieStream(clientEngine, torrentUri, videoFormat);
                movieStreams.Add(movieStream);
            }

            return movieStream;
        }
    }
}
