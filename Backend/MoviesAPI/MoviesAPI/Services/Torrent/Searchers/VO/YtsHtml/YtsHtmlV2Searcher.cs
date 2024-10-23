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

    public class YtsHtmlV2Searcher : ITorrentVOMovieSearcher
    {
        IYtsHtmlUrlProvider htmlUrlProvider;

        public string Url => htmlUrlProvider.GetServiceUrl();

        internal YtsHtmlV2Searcher(IYtsHtmlUrlProvider ytsHtmlUrlProvider)
        {
            htmlUrlProvider = ytsHtmlUrlProvider;
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string movieName, int year)
        {
            var pageIndex = 0;
            bool keepSearch = true;

            var moviesSearchResult = new List<MediaTorrent>();

            while (keepSearch)
            {
                pageIndex++;

                var doc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetMovieSearchByNameUrl(movieName, pageIndex));

                if (doc == null)
                {
                    keepSearch = false;
                }
                else
                {
                    var movieDtos = await GetMovieTorrentsAsync(doc.DocumentNode, movieName, year);

                    if (movieDtos.Any())
                        moviesSearchResult.AddRange(movieDtos);
                    else
                        keepSearch = false;
                }
            }

            return moviesSearchResult;
        }

        private async Task<IEnumerable<MediaTorrent>> GetMovieTorrentsAsync(HtmlNode documentNode, string title, int year)
        {
            var movies = documentNode.SelectNodes("//div[@class='card']");

            if (movies == null)
                return new MediaTorrent[0];

            var movieDtos = new List<MediaTorrent>();
            var tasks = new List<Task<IEnumerable<MediaTorrent>>>();
            foreach (var movie in movies)
            {
                tasks.Add(GetMovieTorrentsAsync(movie.InnerHtml, title, year));
            }

            await Task.WhenAll(tasks).ContinueWith(t => movieDtos.AddRange(t.Result.SelectMany(r => r)));

            return movieDtos;
        }

        private async Task<IEnumerable<MediaTorrent>> GetMovieTorrentsAsync(string movieHtml, string title, int year)
        {
            if (string.IsNullOrEmpty(movieHtml))
                return new MediaTorrent[0];

            var doc = new HtmlDocument();
            doc.LoadHtml(movieHtml);

            var movieTitle = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'title')]")?.InnerText.HtmlUnescape();
            int.TryParse(doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'year')]")?.InnerText, out var movieYear);

            if (!string.IsNullOrEmpty(movieTitle) && movieTitle.CustomCompare(title) && movieYear == year)
            {
                var movieDetailsLink = htmlUrlProvider.GetMovieDetailsUrl(doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'image-container-link')]")?.Attributes["href"].Value);
                return await GetMovieTorrentsAsync(movieDetailsLink);
            }
            else
                return new MediaTorrent[0];
        }

        private async Task<IEnumerable<MediaTorrent>> GetMovieTorrentsAsync(string movieDetailsLink)
        {
            if (string.IsNullOrEmpty(movieDetailsLink))
                return new MediaTorrent[0];

            var doc = await HttpRequester.GetHtmlDocumentAsync(movieDetailsLink);

            if(doc == null)
                return new MediaTorrent[0];

            var downloadTorrentNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'download-torrent')]");

            if(downloadTorrentNodes == null)
                return new MediaTorrent[0];
            else
                return downloadTorrentNodes.Select(n => new MediaTorrent()
                                                        {
                                                            DownloadUrl = htmlUrlProvider.GetTorrentUrl(n.Attributes["href"].Value),
                                                            Quality = n.InnerText
                }).DistinctBy(t => t.DownloadUrl);
        }
    }
}
