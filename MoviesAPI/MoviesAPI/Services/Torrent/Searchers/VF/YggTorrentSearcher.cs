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
    internal class YggTorrentSearcher : ITorrentSearcher
    {
        private const string baseUrl = "https://www2.yggtorrent.co";

        public async Task<IEnumerable<MovieTorrent>> GetTorrentLinksAsync(string frenchMovieName, int year)
        {
            frenchMovieName = frenchMovieName.RemoveSpecialCharacters();

            var searchUrl = $"{baseUrl}/search_torrent/films/" + frenchMovieName.ToLower();

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//div[@class='table-responsive']//tr");

            var result = new List<MovieTorrent>();

            if (searchResultList == null)
                return result;

            var getTorrentTasks = new List<Task>();

            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var labelNode = doc.DocumentNode.SelectSingleNode("//h3");

                if (labelNode != null
                    && labelNode.InnerText.Replace(" ","").RemoveSpecialCharacters().Contains(frenchMovieName.Replace(" ",""), StringComparison.OrdinalIgnoreCase)
                    && labelNode.InnerText.Contains("FRENCH")
                    && labelNode.InnerText.EndsWith(year.ToString())
                    && !labelNode.InnerText.Contains("MD")
                    && (labelNode.InnerText.Contains("720p") || labelNode.InnerText.Contains("1080p") || labelNode.InnerText.Contains("DVDRIP") || labelNode.InnerText.Contains("WEBRIP"))
                    )
                {
                    var linkNode = doc.DocumentNode.SelectSingleNode("//a");

                    getTorrentTasks.Add(Task.Run(async() =>
                    {
                        var downloadUrl = await GetTorrentLinkAsync($"{baseUrl}/{linkNode.Attributes["href"].Value}");

                        if (!string.IsNullOrEmpty(downloadUrl))
                        {
                            result.Add(new MovieTorrent()
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
