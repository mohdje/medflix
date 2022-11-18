using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    internal class YtsHtmlRsUrlProvider : IYtsHtmlUrlProvider
    {
        public string GetMovieDetailsUrl(string movieDetailsLink)
        {
            return GetServiceUrl() + movieDetailsLink;
        }

        public string GetMovieSearchByNameUrl(string name, int pageIndex)
        {
            return GetServiceUrl() + $"/browse-movies/{name}/all/all/0/latest?page={pageIndex}";
        }

        public string GetServiceUrl()
        {
            return "https://yts.rs";
        }

        public string GetTorrentUrl(string torrentLink)
        {
            return torrentLink;
        }
    }
}
