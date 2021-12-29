using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsHtml
{
    public interface IYtsHtmlUrlProvider
    {
        string GetServiceUrl();

        string GetSuggestedMoviesUrl();

        string GetMovieSearchByGenreUrl(string genre, int pageIndex);

        string GetMovieSearchByNameUrl(string name, int pageIndex);

        string GetLastMoviesByGenreUrl(string genre);

        string GetMovieDetailsUrl(string movieId);

        string GetImageUrl(string imagePath);

        string GetMovieId(string movieLinkUrl);

        string GetTorrentUrl(string torrentLink);
    }
}
