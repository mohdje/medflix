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
        private readonly IEnumerable<ITorrentVFMovieSearcher> vfTorrentMovieSearchers;
        private readonly IEnumerable<ITorrentVOMovieSearcher> voTorrentMovieSearchers;

        private readonly IEnumerable<ITorrentSerieSearcher> vfTorrentSerieSearchers;
        private readonly IEnumerable<ITorrentSerieSearcher> voTorrentSeriesSearchers;
        internal TorrentSearchManager(
            IEnumerable<ITorrentVFMovieSearcher> vfTorrentMovieSearchers, 
            IEnumerable<ITorrentVOMovieSearcher> voTorrentMovieSearchers,
            IEnumerable<ITorrentSerieSearcher> vfTorrentSerieSearchers,
            IEnumerable<ITorrentSerieSearcher> voTorrentSeriesSearchers)
        {
            this.vfTorrentMovieSearchers = vfTorrentMovieSearchers;
            this.voTorrentMovieSearchers = voTorrentMovieSearchers;
            this.vfTorrentSerieSearchers = vfTorrentSerieSearchers;
            this.voTorrentSeriesSearchers = voTorrentSeriesSearchers;
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsMovieAsync(string originalMovieName, string frenchMovieName, int year)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in vfTorrentMovieSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(originalMovieName, frenchMovieName, year));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            var torrentLinks = movieTorrents.Where(r => r != null).SelectMany(m => m);

            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsSerieAsync(string frenchSerieName, int seasonNumber, int episodeNumber)
        {
            var torrentLinks = await SearchTorrentsSerie(vfTorrentSerieSearchers, frenchSerieName, null, seasonNumber, episodeNumber);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsMovieAsync(string originalMovieName, int year)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in voTorrentMovieSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(originalMovieName, year));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            var torrentLinks = movieTorrents.Where(r => r != null).SelectMany(m => m);

            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsSerieAsync(string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var torrentLinks = await SearchTorrentsSerie(voTorrentSeriesSearchers, serieName, imdbId, seasonNumber, episodeNumber);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentsMovie(IEnumerable<ITorrentVOMovieSearcher> torrentSearchers, string movieName, int year)
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
