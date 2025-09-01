using MoviesAPI.Services.Torrent.Dtos;
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
            var torrentRequest = new TorrentRequest
            {
                MediaName = frenchMovieName,
                Year = year
            };
            var torrentLinks = await SearchTorrents(vfTorrentMovieSearchers, torrentRequest);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsSerieAsync(string frenchSerieName, int seasonNumber, int episodeNumber)
        {
            var torrentRequest = new TorrentRequest
            {
                MediaName = frenchSerieName,
                SeasonNumber = seasonNumber,
                EpisodeNumber = episodeNumber
            };
            var torrentLinks = await SearchTorrents(vfTorrentSerieSearchers, torrentRequest);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsMovieAsync(string originalMovieName, int year)
        {
            var torrentRequest = new TorrentRequest
            {
                MediaName = originalMovieName,
                Year = year
            };
            var torrentLinks = await SearchTorrents(voTorrentMovieSearchers, torrentRequest);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        public async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsSerieAsync(string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var torrentRequest = new TorrentRequest
            {
                MediaName = serieName,
                SeasonNumber = seasonNumber,
                EpisodeNumber = episodeNumber,
                ImdbId = imdbId
            };
            var torrentLinks = await SearchTorrents(voTorrentSeriesSearchers, torrentRequest);
            return torrentLinks.DistinctBy(t => t.DownloadUrl);
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrents(IEnumerable<ITorrentSearcher> torrentSearchers, TorrentRequest torrentRequest)
        {
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var searcher in torrentSearchers)
            {
                tasks.Add(searcher.GetTorrentLinksAsync(torrentRequest));
            }

            var mediaTorrents = await Task.WhenAll(tasks);

            return mediaTorrents.Where(r => r != null).SelectMany(m => m);
        }
    }
}
