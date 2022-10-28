using HtmlAgilityPack;
using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Extensions;
using MoviesAPI.Services.Torrent.Dtos;

namespace MoviesAPI.Services.Torrent
{
    internal class GkTorrentSearcher : ITorrentSearcher
    {
        private const string baseUrl = "https://www.gktorrents.cc";

        private const string baseSearchUrl = "https://www.gktorrents.cc/recherche/";

        public async Task<IEnumerable<MovieTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            var searchUrl = baseSearchUrl + frenchMovieName;

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//table[@class='table table-hover']//td");

            if (searchResultList == null)
                return null;

            var result = new List<MovieTorrent>();

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
                        && linkNode.InnerText.Contains(frenchMovieName, StringComparison.OrdinalIgnoreCase)
                        && linkNode.InnerText.Contains("FRENCH")
                        && linkNode.InnerText.EndsWith(year.ToString())
                        && !linkNode.InnerText.Contains("MD")
                        && (linkNode.InnerText.Contains("720p") || linkNode.InnerText.Contains("1080p") || linkNode.InnerText.Contains("DVDRIP") || linkNode.InnerText.Contains("WEBRIP"))
                        )


                        getTorrentTasks.Add(Task.Run(async () =>
                        {
                            var torrentLinks = await GetTorrentLinkAsync(baseUrl + linkNode.Attributes["href"].Value);
                            if (torrentLinks.Any())
                            {
                                foreach (var torrentLink in torrentLinks)
                                {
                                    result.Add(new MovieTorrent()
                                    {
                                        Quality = linkNode.InnerText.GetMovieQuality(),
                                        DownloadUrl = torrentLink
                                    });
                                }
                               
                            }
                        }));
                }
            }

            await Task.WhenAll(getTorrentTasks.ToArray());

            return result;
        }

        private async Task<string[]> GetTorrentLinkAsync(string moviePageUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(moviePageUrl);

            var links = new List<string>();

            var magnetNode = doc.DocumentNode.SelectSingleNode("//div[@class='btn-magnet']/a");
            var directDownloadNode = doc.DocumentNode.SelectSingleNode("//div[@class='btn-download']/a");

            if (magnetNode != null)
                links.Add(magnetNode.Attributes["href"].Value);

            if (directDownloadNode != null)
                links.Add(baseUrl + directDownloadNode.Attributes["href"].Value);

            return links.ToArray();
        }

      
        public string GetPingUrl()
        {
            return baseUrl;
        }
    }
}
