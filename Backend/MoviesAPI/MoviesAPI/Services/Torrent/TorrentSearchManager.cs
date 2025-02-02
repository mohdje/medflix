using MoviesAPI.Services.Torrent.Dtos;
using MoviesAPI.Services.Torrent.Searchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    public class TorrentSearchManager
    {
        private readonly IEnumerable<ITorrentSearcher> vfTorrentMovieSearchers;
        private readonly IEnumerable<ITorrentSearcher> voTorrentMovieSearchers;

        private readonly IEnumerable<ITorrentSearcher> vfTorrentSerieSearchers;
        private readonly IEnumerable<ITorrentSearcher> voTorrentSeriesSearchers;
        internal TorrentSearchManager(
            IEnumerable<ITorrentSearcher> vfTorrentMovieSearchers, 
            IEnumerable<ITorrentSearcher> voTorrentMovieSearchers,
            IEnumerable<ITorrentSearcher> vfTorrentSerieSearchers,
            IEnumerable<ITorrentSearcher> voTorrentSeriesSearchers)
        {
            this.vfTorrentMovieSearchers = vfTorrentMovieSearchers;
            this.voTorrentMovieSearchers = voTorrentMovieSearchers;
            this.vfTorrentSerieSearchers = vfTorrentSerieSearchers;
            this.voTorrentSeriesSearchers = voTorrentSeriesSearchers;
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsMovieAsync(string frenchMovieName, int year)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in vfTorrentMovieSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(frenchMovieName, year));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            var torrentLinks = movieTorrents.Where(r => r != null).SelectMany(m => m);

            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsSerieAsync(string frenchSerieName, int seasonNumber, int episodeNumber)
        {
            var torrentLinks = await SearchTorrentsSerie(vfTorrentSerieSearchers, frenchSerieName, seasonNumber, episodeNumber);
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

        public async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsSerieAsync(string serieName, int seasonNumber, int episodeNumber)
        {
            var torrentLinks = await SearchTorrentsSerie(voTorrentSeriesSearchers, serieName, seasonNumber, episodeNumber);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentsMovie(IEnumerable<ITorrentSearcher> torrentSearchers, string movieName, int year)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in torrentSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(movieName, year));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            return movieTorrents.Where(r => r != null).SelectMany(m => m);
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentsSerie(IEnumerable<ITorrentSearcher> torrentSearchers, string serieName, int seasonNumber, int episodeNumber)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in torrentSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(serieName, seasonNumber, episodeNumber));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            return movieTorrents.Where(r => r != null).SelectMany(m => m);
        }
    }
}
