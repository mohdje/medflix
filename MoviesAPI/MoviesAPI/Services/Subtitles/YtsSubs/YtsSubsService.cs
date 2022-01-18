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

        public override IEnumerable<SubtitlesDto> GetSubtitles(string subtitlesSourceUrl)
        {
            var doc = HttpRequester.GetHtmlDocumentAsync(subtitlesSourceUrl).Result;

            var subtitlesDownloadUrl = doc.DocumentNode.SelectSingleNode("//a[@class='btn-icon download-subtitle']")?.Attributes["href"]?.Value;

            if (string.IsNullOrEmpty(subtitlesDownloadUrl))
                return null;

            DownloadSubtitlesZipFile(subtitlesDownloadUrl, null);
            var subtitlesFile = GetSubtitlesFile();

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
    }
}
