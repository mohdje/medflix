using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsHtml
{
    public class YtsHtmlOneUrlProvider : IYtsHtmlUrlProvider
    {
        public string GetImageUrl(string imagePath)
        {
            return GetServiceUrl() + imagePath;
        }

        public string GetLastMoviesByGenreUrl(string genre)
        {
            return GetServiceUrl() + $"/browse-movies/0/all/{genre}/7/year";
        }

        public string GetMovieDetailsUrl(string movieId)
        {
            return GetServiceUrl() + $"/movie/{movieId}";
        }

        public string GetMovieId(string movieLinkUrl)
        {
            return movieLinkUrl.Replace("/movie/", string.Empty).Replace("/", string.Empty);
        }

        public string GetMovieSearchByGenreUrl(string genre, int pageIndex)
        {
            return GetServiceUrl() + $"/browse-movies/0/all/{genre.ToLower()}/0/year?page={pageIndex}";
        }

        public string GetMovieSearchByNameUrl(string name, int pageIndex)
        {
            return GetServiceUrl() + $"/browse-movies/{name}/all/all/0/year?page={pageIndex}";
        }

        public string GetServiceUrl()
        {
            return "https://yts.one";
        }

        public string GetTorrentUrl(string torrentLink)
        {
            return GetServiceUrl() + torrentLink;
        }
    }
}
