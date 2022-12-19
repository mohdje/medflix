using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MoviesAPI.Services.Torrent
{
    public class TorrentSearchManager
    {
        private readonly IEnumerable<ITorrentSearcher> vfTorrentSearchers;
        private readonly IEnumerable<ITorrentSearcher> voTorrentSearchers;
        internal TorrentSearchManager(IEnumerable<ITorrentSearcher> vfTorrentSearchers, IEnumerable<ITorrentSearcher> voTorrentSearchers)
        {
            this.vfTorrentSearchers = vfTorrentSearchers;
            this.voTorrentSearchers = voTorrentSearchers;
        }

        public async Task<IEnumerable<MovieTorrent>> SearchVfTorrentsAsync(string frenchMovieName, int year)
        {
            var torrentLinks = await SearchTorrents(vfTorrentSearchers, frenchMovieName, year);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MovieTorrent>> SearchVoTorrentsAsync(string originalMovieName, int year)
        {
            var torrentLinks = await SearchTorrents(voTorrentSearchers, originalMovieName, year);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        private async Task<IEnumerable<MovieTorrent>> SearchTorrents(IEnumerable<ITorrentSearcher> torrentSearchers, string movieName, int year)
        {
            var tasks = new List<Task<IEnumerable<MovieTorrent>>>();

            foreach (var searcher in torrentSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(movieName, year));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            return movieTorrents.Where(r => r != null).SelectMany(m => m);
        }
    }
}
