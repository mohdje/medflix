using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Torrent
{ 
    public class YtsHtmlOneUrlProvider : IYtsHtmlUrlProvider
    {

        public string GetMovieDetailsUrl(string movieDetailsLink)
        {
            return GetServiceUrl() + movieDetailsLink;
        }

        public string GetMovieSearchByNameUrl(string name, int pageIndex)
        {
            return GetServiceUrl() + $"/browse-movies/{name}/all/all/0/0/year?page={pageIndex}";
        }

        public string GetServiceUrl()
        {
            return "https://yts.do";
        }

        public string GetTorrentUrl(string torrentLink)
        {
            return GetServiceUrl() + torrentLink;
        }
    }
}
