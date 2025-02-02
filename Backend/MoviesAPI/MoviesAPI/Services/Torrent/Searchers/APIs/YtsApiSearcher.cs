using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using MoviesAPI.Services.Torrent.Searchers;

namespace MoviesAPI.Services.Torrent
{
    public class YtsApiSearcher : ITorrentSearcher
    {
        string listMovies = "list_movies.json";

        string Url => "https://yts.mx/api/v2/";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string movieName, int year)
        {
            var parameters = new NameValueCollection
            {
                { "query_term", movieName },
                { "sort_by", "year" },
                { "order_by", "desc" }
            };

            var searchurl = $"{Url}{listMovies}";
            var requestResult = await HttpRequester.GetAsync<YtsResultDto>(searchurl, parameters);

            var movie = requestResult?.Data?.Movies?.FirstOrDefault(m => m.Title.Replace("\'", String.Empty).Equals(movieName.Replace("\'", String.Empty), StringComparison.OrdinalIgnoreCase) && m.Year == year);
              
            return movie != null ? movie.Torrents.Select(t => new MediaTorrent() { DownloadUrl = t.Url, Quality = t.Quality }) : Array.Empty<MediaTorrent>();
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string serieName, int seasonNumber, int episodeNumber)
        {
            return await Task.FromResult(Array.Empty<MediaTorrent>());
        }
    }
}
