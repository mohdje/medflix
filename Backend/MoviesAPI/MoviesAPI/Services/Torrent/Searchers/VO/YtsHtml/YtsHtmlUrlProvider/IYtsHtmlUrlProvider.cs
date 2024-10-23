using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Torrent
{
    public interface IYtsHtmlUrlProvider
    {
        string GetServiceUrl();
        string GetMovieSearchByNameUrl(string name, int pageIndex);

        string GetMovieDetailsUrl(string movieDetailsLink);

        string GetTorrentUrl(string torrentLink);
    }
}
