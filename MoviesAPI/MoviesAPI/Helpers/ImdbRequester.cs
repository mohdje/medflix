using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    internal static class ImdbRequester
    {
        public static async Task<IEnumerable<ImdbMovieInfo>> GetImdbMoviesInfoAsync(string movieTitle)
        {
            var imdbMoviesInfo = new List<ImdbMovieInfo>();

            try
            {
                var searchUrl = $"https://www.imdb.com/find?q={movieTitle}&s=tt&ttype=ft&exact=true&ref_=fn_tt_ex";
                var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

                var searchResultList = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'findResult')]/td[@class='result_text']");

                if (searchResultList == null)
                    return null;

                foreach (var node in searchResultList)
                {
                    doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(node.InnerHtml);

                    var originalTitle = doc.DocumentNode.SelectSingleNode("/i")?.InnerText.Replace("\"", "") ?? doc.DocumentNode.SelectSingleNode("/a")?.InnerText;
                    var frenchTitle = doc.DocumentNode.SelectSingleNode("/a")?.InnerText;
                    var year = doc.DocumentNode.InnerText.Replace(originalTitle, "").Replace(frenchTitle, "").Replace("aka", "").Replace("(", "").Replace(")", "").Replace("\"", "").Trim();

                    if (originalTitle.Contains(movieTitle, StringComparison.OrdinalIgnoreCase))
                        imdbMoviesInfo.Add(new ImdbMovieInfo()
                        {
                            ImdbCode = doc.DocumentNode.SelectSingleNode("/a")?.Attributes["href"].Value.Split("/")[2],
                            OriginalTitle = originalTitle,
                            FrenchTitle = frenchTitle,
                            Year = year
                        });
                }
            }
            catch (Exception)
            {
                return null;
            }

            return imdbMoviesInfo;
        }

        public async static Task<string> GetFrenchMovieTitleAsync(string imdbCode)
        {
            if (string.IsNullOrEmpty(imdbCode))
                return null;

            var searchUrl = $"https://www.imdb.com/title/{imdbCode}";

            try
            {
                var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

                return doc?.DocumentNode.SelectSingleNode("//h1[contains(@class, 'TitleHeader__TitleText')]")?.InnerText;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    internal class ImdbMovieInfo
    {
        public string OriginalTitle { get; set; }
        public string FrenchTitle { get; set; }
        public string Year { get; set; }
        public string ImdbCode { get; set; }
    }
}
