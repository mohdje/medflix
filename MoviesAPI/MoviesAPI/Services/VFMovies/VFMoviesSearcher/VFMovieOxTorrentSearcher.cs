using HtmlAgilityPack;
using MoviesAPI.Helpers;
using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Extensions;

namespace MoviesAPI.Services.VFMovies.VFMoviesSearcher
{
    public class VFMovieOxTorrentSearcher : IVFMovieSearcher
    {
        private const string baseUrl = "https://www.oxtorrents.co";

        private const string baseSearchUrl = "https://www.oxtorrents.co/recherche/";

        public async Task<bool> PingAsync()
        {
            try
            {
                var result = await HttpRequester.GetAsync(new Uri(baseUrl));
                return !string.IsNullOrEmpty(result);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(string title, int year, bool exactTitle)
        {
            var imdbMoviesInfo = await ImdbRequester.GetImdbMoviesInfoAsync(title);
            var imdbMovieInfo = imdbMoviesInfo?.SingleOrDefault(m => m.OriginalTitle.StartsWith(title, StringComparison.OrdinalIgnoreCase) && m.Year == year.ToString());

            if (imdbMovieInfo == null)
                return null;

            var frenchTitle = await ImdbRequester.GetFrenchMovieTitleAsync(imdbMovieInfo.ImdbCode);

            var searchUrl = baseSearchUrl + frenchTitle.Replace(" ", "-");

            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            var searchResultList = doc.DocumentNode.SelectNodes("//table[@class='table table-hover']//td");

            var result = new List<MovieTorrent>();
            foreach (var node in searchResultList)
            {
                doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                var mediaInfo = doc.DocumentNode.SelectSingleNode("/i");
                if (mediaInfo != null && mediaInfo.Attributes["class"].Value == "Films")
                {
                    var linkNode = doc.DocumentNode.SelectSingleNode("/a");
                    if (linkNode != null
                        && ((exactTitle && linkNode.InnerText.Contains(frenchTitle, StringComparison.OrdinalIgnoreCase)) || linkNode.InnerText.ContainsWords(title.Split(" ")))
                        && linkNode.InnerText.Contains("FRENCH")
                        && linkNode.InnerText.EndsWith(year.ToString())
                        && !linkNode.InnerText.Contains("MD")
                        && (linkNode.InnerText.Contains("720p") || linkNode.InnerText.Contains("1080p") || linkNode.InnerText.Contains("DVDRIP"))
                        )
                        result.Add(new MovieTorrent()
                        {
                            Quality = GetTorrentQuality(linkNode.InnerText),
                            DownloadUrl = GetTorrentLink(baseUrl + linkNode.Attributes["href"].Value)
                        });
                }
            }

            return result;

        }

        private string GetTorrentQuality(string linkTitle)
        {
            var qualities = new string[] { "720p", "1080p", "DVDRIP" };

            foreach (var quality in qualities)
            {
                if (linkTitle.Contains(quality))
                    return quality;
            }

            return string.Empty;
        }

        private string GetTorrentLink(string moviePageUrl)
        {
            //la page du site ne contient pas de lien pour télécharger le torrent :'(
            return null;
        }


    }
}
