using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;

namespace MoviesAPI.Services.Torrent
{

    public class YtsHtmlSearcher : ITorrentSearcher
    {
        IYtsHtmlUrlProvider htmlUrlProvider;

        internal YtsHtmlSearcher(IYtsHtmlUrlProvider ytsHtmlUrlProvider)
        {
            htmlUrlProvider = ytsHtmlUrlProvider;
        }

        public async Task<IEnumerable<MovieTorrent>> GetTorrentLinksAsync(string movieName, int year)
        {
            var pageIndex = 0;
            bool keepSearch = true;

            var moviesSearchResult = new List<MovieTorrent>();

            while (keepSearch)
            {
                pageIndex++;

                var doc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetMovieSearchByNameUrl(movieName, pageIndex));

                var movieDtos = await GetMovieTorrentsAsync(doc.DocumentNode, movieName, year);

                if (movieDtos.Any())
                    moviesSearchResult.AddRange(movieDtos);
                else
                    keepSearch = false;
            }

            return moviesSearchResult;
        }

        private async Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(HtmlNode documentNode, string title, int year)
        {
            var movies = documentNode.SelectNodes("//div[contains(@class, 'browse-movie-wrap')]");

            if (movies == null)
                return new MovieTorrent[0];

            var movieDtos = new List<MovieTorrent>();
            var tasks = new List<Task<IEnumerable<MovieTorrent>>>();
            foreach (var movie in movies)
            {
                tasks.Add(GetMovieTorrentsAsync(movie.InnerHtml, title, year));
            }

            await Task.WhenAll(tasks).ContinueWith(t => movieDtos.AddRange(t.Result.SelectMany(r => r)));

            return movieDtos;
        }

        private async Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(string movieHtml, string title, int year)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(movieHtml);

            var movieTitle = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'browse-movie-title')]")?.InnerText.HtmlUnescape();
            int.TryParse(doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'browse-movie-year')]")?.InnerText, out var movieYear);

            if (movieTitle.RemoveSpecialCharacters().Equals(title.RemoveSpecialCharacters(), StringComparison.OrdinalIgnoreCase) && movieYear == year)
            {
                var movieDetailsLink = htmlUrlProvider.GetMovieDetailsUrl(doc.DocumentNode.SelectSingleNode("/*[contains(@class, 'browse-movie-link')]")?.Attributes["href"].Value);
                return await GetMovieTorrentsAsync(movieDetailsLink);
            }
            else
                return new MovieTorrent[0];
        }

        private async Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(string movieDetailsLink)
        {
            if (string.IsNullOrEmpty(movieDetailsLink))
                return new MovieTorrent[0];

            var doc = await HttpRequester.GetHtmlDocumentAsync(movieDetailsLink);

            var movieTorrents = doc.DocumentNode.SelectSingleNode("//p/em[contains(text(), 'Available in')]").ParentNode;
            var movieTorrentsHtml = new HtmlAgilityPack.HtmlDocument();
            movieTorrentsHtml.LoadHtml(movieTorrents.InnerHtml);

            return movieTorrentsHtml.DocumentNode.SelectNodes("//a[contains(@title, 'torrent') or contains(@title, 'Torrent')]")
                                                        ?.Select(n => new MovieTorrent()
                                                        {
                                                            DownloadUrl = htmlUrlProvider.GetTorrentUrl(n.Attributes["href"].Value),
                                                            Quality = n.InnerText
                                                        });
        }

        public string GetPingUrl()
        {
            return htmlUrlProvider.GetServiceUrl();
        }
    }
}
