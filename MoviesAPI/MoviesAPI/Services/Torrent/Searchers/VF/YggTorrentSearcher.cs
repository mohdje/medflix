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
    internal class YggTorrentSearcher : ITorrentMovieSearcher, ITorrentSerieSearcher
    {
        private const string baseUrl = "https://ww1.yggtorrent.fm";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            var searchUrl = $"{baseUrl}/search_torrent/films-french/" + frenchMovieName.RemoveSpecialCharacters(toLower: true);

            return await SearchTorrentLinks(searchUrl,
                            (mediaTitle) =>
                            {
                                return mediaTitle.CustomStartsWith(frenchMovieName)
                                && mediaTitle.EndsWith(year.ToString())
                                && !mediaTitle.Contains("MD")
                                && (mediaTitle.Contains("720p") || mediaTitle.Contains("1080p") || mediaTitle.Contains("DVDRIP") || mediaTitle.Contains("WEBRIP"));
                            });
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchSerieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var seasonId = $"S{(seasonNumber < 10 ? "0" : "")}{seasonNumber}";
            var episodeId = $"E{(episodeNumber < 10 ? "0" : "")}{episodeNumber}";
            var searchUrl = $"{baseUrl}/search_torrent/series-francaise/{frenchSerieName} saison {seasonNumber} {seasonId}{episodeId}";

            return await SearchTorrentLinks(searchUrl,
                            (mediaTitle) =>
                            {
                                return mediaTitle.CustomStartsWith($"{frenchSerieName} Saison {seasonNumber}")
                                       || mediaTitle.CustomStartsWith($"{frenchSerieName} {seasonId}{episodeId}");
                            });
        }

        public string GetPingUrl()
        {
            return baseUrl;
        }

        private async Task<IEnumerable<MediaTorrent>> SearchTorrentLinks(string searchUrl, Func<string, bool> mediaTitleCondition)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//div[@class='table-responsive']//tr");

            var result = new List<MediaTorrent>();

            if (searchResultList == null)
                return result;

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var linkNode = doc.DocumentNode.SelectSingleNode("//a");
                var nodeTitle = linkNode?.Attributes["title"]?.Value;

                if (nodeTitle != null && mediaTitleCondition(nodeTitle))
                {
                    getTorrentTasks.Add(Task.Run(async () =>
                    {
                        var downloadUrl = await GetTorrentLinkAsync($"{baseUrl}/{linkNode.Attributes["href"].Value}");

                        if (!string.IsNullOrEmpty(downloadUrl))
                        {
                            result.Add(new MediaTorrent()
                            {
                                Quality = linkNode.InnerText.GetVideoQuality(),
                                DownloadUrl = downloadUrl
                            });
                        }

                    }));

                }
            }

            await Task.WhenAll(getTorrentTasks);

            return result;

        }

        private async Task<string> GetTorrentLinkAsync(string moviePageUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(moviePageUrl);

            var downloadNodes = doc.DocumentNode.SelectNodes("//a[@class='btn btn-danger download']");

            foreach (var downloadNode in downloadNodes)
            {
                var link = downloadNode.Attributes["href"]?.Value;
                if (!string.IsNullOrEmpty(link) && link.StartsWith("magnet:?"))
                    return link;
            }

            return null;

        }
    }
}
