using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.VFMovies.VFMoviesSearchers
{
    public class VFMoviesTorrent911Searcher : VFMoviesSearcher
    {
        private const string baseUrl = "https://www.torrent911.com";
        internal VFMoviesTorrent911Searcher()
        {

        }

        public async override Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(string title, int year, bool exactTitle)
        {
            var imdbMoviesInfo = await ImdbRequester.GetImdbMoviesInfoAsync(title);
            var imdbMovieInfo = imdbMoviesInfo?.SingleOrDefault(m => m.OriginalTitle.StartsWith(title, StringComparison.OrdinalIgnoreCase) && m.Year == year.ToString());

            if (imdbMovieInfo == null)
                return null;

            var frenchTitle = await ImdbRequester.GetFrenchMovieTitleAsync(imdbMovieInfo.ImdbCode);

            var searchUrl = $"{baseUrl}/recherche/" + frenchTitle.ToLower();

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//table[@class='table table-hover']//td//div[@class='maxi']");

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
                            result.Add(new MovieTorrent()
                            {
                                Quality = linkNode.InnerText.GetMovieQuality(),
                                DownloadUrl = GetTorrentLink(baseUrl + linkNode.Attributes["href"].Value)
                            });
                        }));
                }
                    
            }

            getTorrentTasks.ForEach(t => t.Start());
            Task.WaitAll(getTorrentTasks.ToArray());

            return result;
        }

        private string GetTorrentLink(string moviePageUrl)
        {
            var htmlPage = HttpRequester.GetAsync(new Uri(moviePageUrl)).Result;

            var startIndex = htmlPage.IndexOf("/telecharger/");
            var lastIndex = startIndex > 0 ? htmlPage.IndexOf("'", startIndex) : 0;

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
