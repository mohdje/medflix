using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoviesAPI.Services.Torrent
{
    internal abstract class TorrentVFSearcher : ITorrentVFMovieSearcher, ITorrentSerieSearcher
    {
        public abstract string Url { get; }

        protected abstract string SearchResultListIdentifier { get; }
        protected abstract string MagnetButtonIdentifier { get; }
        protected abstract string TorrentButtonIdentifier { get; }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            var request = new TorrentVFMovieSearchRequest(frenchMovieName, year);

            return await SearchTorrentLinks(request);
        }

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var request = new TorrentVFSerieSearchRequest(serieName, episodeNumber, seasonNumber, imdbId);

            return await SearchTorrentLinks(request);
        }

        protected async Task<IEnumerable<MediaTorrent>> SearchTorrentLinks(TorrentVFSearchRequest torrentVFSearchRequest)
        {
            var searchUrl = $"{Url}/recherche/{torrentVFSearchRequest.MediaName.RemoveSpecialCharacters()}";

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if (doc == null)
                return new MediaTorrent[0];

            var searchResultList = doc.DocumentNode.SelectNodes(SearchResultListIdentifier);

            if (searchResultList == null)
                return new MediaTorrent[0];

            var result = new List<MediaTorrent>();

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                if (torrentVFSearchRequest.Match(node, out var torrentPageRelativeUrl, out var mediaQuality))
                {
                    getTorrentTasks.Add(Task.Run(async () =>
                    {
                        var torrentLinks = await GetTorrentLinkAsync($"{Url}{torrentPageRelativeUrl}");
                        if (torrentLinks.Any())
                        {
                            result.AddRange(torrentLinks.Select(torrentLink => new MediaTorrent()
                                {
                                    Quality = mediaQuality,
                                    DownloadUrl = torrentLink
                                })
                            );
                        }
                    }));
                }
            }

            await Task.WhenAll(getTorrentTasks.ToArray());

            return result;
        }

  
        protected async Task<string[]> GetTorrentLinkAsync(string moviePageUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(moviePageUrl);

            var links = new List<string>();

            var magnetNode = !string.IsNullOrEmpty(MagnetButtonIdentifier) ? doc.DocumentNode.SelectSingleNode(MagnetButtonIdentifier) : null;
            var directDownloadNode = !string.IsNullOrEmpty(TorrentButtonIdentifier) ? doc.DocumentNode.SelectSingleNode(TorrentButtonIdentifier) : null;

            if (magnetNode != null)
                links.Add(magnetNode.Attributes["href"].Value);

            if (directDownloadNode != null)
                links.Add($"{Url}{directDownloadNode.Attributes["href"].Value}");

            return links.ToArray();
        }
    }
}
