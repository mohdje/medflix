using MoviesAPI.Helpers;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.Searchers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    internal class YtsSubsSearcher : ISubtitlesMovieSearcher
    {
        private const string baseUrl = "https://yts-subs.com";

        private readonly ISubtitlesFileProvider subtitlesProvider;

        internal YtsSubsSearcher(ISubtitlesFileProvider subtitlesProvider)
        {
            this.subtitlesProvider = subtitlesProvider;
        }
        public async Task<IEnumerable<string>> GetAvailableMovieSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var searchUrl = baseUrl + "/movie-imdb/" + imdbCode;
            var doc = await HttpRequester.GetHtmlDocumentAsync(searchUrl);

            if (doc == null)
                return new string[0];

            var nodes = doc.DocumentNode.SelectNodes("//table[@class='table other-subs']/tbody/tr");

            if (nodes == null)
                return new string[0];

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

            return subtitlesSourceLinks;
        }

        public async Task<string> GetSubtitlesFileAsync(string subtitlesSourceUrl)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(subtitlesSourceUrl);

            if (doc == null)
                return null;

            var base64dataLink = doc.DocumentNode.SelectSingleNode("//a[@id='btn-download-subtitle']")?.Attributes["data-link"]?.Value;
            var subtitlesDownloadUrl = Base64Decode(base64dataLink);

            if (string.IsNullOrEmpty(subtitlesDownloadUrl))
                return null;

            return await subtitlesProvider.GetSubtitlesFileAsync(subtitlesDownloadUrl, null);
        }

        public async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl)
        {
            var subtitlesFile = await GetSubtitlesFileAsync(subtitlesSourceUrl);

            return SubtitlesConverter.GetSubtitles(subtitlesFile);
        }

        public bool Match(string subtitlesSourceUrl)
        {
            return subtitlesSourceUrl.StartsWith(baseUrl);
        }

        private string GetLanguageCode(SubtitlesLanguage subtitlesLanguage)
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

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
