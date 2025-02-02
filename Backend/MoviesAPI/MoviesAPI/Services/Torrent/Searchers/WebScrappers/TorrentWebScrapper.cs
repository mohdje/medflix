using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using MoviesAPI.Services.Torrent.Searchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace MoviesAPI.Services.Torrent
{
    internal abstract class TorrentWebScrapper : ITorrentSearcher
    {
        public abstract string Url { get; }
        protected abstract string SearchResultListIdentifier { get; }
        protected abstract string TorrentLinkPageIdentifier { get; }
        protected abstract string TorrentLinkButtonsIdentifier { get; }
        protected abstract string MediaQualityIdentifier { get; }
        protected abstract bool FrenchVersion { get; }
        protected abstract bool CheckQuality { get; }
        protected abstract string BuildSearchUrl(TorrentSearchRequest torrentSearchRequest);
        protected abstract string GetTorrentTitle(HtmlDocument htmlNode);


        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string movieName, int year)
        {
            var request = new TorrentMovieSearchRequest(movieName, year, FrenchVersion, CheckQuality);

            return await SearchTorrentLinks(request);
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string serieName, int seasonNumber, int episodeNumber)
        {
            var request = new TorrentSerieSearchRequest(serieName, episodeNumber, seasonNumber, FrenchVersion);

            return await SearchTorrentLinks(request);
        }

        protected async Task<IEnumerable<MediaTorrent>> SearchTorrentLinks(TorrentSearchRequest torrentSearchRequest)
        {
            var searchUrl = BuildSearchUrl(torrentSearchRequest); 

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if (doc == null)
                return Array.Empty<MediaTorrent>();

            var searchResultList = doc.DocumentNode.SelectNodes(SearchResultListIdentifier);

            if (searchResultList == null)
                return Array.Empty<MediaTorrent>();

            var result = new List<MediaTorrent>();

            var getTorrentTasks = new List<Task>();

            foreach (var resultListNode in searchResultList)
            {
                var docResultListNode = new HtmlDocument();
                docResultListNode.LoadHtml(resultListNode.InnerHtml);

                var torrentTitle = GetTorrentTitle(docResultListNode);

                if (!string.IsNullOrEmpty(torrentTitle) && torrentSearchRequest.MatchWithTorrentTitle(torrentTitle))
                {
                    var torrentLinkNode = docResultListNode.DocumentNode.SelectSingleNode(TorrentLinkPageIdentifier);

                    if(torrentLinkNode != null)
                    {
                        var link = torrentLinkNode.Attributes["href"].Value;
                        var pageUrl = link.StartsWith("http") ? link : $"{Url}{link}";

                        getTorrentTasks.Add(Task.Run(async () =>
                        {
                            var mediaTorrents = await GetMediaTorrentsAsync(pageUrl);
                            if (mediaTorrents.Any())
                                result.AddRange(mediaTorrents);

                        }));
                    }
                    
                }
            }

            await Task.WhenAll(getTorrentTasks.ToArray());

            return result;
        }

  
        protected async Task<IEnumerable<MediaTorrent>> GetMediaTorrentsAsync(string moviePageUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(moviePageUrl);

            var mediaTorrents = new List<MediaTorrent>();

            var torrentLinkNodes = doc.DocumentNode.SelectNodes(TorrentLinkButtonsIdentifier);

            foreach (var torrentLinkNode in torrentLinkNodes)
            {
                var torrentLink = torrentLinkNode.Attributes["href"].Value;
                if (!torrentLink.StartsWith("magnet") && !torrentLink.StartsWith("http"))
                    torrentLink = $"{Url}{torrentLink}";

                var qualityNode = !string.IsNullOrEmpty(MediaQualityIdentifier) ? 
                    (doc.DocumentNode.SelectSingleNode(MediaQualityIdentifier) ?? torrentLinkNode)
                    : torrentLinkNode;

                mediaTorrents.Add(new MediaTorrent
                {
                    Quality = qualityNode.InnerText.GetVideoQuality(),
                    DownloadUrl = torrentLink
                });
            }

            return mediaTorrents;
        }
    }
}
