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

        public Stream GetStream(string torrentUri, int offset)
        {
            var movieStream = GetMovieStream(torrentUri);

            return movieStream.GetStream(offset);
        }

        public string GetStreamDownloadingState(string torrentUri)
        {
            var movieStream = movieStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (movieStream == null)
                return string.Empty;

            return movieStream.GetDownloadingState();
        }

        private MovieStream GetMovieStream(string torrentUri)
        {
            foreach (var stream in movieStreams.Where(m => m.TorrentUri != torrentUri))
                stream.PauseDownloadAsync();

            var movieStream = movieStreams.SingleOrDefault(m => m.TorrentUri == torrentUri);

            if (movieStream == null)
            {
                movieStream = new MovieStream(clientEngine, torrentUri);
                movieStreams.Add(movieStream);
            }

            return movieStream;
        }
    }
}
