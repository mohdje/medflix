using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    internal class ZeTorrentsSearcher : ITorrentMovieSearcher
    {
        private const string baseUrl = "https://www.zetorrents.biz";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            frenchMovieName = frenchMovieName.RemoveSpecialCharacters();

            var searchUrl = $"{baseUrl}/recherche/" + frenchMovieName.ToLower();

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//div[@class='content-list-torrent']//div[@class='maxi']");

            var result = new List<MediaTorrent>();

            if (searchResultList == null)
                return result;

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var linkNode = doc.DocumentNode.SelectSingleNode("/a");

                if (linkNode != null
                    && linkNode.InnerText.Replace(" ","").RemoveSpecialCharacters().Contains(frenchMovieName.Replace(" ",""), StringComparison.OrdinalIgnoreCase)
                    && linkNode.InnerText.Contains("FRENCH")
                    && linkNode.InnerText.EndsWith(year.ToString())
                    && !linkNode.InnerText.Contains("MD")
                    && (linkNode.InnerText.Contains("720p") || linkNode.InnerText.Contains("1080p") || linkNode.InnerText.Contains("DVDRIP") || linkNode.InnerText.Contains("WEBRIP"))
                    )
                {
                    getTorrentTasks.Add(Task.Run(async() =>
                    {
                        var downloadUrl = await GetTorrentLinkAsync(baseUrl + linkNode.Attributes["href"].Value);

                        if (!string.IsNullOrEmpty(downloadUrl))
                        {
                            result.Add(new MediaTorrent()
                            {
                                Quality = linkNode.InnerText.GetMovieQuality(),
                                DownloadUrl = downloadUrl
                            });
                        } 
                    }));
                }
            }

            await Task.WhenAll(getTorrentTasks);

            return result;
        }

      
        public string GetPingUrl()
        {
            return baseUrl;
        }

        private async Task<string> GetTorrentLinkAsync(string moviePageUrl)
        {
            var htmlPage = await HttpRequester.GetAsync(new Uri(moviePageUrl));

            var startIndex = htmlPage.IndexOf("/telecharger/");
            var lastIndex = startIndex > 0 ? htmlPage.IndexOf("'", startIndex) : 0;

            if (startIndex * lastIndex > 0)
                return baseUrl + htmlPage.Substring(startIndex, lastIndex - startIndex);
            else
                return null;
        }
    }
}
