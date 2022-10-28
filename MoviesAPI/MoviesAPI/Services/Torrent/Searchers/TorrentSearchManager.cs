using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent.Searchers
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
            return await SearchTorrents(vfTorrentSearchers, frenchMovieName, year);
        }

        public async Task<IEnumerable<MovieTorrent>> SearchVoTorrentsAsync(string originalMovieName, int year)
        {
            return await SearchTorrents(voTorrentSearchers, originalMovieName, year);
        }

        private async Task<IEnumerable<MovieTorrent>> SearchTorrents(IEnumerable<ITorrentSearcher> torrentSearchers, string movieName, int year)
        {
            var tasks = new List<Task<IEnumerable<MovieTorrent>>>();

            foreach (var searcher in torrentSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(movieName, year));
            }

            var movieTorrents = await Task.WhenAll(tasks);

            return movieTorrents.SelectMany(m => m);
        }
    }
}
