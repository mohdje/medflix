using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Chromium;

namespace VfTorrentSearcher
{
    public class MonTorrentMovieSearcher
    {

        private string GetBaseUri()
        {
            return "https://www.montorrent.com";
        }

        private string GetSearchUri(string title)
        {
            return GetBaseUri() + $"/recherche/?s_films=on&langue=french&order=id&orderby=desc&query={title.Replace(" ", "+").ToLower()}#";
        }

        public async Task<IEnumerable<MovieTorrent>> SearchVfAsync(string title)
        {
            var url = GetSearchUri(title);

            string result;
            using (var webRequester = new ChromiumWebClient())
            {
                result = await webRequester.LoadHtlmSourceAsync(url);
            }

            var torrents = GetMovieTorrents(title, result);

            return torrents;
        }

        private IEnumerable<MovieTorrent> GetMovieTorrents(string movieTitle, string htmlSourceCode)
        {
            var result = new List<MovieTorrent>();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlSourceCode);

            var noResultElement = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'h1-aucun-resultat')]");

            if (noResultElement != null)
                return result;

            var torrentHtmlLines = doc.DocumentNode.SelectNodes("//div[contains(@class, 't-details')]");

            if (torrentHtmlLines == null)
                return null;

            foreach (var torrentHtmlLine in torrentHtmlLines)
            {
                var titleElement = torrentHtmlLine.SelectSingleNode(".//div[contains(@class, 't-rls text-start')]/a");

                if (titleElement == null || !titleElement.InnerText.StartsWith(movieTitle, StringComparison.OrdinalIgnoreCase))
                    continue;

                var pageLink = titleElement?.Attributes["href"].Value;
                var quality = torrentHtmlLine.SelectSingleNode(".//a[contains(@class, 'liste-categorie-couleur')]")?.Attributes["title"].Value;
                var downloadUrl = torrentHtmlLine.SelectSingleNode(".//div[contains(@class, 't-telechargement')]/a")?.Attributes["href"].Value;

                using (var webRequester = new ChromiumWebClient())
                {
                    var htmlPage = webRequester.LoadHtlmSourceAsync(this.GetBaseUri() + pageLink).Result;

                    var pageDoc = new HtmlAgilityPack.HtmlDocument();
                    pageDoc.LoadHtml(htmlPage);

                    var year = pageDoc.DocumentNode.SelectSingleNode(".//em[contains(@title, 'Année de production du film')]")?.InnerText;
                    var originalTitle = pageDoc.DocumentNode.SelectSingleNode(".//em[contains(@class, 'film-nom-original')]")?.InnerText;

                    if (originalTitle != null)
                        originalTitle = originalTitle.Substring(1, originalTitle.Length - 2).Trim();
                    else
                        originalTitle = pageDoc.DocumentNode.SelectSingleNode(".//li[contains(@title, 'Nom du Film')]/text()[1]")?.InnerText.Trim();

                    if (originalTitle.Equals(movieTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(new MovieTorrent()
                        {
                            Quality = quality.Replace("Accéder à la catégorie", "").Trim(),
                            DownloadUrl = this.GetBaseUri() + downloadUrl,
                            Year = year,
                            Title = originalTitle
                        });
                    }
                }
            }

            return result;
        }
    }
}
