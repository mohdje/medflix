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
        public override async Task<SubtitlesSearchResultDto> GetAvailableSubtitlesAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var openSubtitleMovieId = await GetOpenSubtitleMovieId(imdbCode);
            if (string.IsNullOrEmpty(openSubtitleMovieId))
                return null;

            var doc = await HttpRequester.GetHtmlDocumentAsync(baseUrl + "/en/search/idmovie-" + openSubtitleMovieId + "/sublanguageid-" + GetLanguageCode(subtitlesLanguage));
            if (doc == null)
                return null;

            var htmlTableResults = doc.DocumentNode.SelectSingleNode("//table[@id='search_results']");
            if (htmlTableResults == null)
                return null;

            var searchResultsHtml = new HtmlDocument();
            searchResultsHtml.LoadHtml(htmlTableResults.InnerHtml);

            return new SubtitlesSearchResultDto()
            {
                Language = GetLanguageLabel(subtitlesLanguage),
                SubtitlesIds = searchResultsHtml.DocumentNode.SelectNodes("//a[contains(@onclick, '/subtitleserve/sub/')]")?
                                                            .Select(n =>
                                                            {
                                                                var values = n.Attributes["href"].Value.Split('/');
                                                                return values[values.Length - 1];
                                                            }).ToArray()
            };
        }

        public override IEnumerable<SubtitlesDto> GetSubtitles(string subtitleId, string extractionFolder)
        {
            if (!Directory.Exists(extractionFolder))
                Directory.CreateDirectory(extractionFolder);

            var url = "https://dl.opensubtitles.org/en/download/sub/" + subtitleId;

            var downloadedFile = Path.Combine(extractionFolder, "subtitles.zip");
            if (File.Exists(downloadedFile))
                File.Delete(downloadedFile);

            DownloadSubtitle(url, Path.Combine(extractionFolder, downloadedFile));

            var extractedFile = Path.Combine(extractionFolder, "subtitles.srt");
            if (File.Exists(extractedFile))
                File.Delete(extractedFile);

            ExtractSubtitlesFile(downloadedFile, extractedFile);

           return SubtitlesConverter.GetSubtitles(extractedFile);
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

        private void DownloadSubtitle(string url, string destinationFileName)
        {
            var httpRequestHeaders = new List<KeyValuePair<string, string>>();
            httpRequestHeaders.Add(new KeyValuePair<string, string>("referer", baseUrl));

            var result = HttpRequester.DownloadAsync(new Uri(url), httpRequestHeaders, false).Result;

            File.WriteAllBytes(destinationFileName, result);
        }

        private void ExtractSubtitlesFile(string zipFile, string extractedFile)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".srt", StringComparison.OrdinalIgnoreCase))
                    {                        
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(extractedFile);

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        var destinationFolder = Path.GetDirectoryName(extractedFile);
                        if (destinationPath.StartsWith(destinationFolder, StringComparison.Ordinal))
                            entry.ExtractToFile(destinationPath);
                    }
                }
            }
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

        private string GetLanguageLabel(SubtitlesLanguage subtitlesLanguage)
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
