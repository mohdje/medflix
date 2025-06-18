using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs;
using MoviesAPI.Helpers;
using System.Collections.Specialized;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.Searchers;

namespace MoviesAPI.Services.Subtitles
{
    public class OpenSubtitlesSearcher : ISubtitlesMovieSearcher, ISubtitlesSerieSearcher
    {
        private const string baseUrl = "https://www.opensubtitles.org";
        private const string subtitlesDownloadBaseUrl = "https://dl.opensubtitles.org/en/download/sub/";

        private readonly SubtitlesDownloader subtitlesDownloader;

        internal OpenSubtitlesSearcher(SubtitlesDownloader subtitlesDownloader)
        {
            this.subtitlesDownloader = subtitlesDownloader;
        }
        public async Task<IEnumerable<string>> GetAvailableMovieSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var doc = await GetSearchResultPageAsync(imdbCode, subtitlesLanguage);
            if (doc == null)
                return Array.Empty<string>();

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
                var singleResult = doc.DocumentNode.SelectSingleNode($"//a[contains(@href, '{subtitlesDownloadBaseUrl}')]")?.Attributes["href"]?.Value;
                if (!string.IsNullOrEmpty(singleResult))
                    return [singleResult];
            }

            return Array.Empty<string>();
        }

        public async Task<IEnumerable<string>> GetAvailableSerieSubtitlesUrlsAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var doc = await GetSearchResultPageAsync(imdbCode, subtitlesLanguage);
            if (doc == null)
                return Array.Empty<string>();

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
                                return [baseUrl + downloadNode.Attributes["href"].Value];
                        }
                    }
                }
            }

            return Array.Empty<string>();
        }

        private async Task<int?> GetOpenSubtitleMediaId(string imdbCode)
        {
            var url = "https://www.opensubtitles.org/libs/suggest.php";

            var pamareters = new NameValueCollection
            {
                { "format", "json3" },
                { "MovieName", imdbCode }
            };

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

        private async Task<HtmlDocument> GetSearchResultPageAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var openSubtitleMovieId = await GetOpenSubtitleMediaId(imdbCode);
            if (!openSubtitleMovieId.HasValue)
                return null;

            return await HttpRequester.GetHtmlDocumentAsync(BuildSubtitlesListPageUrl(openSubtitleMovieId.Value.ToString(), subtitlesLanguage));
        }

        public async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitleSourceUrl)
        {
            var httpRequestHeaders = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("referer", baseUrl)
            };

            return await subtitlesDownloader.DownloadSubtitlesAsync(subtitleSourceUrl, httpRequestHeaders);
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
    }
}
