using MoviesAPI.Services.Subtitles.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles.Searchers
{
    internal interface ISubtitlesMovieSearcher : ISearcherService
    {
        Task<IEnumerable<string>> GetAvailableMovieSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage);
        Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl);
        bool Match(string subtitlesSourceUrl);
    }
}
