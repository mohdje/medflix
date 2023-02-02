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
    internal class ZeTorrentsSearcher : ITorrentMovieSearcher, ITorrentSerieSearcher
    {
        private const string baseUrl = "https://www.zetorrents.ch";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            var searchUrl = $"{baseUrl}/recherche/{frenchMovieName.RemoveSpecialCharacters(toLower: true)}";

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if (doc == null)
                return new MediaTorrent[0];

            var searchResultList = doc.DocumentNode.SelectNodes("//div[@class='content-list-torrent']//div[@class='maxi']");

            if (searchResultList == null)
                return new MediaTorrent[0];

            var result = new List<MediaTorrent>();
            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var linkNode = doc.DocumentNode.SelectSingleNode("/a");

                if (linkNode != null
                    && linkNode.InnerText.RemoveSpecialCharacters(removeSpaces: true, toLower: true).Contains(frenchMovieName.RemoveSpecialCharacters(removeSpaces: true, toLower: true))
                    && (linkNode.InnerText.Contains("FRENCH") || linkNode.InnerText.Contains("TRUEFRENCH"))
                    && linkNode.InnerText.EndsWith(year.ToString())
                    && !linkNode.InnerText.Contains("MD")
                    && (linkNode.InnerText.Contains("720p") || linkNode.InnerText.Contains("1080p") || linkNode.InnerText.Contains("DVDRIP") || linkNode.InnerText.Contains("WEBRIP"))
                    )
                {
                    getTorrentTasks.Add(Task.Run(async() =>
                    {
                        var torrentLinks = await GetTorrentLinkAsync(baseUrl + linkNode.Attributes["href"].Value);
                        if (torrentLinks.Any())
                        {
                            foreach (var torrentLink in torrentLinks)
                            {
                                result.Add(new MediaTorrent()
                                {
                                    Quality = linkNode.InnerText.GetVideoQuality(),
                                    DownloadUrl = torrentLink
                                });
                            }

                        }
                    }));
                }
            }

            await Task.WhenAll(getTorrentTasks);

            return result;
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchSerieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var searchUrl = $"{baseUrl}/recherche/{frenchSerieName.RemoveSpecialCharacters(toLower: true)}";

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if(doc == null)
                return new MediaTorrent[0];

            var searchResultList = doc.DocumentNode.SelectNodes("//div[@class='content-list-torrent']//div[@class='maxi']");

            if (searchResultList == null)
                return new MediaTorrent[0];

            var result = new List<MediaTorrent>();

            var getTorrentTasks = new List<Task>();

            var seasonId = $"S{(seasonNumber < 10 ? "0" : "")}{seasonNumber}";
            var episodeId = $"E{(episodeNumber < 10 ? "0" : "")}{episodeNumber}";

            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var linkNode = doc.DocumentNode.SelectSingleNode("/a");

                if (linkNode != null
                    && (linkNode.InnerText.CustomStartsWith($"{frenchSerieName} Saison {seasonNumber}")
                        || linkNode.InnerText.CustomStartsWith($"{frenchSerieName} {seasonId}{episodeId}"))
                    && linkNode.InnerText.Contains("FRENCH"))
                {
                    getTorrentTasks.Add(Task.Run(async () =>
                    {
                        var torrentLinks = await GetTorrentLinkAsync(baseUrl + linkNode.Attributes["href"].Value);
                        if (torrentLinks.Any())
                        {
                            foreach (var torrentLink in torrentLinks)
                            {
                                result.Add(new MediaTorrent()
                                {
                                    Quality = linkNode.InnerText.GetVideoQuality(),
                                    DownloadUrl = torrentLink
                                });
                            }

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

        private async Task<string[]> GetTorrentLinkAsync(string moviePageUrl)
        {
            //var htmlPage = await HttpRequester.GetAsync(new Uri(moviePageUrl));

            //var startIndex = htmlPage.IndexOf("/telecharger/");
            //var lastIndex = startIndex > 0 ? htmlPage.IndexOf("'", startIndex) : 0;

            //if (startIndex * lastIndex > 0)
            //    return baseUrl + htmlPage.Substring(startIndex, lastIndex - startIndex);
            //else
            //    return null;
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
    }
}
