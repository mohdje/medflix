using MoviesAPI.Services.Torrent.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent.Searchers
{
    internal interface ITorrentSearcher
    {
        Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string movieName, int year);

        Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string serieName, int seasonNumber, int episodeNumber);
    }
}
