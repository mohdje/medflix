using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    public abstract class SubtitlesSearcher : BaseService
    {
        public abstract Task<SubtitlesSearchResultDto> GetAvailableSubtitlesAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage);

        public abstract IEnumerable<SubtitlesDto> GetSubtitles(string subtitleId, string extractionFolder);
    }
}
