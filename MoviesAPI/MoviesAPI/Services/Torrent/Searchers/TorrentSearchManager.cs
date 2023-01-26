using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    public class TorrentSearchManager
    {
        private readonly IEnumerable<ITorrentMovieSearcher> vfTorrentMovieSearchers;
        private readonly IEnumerable<ITorrentMovieSearcher> voTorrentMovieSearchers;

        private readonly IEnumerable<ITorrentSerieSearcher> vfTorrentSerieSearchers;
        private readonly IEnumerable<ITorrentSerieSearcher> voTorrentSeriesSearchers;
        internal TorrentSearchManager(
            IEnumerable<ITorrentMovieSearcher> vfTorrentMovieSearchers, 
            IEnumerable<ITorrentMovieSearcher> voTorrentMovieSearchers,
            IEnumerable<ITorrentSerieSearcher> vfTorrentSerieSearchers,
            IEnumerable<ITorrentSerieSearcher> voTorrentSeriesSearchers)
        {
            this.vfTorrentMovieSearchers = vfTorrentMovieSearchers;
            this.voTorrentMovieSearchers = voTorrentMovieSearchers;
            this.vfTorrentSerieSearchers = vfTorrentSerieSearchers;
            this.voTorrentSeriesSearchers = voTorrentSeriesSearchers;
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsMovieAsync(string frenchMovieName, int year)
        {
            var torrentLinks = await SearchTorrentsMovie(vfTorrentMovieSearchers, frenchMovieName, year);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsMovieAsync(string originalMovieName, int year)
        {
            var torrentLinks = await SearchTorrentsMovie(voTorrentMovieSearchers, originalMovieName, year);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsSerieAsync(string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var torrentLinks = await SearchTorrentsSerie(voTorrentSeriesSearchers, serieName, imdbId, seasonNumber, episodeNumber);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentsMovie(IEnumerable<ITorrentMovieSearcher> torrentSearchers, string movieName, int year)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in torrentSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(movieName, year));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            return movieTorrents.Where(r => r != null).SelectMany(m => m);
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentsSerie(IEnumerable<ITorrentSerieSearcher> torrentSearchers, string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in torrentSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(serieName, imdbId, seasonNumber, episodeNumber));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            return movieTorrents.Where(r => r != null).SelectMany(m => m);
        }
    }
}
