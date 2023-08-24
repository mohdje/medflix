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
    internal class YggTorrentSearcher : ITorrentMovieSearcher, ITorrentSerieSearcher
    {
        private const string baseUrl = "https://www5.yggtorrent.ac";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            var searchUrl = $"{baseUrl}/recherche/{frenchMovieName}";

            return await SearchTorrentLinks(searchUrl,
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

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentLinks(string searchUrl, Func<string, bool> mediaTitleCondition)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//div[@class='default slidable']//a");

            var result = new List<MediaTorrent>();

            if (searchResultList == null)
                return result;

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                var nodeTitle = HttpUtility.HtmlDecode(node.InnerText);

                if (nodeTitle != null && mediaTitleCondition(nodeTitle))
                {
                    getTorrentTasks.Add(Task.Run(async () =>
                    {
                        var torrentLinks = await GetTorrentLinkAsync(node.Attributes["href"].Value);

                        if (torrentLinks != null)
                        {
                            foreach (var torrentLink in torrentLinks)
                            {
                                result.Add(new MediaTorrent()
                                {
                                    Quality = node.InnerText.GetVideoQuality(),
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

        private async Task<string[]> GetTorrentLinkAsync(string moviePageUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(moviePageUrl);

            var downloadNodes = doc.DocumentNode.SelectNodes("//a[@class='butt' or @class='bott']");

            return downloadNodes?.Select(node => node.Attributes["href"]?.Value).ToArray();

        }
    }
}
