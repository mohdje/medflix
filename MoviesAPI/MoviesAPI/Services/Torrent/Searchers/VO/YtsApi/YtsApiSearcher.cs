using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Services.VOMovies.YtsApi.DTOs;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Movies.Dtos;
using MoviesAPI.Services.Torrent.Dtos;

namespace MoviesAPI.Services.Torrent
{
    public class YtsApiSearcher : ITorrentSearcher
    {
        IYtsApiUrlProvider ytsApiUrlProvider;

        private string listMovies = "list_movies.json";

        internal YtsApiSearcher(IYtsApiUrlProvider ytsApiUrlProvider)
        {
            this.ytsApiUrlProvider = ytsApiUrlProvider;
        }

        public async Task<IEnumerable<MovieTorrent>> GetTorrentLinksAsync(string movieName, int year)
        {
            var parameters = new NameValueCollection();
            parameters.Add("query_term", movieName);
            parameters.Add("sort_by", "year");
            parameters.Add("order_by", "desc");

            var requestResult = await HttpRequester.GetAsync<YtsResultDto>(ytsApiUrlProvider.GetBaseApiUrl() + listMovies, parameters);

            var movie = requestResult?.Data?.Movies?.FirstOrDefault(m => m.Title.Equals(movieName, StringComparison.OrdinalIgnoreCase) && m.Year == year);
              
            return movie != null ? movie.Torrents.Select(t => new MovieTorrent() { DownloadUrl = t.Url, Quality = t.Quality }) : new MovieTorrent[0];
        }

        public string GetPingUrl()
        {
            return this.ytsApiUrlProvider.GetPingUrl();
        }
    }
}
