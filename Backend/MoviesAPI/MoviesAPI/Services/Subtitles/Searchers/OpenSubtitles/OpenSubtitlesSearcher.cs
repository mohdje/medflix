using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.Searchers;
using System.Text.Json;

namespace MoviesAPI.Services.Subtitles
{
    public class OpenSubtitlesSearcher : ISubtitlesMovieSearcher, ISubtitlesSerieSearcher
    {
        private const string baseUrl = "https://www.opensubtitles.org";
        private const string subtitlesDownloadBaseUrl = "https://dl.opensubtitles.org/en/download";

        private readonly SubtitlesDownloader subtitlesDownloader;

        internal OpenSubtitlesSearcher(SubtitlesDownloader subtitlesDownloader)
        {
            this.subtitlesDownloader = subtitlesDownloader;
        }

        public async Task<IEnumerable<string>> GetAvailableMovieSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var url = $"https://rest.opensubtitles.org/search/imdbid-{imdbCode.Replace("t", "")}/sublanguageid-{GetLanguageCode(subtitlesLanguage)}";
            var response = await HttpRequester.GetAsync<List<OpenSubtitleDto>>(url, jsonNamingPolicy: JsonNamingPolicy.CamelCase);
            return response != null ? response.Select(r => r.ZipDownloadLink).Where(l => !string.IsNullOrEmpty(l)) : Array.Empty<string>();
        }

        public async Task<IEnumerable<string>> GetAvailableSerieSubtitlesUrlsAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var url = $"https://rest.opensubtitles.org/search/episode-{episodeNumber}/imdbid-{imdbCode.Replace("t", "")}/season-{seasonNumber}/sublanguageid-{GetLanguageCode(subtitlesLanguage)}";
            var response = await HttpRequester.GetAsync<List<OpenSubtitleDto>>(url, jsonNamingPolicy: JsonNamingPolicy.CamelCase);
            return response != null ? response.Select(r => r.ZipDownloadLink).Where(l => !string.IsNullOrEmpty(l)) : Array.Empty<string>();
        }

        public async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitleSourceUrl)
        {
            return await subtitlesDownloader.DownloadSubtitlesAsync(subtitleSourceUrl);
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

        public bool Match(string subtitlesSourceUrl)
        {
            return subtitlesSourceUrl.StartsWith(subtitlesDownloadBaseUrl) || subtitlesSourceUrl.StartsWith(baseUrl);
        }
    }
}
