using MoviesAPI.Helpers;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.Searchers.SubSource.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles.Searchers
{
    internal class SubSourceApi : ISubtitlesMovieSearcher, ISubtitlesSerieSearcher
    {
        readonly string BaseUrl = "https://api.subsource.net/v1";
        private readonly SubtitlesDownloader subtitlesDownloader;

        public SubSourceApi(SubtitlesDownloader subtitlesDownloader)
        {
            this.subtitlesDownloader = subtitlesDownloader;
        }
        public async Task<IEnumerable<string>> GetAvailableMovieSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            string mediaIdentifier = await GetMediaIdentifier(imdbCode);
            if (string.IsNullOrEmpty(mediaIdentifier))
                return Array.Empty<string>();

            var subtitlesSources = await GetSubtitlesSourcesAsync(mediaIdentifier, subtitlesLanguage);

            if (subtitlesSources == null || !subtitlesSources.Any())
                return Array.Empty<string>();

            var selelectedSubtitles = SelectBestSubtitlesSources(subtitlesSources);

            if (selelectedSubtitles == null || !selelectedSubtitles.Any())
                return Array.Empty<string>();

            return await GetSubtitlesDownloadLinks(selelectedSubtitles);
        }

        public async Task<IEnumerable<string>> GetAvailableSerieSubtitlesUrlsAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            string mediaIdentifier = await GetMediaIdentifier(imdbCode);
            if (string.IsNullOrEmpty(mediaIdentifier))
                return Array.Empty<string>();

            var subtitlesSources = await GetSubtitlesSourcesAsync(mediaIdentifier, subtitlesLanguage, seasonNumber);

            if (subtitlesSources == null || !subtitlesSources.Any())
                return Array.Empty<string>();

            var selelectedSubtitles = SelectBestSubtitlesSources(subtitlesSources, episodeNumber);

            if (selelectedSubtitles == null || !selelectedSubtitles.Any())
                return Array.Empty<string>();

            return await GetSubtitlesDownloadLinks(selelectedSubtitles);
        }

        public async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl)
        {
            return await subtitlesDownloader.DownloadSubtitlesAsync(subtitlesSourceUrl);
        }

        public bool Match(string subtitlesSourceUrl)
        {
            return subtitlesSourceUrl.StartsWith(BaseUrl);
        }

        private async Task<string> GetMediaIdentifier(string imdbCode)
        {
            var mediaSearchUrl = $"{BaseUrl}/movie/Search";

            var body = new Dictionary<string, object>
            {
                { "query", imdbCode },
                { "limit", 1 }
            };

            var mediaSearchResponse = await HttpRequester.PostAsync<SubSourceMediaSearchResult>(mediaSearchUrl, body);

            if (mediaSearchResponse?.Results == null || !mediaSearchResponse.Results.Any())
                return null;

            return mediaSearchResponse.Results.First().Link.Split("/").Last();
        }

        private async Task<IEnumerable<SubSourceSubtitle>> GetSubtitlesSourcesAsync(string mediaIdentifier, SubtitlesLanguage subtitlesLanguage, int? seasonNumber = null)
        {
            var subtitlesSearchUrl = $"{BaseUrl}/subtitles/{mediaIdentifier}";

            if (seasonNumber.HasValue)
                subtitlesSearchUrl += $"/season-{seasonNumber.Value}";

            var parameters = new NameValueCollection
            {
                { "language", GetLanguageCode(subtitlesLanguage) },
                { "sort_by_date", "true"}
            };

            var subtitlesSearchResponse = await HttpRequester.GetAsync<SubSourceSubtitlesSearchResult>(subtitlesSearchUrl, parameters);

            return subtitlesSearchResponse.Subtitles;
        }

        private static IEnumerable<SubSourceSubtitle> SelectBestSubtitlesSources(IEnumerable<SubSourceSubtitle> subtitlesSources, int? episodeNumber = null)
        {
            if (episodeNumber.HasValue)
                subtitlesSources = subtitlesSources.Where(s => s.ReleaseInfo.Contains($"E{episodeNumber.Value.ToString("D2")}"));

            if (!subtitlesSources.Any())
                return Array.Empty<SubSourceSubtitle>();

            var distinctSubtitles = subtitlesSources.GroupBy(s => s.Link).Select(s => s.First());

            var selelectedSubtitles = distinctSubtitles.Where(s => s.Rating == "good");

            if (!selelectedSubtitles.Any())
                selelectedSubtitles = distinctSubtitles.Where(s => s.UploaderBadges != null && s.UploaderBadges.Any());

            if (selelectedSubtitles.Count() < 5)
                selelectedSubtitles = selelectedSubtitles.Concat(
                    distinctSubtitles.Except(selelectedSubtitles).Take(5 - selelectedSubtitles.Count())
                );

            return selelectedSubtitles;
        }

        private async Task<IEnumerable<string>> GetSubtitlesDownloadLinks(IEnumerable<SubSourceSubtitle> selelectedSubtitles)
        {
            var tasks = new List<Task<string>>();
            foreach (var subtitle in selelectedSubtitles)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var subtitleFileDetailsUrl = $"{BaseUrl}/subtitle/{subtitle.Link}";
                    var response = await HttpRequester.GetAsync<SubSourceSubtitleFileInfo>(subtitleFileDetailsUrl);
                    return response?.Subtitle?.DownloadToken;
                }));
            }

            var subtitlesDownloadLinks = await Task.WhenAll(tasks);

            return subtitlesDownloadLinks.Where(l => !string.IsNullOrEmpty(l)).Select(l => $"{BaseUrl}/subtitle/download/{l}");
        }

        private string GetLanguageCode(SubtitlesLanguage subtitlesLanguage)
        {
            switch (subtitlesLanguage)
            {
                case SubtitlesLanguage.French:
                    return "french";
                case SubtitlesLanguage.English:
                    return "english";
                default:
                    return null;
            }
        }
    }
}
