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
using System.Web;

namespace MoviesAPI.Services.Torrent
{
    internal class ZeTorrentsSearcher : ITorrentMovieSearcher, ITorrentSerieSearcher
    {
        private const string baseUrl = "https://www.zetorrents.pw";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            return await SearchTorrentLinks(frenchMovieName,
                                            (mediaTitle) =>
                                            {
                                                return mediaTitle.CustomStartsWith(frenchMovieName)
                                                && (mediaTitle.Contains("FRENCH") || mediaTitle.Contains("TRUEFRENCH"))
                                                && mediaTitle.EndsWith(year.ToString())
                                                && !mediaTitle.Contains("MD")
                                                && (mediaTitle.Contains("720p") || mediaTitle.Contains("1080p") || mediaTitle.Contains("DVDRIP") || mediaTitle.Contains("WEBRIP"));
                                            });
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchSerieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var seasonId = $"S{(seasonNumber < 10 ? "0" : "")}{seasonNumber}";
            var episodeId = $"E{(episodeNumber < 10 ? "0" : "")}{episodeNumber}";

            return await SearchTorrentLinks(frenchSerieName,
                                            (mediaTitle) =>
                                            {
                                                return (mediaTitle.CustomStartsWith($"{frenchSerieName} Saison {seasonNumber}") || mediaTitle.CustomStartsWith($"{frenchSerieName} {seasonId}{episodeId}"))
                                                && mediaTitle.Contains("FRENCH");
                                            });
        }

        public string GetPingUrl()
        {
            return baseUrl;
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentLinks(string mediaName, Func<string, bool> mediaTitleCondition)
        {
            var searchUrl = $"{baseUrl}/recherche/{mediaName}";

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

                if (linkNode != null && mediaTitleCondition(HttpUtility.HtmlDecode(linkNode.InnerText)))
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
                                    DownloadUrl = torrentLink,
                                    LanguageVersion = "French"
                                });
                            }

                        }
                    }));
                }
            }

            await Task.WhenAll(getTorrentTasks);

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
    }
}
