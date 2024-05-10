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
using System.Xml.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class YggTorrentSearcher : ITorrentMovieSearcher, ITorrentSerieSearcher
    {
        private const string baseUrl = "https://www.yggtorrent.pm";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            var searchUrl = $"{baseUrl}/recherche/{frenchMovieName}";

            return await SearchTorrentLinks(searchUrl,
                            (mediaType) => mediaType.Contains("Films"),
                            (mediaTitle) =>
                            {
                                return mediaTitle.CustomStartsWith(frenchMovieName)
                                && mediaTitle.EndsWith(year.ToString())
                                && (mediaTitle.Contains("FRENCH") || mediaTitle.Contains("TRUEFRENCH"))
                                && !mediaTitle.Contains("MD")
                                && (mediaTitle.Contains("720p") || mediaTitle.Contains("1080p") || mediaTitle.Contains("DVDRIP") || mediaTitle.Contains("WEBRIP"));
                            });
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchSerieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var seasonId = $"S{(seasonNumber < 10 ? "0" : "")}{seasonNumber}";
            var episodeId = $"E{(episodeNumber < 10 ? "0" : "")}{episodeNumber}";
            var searchUrl = $"{baseUrl}/recherche/{frenchSerieName}";

            return await SearchTorrentLinks(searchUrl,
                            (mediaType) => mediaType.Contains("Animes") || mediaType.Contains("Séries"),
                            (mediaTitle) =>
                            {
                                return (mediaTitle.Contains("FRENCH") || mediaTitle.Contains("TRUEFRENCH")) 
                                && (mediaTitle.CustomStartsWith($"{frenchSerieName} Saison {seasonNumber}") || mediaTitle.CustomStartsWith($"{frenchSerieName} {seasonId}{episodeId}"));
                            });
        }

        public string GetPingUrl()
        {
            return baseUrl;
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentLinks(string searchUrl, Func<string, bool> mediaTypeCondition, Func<string, bool> mediaTitleCondition)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//td");

            var result = new List<MediaTorrent>();

            if (searchResultList == null)
                return result;

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                var type = node.Descendants()?.FirstOrDefault(n => n.Name == "img");
                var title = HttpUtility.HtmlDecode(node.InnerText);

                var isTypeMatching = type != null && mediaTypeCondition(type.Attributes["src"].Value);
                var isTitleMatching = title != null && mediaTitleCondition(title);

                if (!isTypeMatching || !isTitleMatching)
                    continue;

                var torrentPageLink = node.Descendants()?.FirstOrDefault(n => n.Name == "a")?.Attributes["href"]?.Value;

                if(torrentPageLink != null)
                {
                    getTorrentTasks.Add(Task.Run(async () =>
                    {
                        var torrentLinks = await GetTorrentLinkAsync($"{baseUrl}{torrentPageLink}");

                        if (torrentLinks != null)
                        {
                            foreach (var torrentLink in torrentLinks)
                            {
                                result.Add(new MediaTorrent()
                                {
                                    Quality = node.InnerText.GetVideoQuality(),
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

        private async Task<string[]> GetTorrentLinkAsync(string pageUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(pageUrl);

            var downloadNodes = doc.DocumentNode.SelectNodes("//a[@class='butt' or @class='bott']");

            return downloadNodes?.Select(node => BuildTorrentUrl(node.Attributes["href"]?.Value)).ToArray();
        }

        private string BuildTorrentUrl(string originalTorrentLink)
        {
            return originalTorrentLink.StartsWith("magnet") ? originalTorrentLink : $"{baseUrl}{originalTorrentLink}";
        }
    }
}
