using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using MoviesAPI.Extensions;

namespace MoviesAPI.Services.Torrent
{
    internal class YtsApiSearcher : ITorrentSearcher
    {
        string listMovies = "list_movies.json";

        string Url => "https://yts.bz/api/v2/";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(TorrentRequest torrentRequest)
        {
            var parameters = new NameValueCollection
            {
                { "query_term", torrentRequest.MediaName },
                { "sort_by", "year" },
                { "order_by", "desc" }
            };

            var searchurl = $"{Url}{listMovies}";
            var requestResult = await HttpRequester.GetAsync<YtsResultDto>(searchurl, parameters);

            var movie = requestResult?.Data?.Movies?.FirstOrDefault(m => m.Title.StartsWithIgnoreDiactrics(torrentRequest.MediaName) && m.Year == torrentRequest.Year);

            return movie != null ? movie.Torrents.Select(t => new MediaTorrent() { DownloadUrl = t.Url, Quality = t.Quality }) : Array.Empty<MediaTorrent>();
        }
    }
}
