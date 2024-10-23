using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Torrent
{
    public class YtsHtmlMxUrlProvider : IYtsHtmlUrlProvider
    {


        public string GetMovieDetailsUrl(string movieDetailsLink)
        {
            return movieDetailsLink;
        }

        public string GetMovieSearchByNameUrl(string name, int pageIndex)
        {
            var pageParameter = pageIndex == 1 ? string.Empty : $"?page={pageIndex}";
            return GetServiceUrl() + $"/browse-movies/{name}/all/all/0/year/0/all{pageParameter}";
        }

        public string GetServiceUrl()
        {
            return "https://yts.mx";
        }

        public string GetTorrentUrl(string torrentLink)
        {
            return torrentLink;
        }

    }
}
