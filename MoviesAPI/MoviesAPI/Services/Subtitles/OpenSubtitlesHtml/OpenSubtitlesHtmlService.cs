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
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles;
using System.Net.Http.Headers;

namespace MoviesAPI.Services.Subtitles.OpenSubtitlesHtml
{
    public class OpenSubtitlesHtmlService : SubtitlesSearcher
    {
        private string baseUrl = "https://www.opensubtitles.org";

        internal OpenSubtitlesHtmlService()
        {
        }
        public override async Task<SubtitlesSearchResultDto> GetAvailableSubtitlesAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var openSubtitleMovieId = await GetOpenSubtitleMovieId(imdbCode);
            if (string.IsNullOrEmpty(openSubtitleMovieId))
                return null;

            var doc = await HttpRequester.GetHtmlDocumentAsync(baseUrl + "/en/search/idmovie-" + openSubtitleMovieId + "/sublanguageid-" + GetLanguageCode(subtitlesLanguage));
            if (doc == null)
                return null;

            var htmlTableResults = doc.DocumentNode.SelectSingleNode("//table[@id='search_results']");
            if (htmlTableResults != null)
            {
                var searchResultsHtml = new HtmlDocument();
                searchResultsHtml.LoadHtml(htmlTableResults.InnerHtml);

                return new SubtitlesSearchResultDto()
                {
                    Language = GetLanguageLabel(subtitlesLanguage),
                    SubtitlesSourceUrls = searchResultsHtml.DocumentNode.SelectNodes("//a[contains(@onclick, '/subtitleserve/sub/')]")?
                                                                .Select(n =>
                                                                {
                                                                    var values = n.Attributes["href"].Value.Split('/');
                                                                    return "https://dl.opensubtitles.org/en/download/sub/" + values[values.Length - 1];
                                                                }).ToArray()
                };
            }

            var singleResult = doc.DocumentNode.SelectSingleNode("//a[@id='bt-dwl-bt']")?.Attributes["href"]?.Value;
            if (!string.IsNullOrEmpty(singleResult))
                return new SubtitlesSearchResultDto()
                {
                    Language = GetLanguageLabel(subtitlesLanguage),
                    SubtitlesSourceUrls = new string[] {baseUrl + singleResult}
                };

            return null;
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

        public override async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitleSourceUrl)
        {
            var httpRequestHeaders = new List<KeyValuePair<string, string>>();
            httpRequestHeaders.Add(new KeyValuePair<string, string>("referer", baseUrl));

            var subtitlesFile = await GetSubtitlesFileAsync(subtitleSourceUrl, httpRequestHeaders);

            return SubtitlesConverter.GetSubtitles(subtitlesFile);
        }

      
       
        protected override string GetLanguageCode(SubtitlesLanguage subtitlesLanguage)
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

        protected override string GetLanguageLabel(SubtitlesLanguage subtitlesLanguage)
        {
            switch (subtitlesLanguage)
            {
                case SubtitlesLanguage.French:
                    return "French";
                case SubtitlesLanguage.English:
                    return "English";
                default:
                    return null;
            }
        }

        protected override string GetPingUrl()
        {
            return baseUrl;
        }
    }
}
