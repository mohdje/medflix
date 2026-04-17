using System.Collections.Generic;
using System.Threading.Tasks;
using MoviesAPI.Services.Subtitles.DTOs;

namespace MoviesAPI.Services.Subtitles.Searchers
{
    internal interface ISubtitlesSerieSearcher
    {
        Task<IEnumerable<string>> GetAvailableSerieSubtitlesUrlsAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage subtitlesLanguage);
        Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl);
        bool Match(string subtitlesSourceUrl);
    }
}
