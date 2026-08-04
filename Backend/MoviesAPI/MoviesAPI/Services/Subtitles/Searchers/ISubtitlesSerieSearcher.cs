using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles.Searchers
{
    internal interface ISubtitlesSerieSearcher
    {
        Task<IEnumerable<string>> GetAvailableSerieSubtitlesUrlsAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage subtitlesLanguage);
        Task<string> DownloadSubtitlesFileAsync(string subtitlesSourceUrl);
        bool Match(string subtitlesSourceUrl);
    }
}
