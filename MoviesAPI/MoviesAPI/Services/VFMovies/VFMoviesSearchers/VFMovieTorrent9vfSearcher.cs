using HtmlAgilityPack;
using MoviesAPI.Helpers;
using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Extensions;

namespace MoviesAPI.Services.VFMovies.VFMoviesSearchers
{
    public class VFMovieTorrent9vfSearcher : VFMoviesSearcher
    {
        private const string baseUrl = "https://www1.torrent9vf.fr/";

        internal VFMovieTorrent9vfSearcher()
        {

        }

        public async override Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(string title, int year, bool exactTitle)
        {
            var imdbMoviesInfo = await ImdbRequester.GetImdbMoviesInfoAsync(title);
            var imdbMovieInfo = imdbMoviesInfo?.SingleOrDefault(m => m.OriginalTitle.StartsWith(title, StringComparison.OrdinalIgnoreCase) && m.Year == year.ToString());

            if (imdbMovieInfo == null)
                return null;

            var searchUrl = baseUrl + "index.php?do=search&subaction=search";

            var htmlPage = await HttpRequester.PostAsync(searchUrl, new
            {
                search = imdbMovieInfo.FrenchTitle
            });

            if (string.IsNullOrEmpty(htmlPage))
                return null;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlPage);

            var searchResultList = doc.DocumentNode.SelectNodes("//table[contains(@class,'film-table')]");

            var result = new List<MovieTorrent>();

            if (searchResultList == null)
                return result;

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);

                var linkNode = doc.DocumentNode.SelectSingleNode("/a");
                if (linkNode != null
                    && ((exactTitle && linkNode.InnerText.Contains(imdbMovieInfo.FrenchTitle, StringComparison.OrdinalIgnoreCase)) || linkNode.InnerText.ContainsWords(title.Split(" ")))
                    && linkNode.InnerText.Contains("FRENCH")
                    && linkNode.InnerText.EndsWith(year.ToString())
                    && !linkNode.InnerText.Contains("MD")
                    && (linkNode.InnerText.Contains("720p") || linkNode.InnerText.Contains("1080p") || linkNode.InnerText.Contains("DVDRIP") || linkNode.InnerText.Contains("WEBRIP"))
                    )

                    getTorrentTasks.Add(new Task(() =>
                    {
                        var torrentLink = GetTorrentLink(linkNode.Attributes["href"].Value);
                        if (!string.IsNullOrEmpty(torrentLink))
                        {
                            result.Add(new MovieTorrent()
                            {
                                Quality = linkNode.InnerText.GetMovieQuality(),
                                DownloadUrl = torrentLink
                            });
                        }
                    }));

            }

            getTorrentTasks.ForEach(t => t.Start());
            Task.WaitAll(getTorrentTasks.ToArray());

            return result;
        }

        private string GetTorrentLink(string moviePageUrl)
        {
            var htmlPage = HttpRequester.GetAsync(new Uri(moviePageUrl)).Result;

            var startIndex = htmlPage.IndexOf("magnet:?xt=urn:btih");
            var lastIndex = startIndex > 0 ? htmlPage.IndexOf("\"", startIndex) : 0;

            if (startIndex * lastIndex > 0)
                return baseUrl + htmlPage.Substring(startIndex, lastIndex - startIndex);
            else
                return null;
        }

        protected override string GetPingUrl()
        {
            return baseUrl;
        }
    }
}
