using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    internal class YtsTvSearcher : ITorrentSerieSearcher
    {
        private string baseUrl = "https://ytstv.me";
        public string GetPingUrl()
        {
            return baseUrl;
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var searchUrl = $"{baseUrl}/?s={serieName.RemoveSpecialCharacters(toLower: true)}";

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if (doc == null)
                return new MediaTorrent[0];

            var searchResultList = doc.DocumentNode.SelectNodes("//a[@class='ml-mask jt']");

            if (searchResultList == null)
                return new MediaTorrent[0];

            foreach (var node in searchResultList)
            {
                if (this.IsMatching(node.Attributes["oldtitle"]?.Value, serieName))
                    return await GetDownloadUrlsAsync(node.Attributes["href"]?.Value, seasonNumber, episodeNumber);
            }

            return new MediaTorrent[0];
        }

        private bool IsMatching(string nodeTitle, string serieName)
        {
            if (string.IsNullOrEmpty(nodeTitle))
                return false;

            var mediaTitleInfos = nodeTitle?.Split(" (");
            if (!mediaTitleInfos.Any())
                return false;

            var serieNameInMediaTitle = mediaTitleInfos[0];

            return serieNameInMediaTitle.CustomCompare(serieName);
        }

        private async Task<IEnumerable<MediaTorrent>> GetDownloadUrlsAsync(string mediaUrl, int seasonNumber, int episodeNumber)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(mediaUrl);

            if (doc == null)
                return new MediaTorrent[0];

            var seasonsNodes = doc.DocumentNode.SelectNodes("//div[@class='tvseason']");

            foreach (var seasonNode in seasonsNodes)
            {
                var text = seasonNode.InnerText;

                if (seasonNode.InnerText.Contains($"Season {seasonNumber}"))
                {
                    doc = new HtmlDocument();
                    doc.LoadHtml(seasonNode.InnerHtml);

                    var episodeNodes = doc.DocumentNode.SelectNodes("//a");

                    if (episodeNodes == null || !episodeNodes.Any())
                        return new MediaTorrent[0];
                    else if(episodeNodes.Count == 1 && episodeNodes[0].InnerText.Contains("Episode 1 -"))
                    {
                        var lastEpisodeNumberStr = episodeNodes[0].InnerText.Split(" ").Last();
                        int.TryParse(lastEpisodeNumberStr, out int lastEpisodeNumber);

                        return lastEpisodeNumber >= episodeNumber ? await GetEpisodeTorrentLinks(episodeNodes[0].Attributes["href"]?.Value) : new MediaTorrent[0];
                    }
                    else
                    {
                        var episodeNode = episodeNodes.SingleOrDefault(node => node.InnerText.Contains($"Episode {episodeNumber}"));
                        return episodeNode != null ? await GetEpisodeTorrentLinks(episodeNode.Attributes["href"]?.Value) : new MediaTorrent[0];
                    }
                }
            }

            return null;
        }

        private async Task<IEnumerable<MediaTorrent>> GetEpisodeTorrentLinks(string episodeUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(episodeUrl);

            if (doc == null)
                return new MediaTorrent[0];

            var linkNodes = doc.DocumentNode.SelectNodes("//div[@id='list-dl']//a");

            if (linkNodes == null || !linkNodes.Any())
                return new MediaTorrent[0];
            else
                return linkNodes.Select(node => new MediaTorrent
                {
                    DownloadUrl = node.Attributes["href"]?.Value,
                    Quality = node.Attributes["href"]?.Value.GetVideoQuality()
                });
        }
    }
}
