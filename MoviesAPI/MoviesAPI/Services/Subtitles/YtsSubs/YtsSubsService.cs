using MoviesAPI.Helpers;
using MoviesAPI.Services.Subtitles.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    public class YtsSubsService : SubtitlesSearcher
    {
        private const string baseUrl = "https://yts-subs.com";

        internal YtsSubsService()
        {

        }
        public async override Task<SubtitlesSearchResultDto> GetAvailableSubtitlesAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var searchUrl = baseUrl + "/movie-imdb/" + imdbCode;
            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if (doc == null)
                return null;

            var nodes = doc.DocumentNode.SelectNodes("//table[@class='table other-subs']/tbody/tr");

            if (nodes == null)
                return null;

            var subtitlesSourceLinks = new List<string>();

            var languageCode = GetLanguageCode(subtitlesLanguage);

            foreach (var node in nodes)
            {
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(node.InnerHtml);

                var languageInfo = doc.DocumentNode.SelectSingleNode("//span[@class='sub-lang']");

                if (languageInfo != null && languageInfo.InnerText == languageCode)
                {
                    var subtitleLink = doc.DocumentNode.SelectSingleNode("//td[@class='download-cell']//a")?.Attributes["href"]?.Value;

                    if (subtitleLink != null)
                        subtitlesSourceLinks.Add(baseUrl + subtitleLink);
                }
            }

            if (subtitlesSourceLinks.Any())
            {
                return new SubtitlesSearchResultDto()
                {
                    Language = GetLanguageLabel(subtitlesLanguage),
                    SubtitlesSourceUrls = subtitlesSourceLinks.ToArray()
                };
            }

            return null;
        }

        public override async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(subtitlesSourceUrl);

            var base64dataLink = doc.DocumentNode.SelectSingleNode("//a[@id='btn-download-subtitle']")?.Attributes["data-link"]?.Value;
            var subtitlesDownloadUrl = Base64Decode(base64dataLink);

            if (string.IsNullOrEmpty(subtitlesDownloadUrl))
                return null;

            var subtitlesFile = await GetSubtitlesFileAsync(subtitlesDownloadUrl, null);

            return SubtitlesConverter.GetSubtitles(subtitlesFile);
        }

        protected override string GetLanguageCode(SubtitlesLanguage subtitlesLanguage)
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

        protected override string GetLanguageLabel(SubtitlesLanguage subtitlesLanguage)
        {
            return GetLanguageCode(subtitlesLanguage);
        }

        protected override string GetPingUrl()
        {
            return baseUrl;
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
