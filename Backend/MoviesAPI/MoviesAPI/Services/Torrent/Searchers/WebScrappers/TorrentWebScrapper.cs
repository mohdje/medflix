using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using MoviesAPI.Services.Torrent.Searchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        protected abstract string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest);
        protected abstract string GetTorrentTitle(HtmlDocument torrentResultNode);
        protected abstract bool TorrentHasSeeders(HtmlDocument torrentHtmlPage);

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
            var searchUrls = GetSearchUrls(torrentSearchRequest);

            HtmlNodeCollection searchResultList = null;
            foreach (var searchUrl in searchUrls)
            {
                searchResultList = await GetSearchResults(searchUrl);
                if (searchResultList != null && searchResultList.Any())
                    break;
            }

            if (searchResultList == null)
                return Array.Empty<MediaTorrent>();


            var getTorrentTasks = new List<Task<IEnumerable<MediaTorrent>>>();

            foreach (var resultListNode in searchResultList)
            {
                var docResultListNode = new HtmlDocument();
                docResultListNode.LoadHtml(resultListNode.InnerHtml);

                var torrentTitle = WebUtility.HtmlDecode(GetTorrentTitle(docResultListNode));

                if (!string.IsNullOrEmpty(torrentTitle) && torrentSearchRequest.MatchWithTorrentTitle(torrentTitle))
                {
                    var torrentLinkNode = docResultListNode.DocumentNode.SelectSingleNode(TorrentLinkPageIdentifier);

                    if(torrentLinkNode != null)
                    {
                        var link = torrentLinkNode.Attributes["href"].Value;
                        var pageUrl = link.StartsWith("http") ? link : $"{Url}{link}";

                        getTorrentTasks.Add(GetMediaTorrentsAsync(pageUrl));
                    }               
                }
            }

            var result = await Task.WhenAll(getTorrentTasks);

            return result.Where(r => r!= null).SelectMany(mediaTorrents => mediaTorrents);
        }

        protected async Task<HtmlNodeCollection> GetSearchResults(string searchUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if (doc == null)
                return null;

            return doc.DocumentNode.SelectNodes(SearchResultListIdentifier);
        }
  
        protected async Task<IEnumerable<MediaTorrent>> GetMediaTorrentsAsync(string mediaTorrentPageUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(mediaTorrentPageUrl);

            if (doc == null)
                return Array.Empty<MediaTorrent>();

            var mediaTorrents = new List<MediaTorrent>();

            if (!TorrentHasSeeders(doc))
                return Array.Empty<MediaTorrent>();

            var torrentLinkNodes = doc.DocumentNode.SelectNodes(TorrentLinkButtonsIdentifier);

            if (torrentLinkNodes == null || !torrentLinkNodes.Any())
                return Array.Empty<MediaTorrent>();

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
