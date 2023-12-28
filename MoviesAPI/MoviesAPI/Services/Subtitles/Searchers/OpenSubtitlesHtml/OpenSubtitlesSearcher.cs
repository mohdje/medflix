using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs;
using System.IO;
using System.IO.Compression;
using MoviesAPI.Helpers;
using System.Collections.Specialized;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles;
using System.Net.Http.Headers;
using MoviesAPI.Services.Subtitles.Searchers;

namespace MoviesAPI.Services.Subtitles
{
    public class OpenSubtitlesSearcher : ISubtitlesMovieSearcher, ISubtitlesSerieSearcher
    {
        private const string baseUrl = "https://www.opensubtitles.org";
        private const string subtitlesDownloadBaseUrl = "https://dl.opensubtitles.org/en/download/sub/";

        private readonly ISubtitlesFileProvider subtitlesProvider;

        internal OpenSubtitlesSearcher(ISubtitlesFileProvider subtitlesProvider)
        {
            this.subtitlesProvider = subtitlesProvider;
        }
        public async Task<IEnumerable<string>> GetAvailableMovieSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var openSubtitleMovieId = await GetOpenSubtitleMovieId(imdbCode);
            if (string.IsNullOrEmpty(openSubtitleMovieId))
                return new string[0];

            var doc = await HttpRequester.GetHtmlDocumentAsync(BuildSubtitlesListPageUrl(openSubtitleMovieId, subtitlesLanguage));
            if (doc == null)
                return new string[0];

            var htmlTableResults = doc.DocumentNode.SelectSingleNode("//table[@id='search_results']");
            if (htmlTableResults != null)
            {
                var searchResultsHtml = new HtmlDocument();
                searchResultsHtml.LoadHtml(htmlTableResults.InnerHtml);

                return searchResultsHtml.DocumentNode.SelectNodes("//a[contains(@onclick, '/subtitleserve/sub/')]")?
                                                                .Select(n =>
                                                                {
                                                                    var values = n.Attributes["href"].Value.Split('/');
                                                                    return subtitlesDownloadBaseUrl + values[values.Length - 1];
                                                                });

            }
            else
            {
                var singleResult = doc.DocumentNode.SelectSingleNode("//a[@id='bt-dwl-bt']")?.Attributes["href"]?.Value;
                if (!string.IsNullOrEmpty(singleResult))
                    return new string[] { baseUrl + singleResult };
            }

            return new string[0];
        }

        public async Task<IEnumerable<string>> GetAvailableSerieSubtitlesUrlsAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var openSubtitleMovieId = await GetOpenSubtitleMovieId(imdbCode);
            if (string.IsNullOrEmpty(openSubtitleMovieId))
                return new string[0];

            var doc = await HttpRequester.GetHtmlDocumentAsync(BuildSubtitlesListPageUrl(openSubtitleMovieId, subtitlesLanguage));
            if (doc == null)
                return new string[0];

            var htmlTableResults = doc.DocumentNode.SelectSingleNode("//table[@id='search_results']");
            if (htmlTableResults != null)
            {
                var searchResultsHtml = new HtmlDocument();
                searchResultsHtml.LoadHtml(htmlTableResults.InnerHtml);

                var rows = searchResultsHtml.DocumentNode.SelectNodes("//tr");

                var seasonNodeFound = false;
                foreach (var row in rows)
                {
                    var rowHtml = new HtmlDocument();
                    rowHtml.LoadHtml(row.InnerHtml);

                    if (!seasonNodeFound)
                    {
                        var seasonNode = rowHtml.DocumentNode.SelectSingleNode($"//span[@id='season-{seasonNumber}']");
                        if (seasonNode != null)
                        {
                            seasonNodeFound = true;
                            continue;
                        }
                    }
                    else
                    {
                        var episodeNode = rowHtml.DocumentNode.SelectSingleNode($"//span[@itemprop='episodeNumber']");
                        if(episodeNode != null && episodeNode.InnerText == episodeNumber.ToString())
                        {
                            var downloadNode = rowHtml.DocumentNode.SelectSingleNode($"//a[starts-with(@href,'/download/')]");
                            if(downloadNode != null)
                                return new string[] { baseUrl + downloadNode.Attributes["href"].Value };
                        }
                    }
                }
            }

            return new string[0];
        }

        private async Task<string> GetOpenSubtitleMovieId(string imdbCode)
        {
            var url = "https://www.opensubtitles.org/libs/suggest.php";

            var pamareters = new NameValueCollection();
            pamareters.Add("format", "json3");
            pamareters.Add("MovieName", imdbCode);

            try
            {
                var dto = await HttpRequester.GetAsync<List<OpenSubtitleMovieIdDto>>(url, pamareters);
                return dto?.FirstOrDefault()?.OpenSubtitleMovieId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitleSourceUrl)
        {
            var httpRequestHeaders = new List<KeyValuePair<string, string>>();
            httpRequestHeaders.Add(new KeyValuePair<string, string>("referer", baseUrl));

            var subtitlesFile = await subtitlesProvider.GetSubtitlesFileAsync(subtitleSourceUrl, httpRequestHeaders);

            return SubtitlesConverter.GetSubtitles(subtitlesFile);
        }

        private string GetLanguageCode(SubtitlesLanguage subtitlesLanguage)
        {
            switch (subtitlesLanguage)
            {
                case SubtitlesLanguage.French:
                    return "fre";
                case SubtitlesLanguage.English:
                    return "eng";
                default:
                    return null;
            }
        }

        private string BuildSubtitlesListPageUrl(string openSubtitleMovieId, SubtitlesLanguage subtitlesLanguage)
        {
            return $"{baseUrl}/en/search/idmovie-{openSubtitleMovieId}/sublanguageid-{GetLanguageCode(subtitlesLanguage)}";
        }

        public bool Match(string subtitlesSourceUrl)
        {
            return subtitlesSourceUrl.StartsWith(subtitlesDownloadBaseUrl) || subtitlesSourceUrl.StartsWith(baseUrl);
        }

        string ISearcherService.GetPingUrl()
        {
            return baseUrl;
        }
    }
}
