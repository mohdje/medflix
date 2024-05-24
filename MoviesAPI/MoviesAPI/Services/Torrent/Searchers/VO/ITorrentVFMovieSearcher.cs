using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    internal interface ITorrentVFMovieSearcher : ISearcherService
    {
        Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string originalMovieName, string frenchMovieName, int year);
    }
}
