using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsHtml
{
    public class YtsHtmlLtdUrlProvider : IYtsHtmlUrlProvider
    {
        public string GetImageUrl(string imagePath)
        {
            return imagePath.StartsWith("http") ? imagePath : GetServiceUrl() + imagePath;
        }

        public string GetLastMoviesByGenreUrl(string genre)
        {
            return GetServiceUrl() + $"/browse-movies/0/all/{genre.ToLower()}/7/year/0/all"; 
        }

        public string GetMovieDetailsUrl(string movieId)
        {
            return GetServiceUrl() + $"/movies/{movieId}";
        }

        public string GetMovieId(string movieLinkUrl)
        {
            return movieLinkUrl.Replace(GetServiceUrl() + "/movies/", string.Empty);
        }

        public string GetMovieSearchByGenreUrl(string genre, int pageIndex)
        {
            var pageParameter = pageIndex == 1 ? string.Empty : $"?page={pageIndex}";
            return GetServiceUrl() + $"/browse-movies/0/all/{genre.ToLower()}/0/year/0/all{pageParameter}";
        }

        public string GetMovieSearchByNameUrl(string name, int pageIndex)
        {
            var pageParameter = pageIndex == 1 ? string.Empty : $"?page={pageIndex}";
            return GetServiceUrl() + $"/browse-movies/{name}/all/all/0/year/0/all{pageParameter}"; 
        }

        public string GetServiceUrl()
        {
            return "https://yts.unblockit.tv";
        }

        public string GetSuggestedMoviesUrl()
        {
            throw new NotImplementedException();
        }

        public string GetTorrentUrl(string torrentLink)
        {
            return torrentLink;
        }
    }
}
