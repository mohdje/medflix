using MoviesAPI.Services;
using MoviesAPI.Services.Torrent;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApiSample.Samples
{
    internal class TorrentSample
    {
        TorrentSearchManager torrentSearchManager;
        public TorrentSample()
        {
            torrentSearchManager = MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync().Result;
        }

        public async Task Test()
        {
           //await SearchVfMovieTorrents("Very Bad Trip", 2009);
           //await SearchVoMovieTorrents("Hang Over", 2009);
           await SearchVoSerieTorrents("Wednesday", "tt13443470", 1, 1);
        }
        private async Task SearchVfMovieTorrents(string frenchTitleMovie, int year)
        {
            Console.WriteLine($"Search vf torrents for {frenchTitleMovie}, {year}");

            var vfTorrents = await torrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitleMovie, year);

            PrintResult(vfTorrents);
        }

        private async Task SearchVoMovieTorrents(string originalTitle, int year)
        {
            Console.WriteLine($"Search vo torrents for {originalTitle}, {year}");
            var voTorrentSearcher = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            var voTorrents = await voTorrentSearcher.SearchVoTorrentsMovieAsync(originalTitle, year);

            PrintResult(voTorrents);
        }

        private async Task SearchVoSerieTorrents(string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var torrentSerieSearcher = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            var voTorrents = await torrentSerieSearcher.SearchVoTorrentsSerieAsync(serieName, imdbId, seasonNumber, episodeNumber);

            PrintResult(voTorrents);
        }

        private void PrintResult(IEnumerable<MediaTorrent> mediaTorrents)
        {
            if (mediaTorrents == null || !mediaTorrents.Any())
                Console.WriteLine("No torrent found");
            else
            {
                foreach (var torrent in mediaTorrents)
                {
                    Console.WriteLine($"Quality: {torrent.Quality}, Url: {torrent.DownloadUrl}");
                }
            }
        }

    }
}
