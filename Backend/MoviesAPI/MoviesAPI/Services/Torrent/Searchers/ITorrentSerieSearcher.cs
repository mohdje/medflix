using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    internal interface ITorrentSerieSearcher : ISearcherService
    {
        Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string serieName, string imdbId, int seasonNumber, int episodeNumber);
    }
}
