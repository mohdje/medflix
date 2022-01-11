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
    public class VFMovieOxTorrentSearcher : VFMoviesSearcher
    {
        private const string baseUrl = "https://www.oxtorrents.co";

        private const string baseSearchUrl = "https://www.oxtorrents.co/recherche/";

        internal VFMovieOxTorrentSearcher()
        {

        }
        public override async Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(string title, int year, bool exactTitle)
        {
            var imdbMoviesInfo = await ImdbRequester.GetImdbMoviesInfoAsync(title);
            var imdbMovieInfo = imdbMoviesInfo?.SingleOrDefault(m => m.OriginalTitle.StartsWith(title, StringComparison.OrdinalIgnoreCase) && m.Year == year.ToString());

            if (imdbMovieInfo == null)
                return null;

            var frenchTitle = await ImdbRequester.GetFrenchMovieTitleAsync(imdbMovieInfo.ImdbCode);

            var searchUrl = baseSearchUrl + frenchTitle.Replace(" ", "-");

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//table[@class='table table-hover']//td");

            var result = new List<MovieTorrent>();

            if (searchResultList == null)
                return result;

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var mediaInfo = doc.DocumentNode.SelectSingleNode("/i");
                if (mediaInfo != null && mediaInfo.Attributes["class"].Value == "Films")
                {
                    var linkNode = doc.DocumentNode.SelectSingleNode("/a");
                    if (linkNode != null
                        && ((exactTitle && linkNode.InnerText.Contains(frenchTitle, StringComparison.OrdinalIgnoreCase)) || linkNode.InnerText.ContainsWords(title.Split(" ")))
                        && linkNode.InnerText.Contains("FRENCH")
                        && linkNode.InnerText.EndsWith(year.ToString())
                        && !linkNode.InnerText.Contains("MD")
                        && (linkNode.InnerText.Contains("720p") || linkNode.InnerText.Contains("1080p") || linkNode.InnerText.Contains("DVDRIP") || linkNode.InnerText.Contains("WEBRIP"))
                        )


                        getTorrentTasks.Add(new Task(() =>
                        {
                            var torrentLink = GetTorrentLink(baseUrl + linkNode.Attributes["href"].Value);
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
            }

            getTorrentTasks.ForEach(t => t.Start());
            Task.WaitAll(getTorrentTasks.ToArray());

            return result;
        }

        private string GetTorrentLink(string moviePageUrl)
        {
            var doc = HttpRequester.GetHtmlDocumentAsync(moviePageUrl).Result;

            var magnetNode = doc.DocumentNode.SelectSingleNode("//div[@class='btn-magnet']/a");

            return magnetNode != null ? magnetNode.Attributes["href"].Value : null;
        }

        protected override string GetPingUrl()
        {
            return baseUrl;
        }
    }
}
