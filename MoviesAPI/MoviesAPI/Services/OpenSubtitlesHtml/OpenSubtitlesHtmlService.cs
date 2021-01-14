using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Services.OpenSubtitlesHtml.DTOs;
using System.IO;
using System.IO.Compression;
using MoviesAPI.Helpers;
using System.Collections.Specialized;
using MoviesAPI.Services.CommonDtos;

namespace MoviesAPI.Services.OpenSubtitlesHtml
{
    public class OpenSubtitlesHtmlService
    {
        private string baseUrl = "https://www.opensubtitles.org";
        public async Task<OpenSubtitlesDto> GetAvailableSubtitlesAsync(string imdbCode, string languageCode, string languageLabel)
        {
            var openSubtitleMovieId = await GetOpenSubtitleMovieId(imdbCode);
            if (string.IsNullOrEmpty(openSubtitleMovieId))
                return null;

            var doc = await GetDocument(baseUrl + "/en/search/idmovie-" + openSubtitleMovieId + "/sublanguageid-" + languageCode);
            if (doc == null)
                return null;

            var htmlTableResults = doc.DocumentNode.SelectSingleNode("//table[@id='search_results']")?.InnerHtml;
            if (string.IsNullOrEmpty(htmlTableResults))
                return null;

            var searchResultsHtml = new HtmlAgilityPack.HtmlDocument();
            searchResultsHtml.LoadHtml(htmlTableResults);

            return new OpenSubtitlesDto()
            {
                Language = languageLabel,
                SubtitlesIds = searchResultsHtml.DocumentNode.SelectNodes("//a[contains(@onclick, '/subtitleserve/sub/')]")
                                                            .Select(n =>
                                                            {
                                                                var values = n.Attributes["href"].Value.Split('/');
                                                                return values[values.Length - 1];
                                                            }).ToArray()
            };
        }

        public IEnumerable<SubtitlesDto> GetSubtitles(string subtitleId, string extractionFolder)
        {
            if (Directory.Exists(extractionFolder))
                Directory.Delete(extractionFolder, true);

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

            var dto = await HttpRequestHelper.GetAsync<List<OpenSubtitleMovieIdDto>>(url, pamareters);

            return dto?.FirstOrDefault()?.OpenSubtitleMovieId;
        }

        private void DownloadSubtitle(string url, string destinationFileName)
        {
            using (var client = new System.Net.WebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla / 5.0(Windows NT 6.3; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 87.0.4280.88 Safari / 537.36");
                client.Headers.Add("referer", baseUrl);
                client.DownloadFile(url, destinationFileName);
            }
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

        private async Task<HtmlDocument> GetDocument(string url)
        {
            var html = await HttpRequestHelper.GetAsync(url, null);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
    }
}
