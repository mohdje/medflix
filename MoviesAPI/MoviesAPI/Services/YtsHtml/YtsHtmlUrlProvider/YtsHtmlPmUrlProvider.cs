using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsHtml
{
    public class YtsHtmlPmUrlProvider : IYtsHtmlUrlProvider
    {
        public string GetImageUrl(string imagePath)
        {
            return "https:" + imagePath;
        }

        public string GetLastMoviesByGenreUrl(string genre)
        {
            return GetServiceUrl() + $"/browse-movies/all/all/{genre.ToLower()}/7"; 
        }

        public string GetMovieDetailsUrl(string movieId)
        {
            return GetServiceUrl() + $"/movies/{movieId}";
        }

        public string GetMovieId(string movieLinkUrl)
        {
            return movieLinkUrl.Replace("/movies/", string.Empty);
        }

        public string GetMovieIdFromUrl(string movieDetailsUrl)
        {
            return movieDetailsUrl.Replace("/movies/", string.Empty);
        }

        public string GetMovieSearchByGenreUrl(string genre, int pageIndex)
        {
            return GetServiceUrl() + $"/browse-movies/all/all/{genre.ToLower()}/0/year?page={pageIndex}";
        }

        public string GetMovieSearchByNameUrl(string name, int pageIndex)
        {
            return GetServiceUrl() + $"/browse-movies/{name}/all/all/0/year?page={pageIndex}"; 
        }

        public string GetServiceUrl()
        {
            return "https://yts.pm";
        }

        public string GetTorrentUrl(string torrentLink)
        {
            return GetServiceUrl() + torrentLink;
        }
    }
}
