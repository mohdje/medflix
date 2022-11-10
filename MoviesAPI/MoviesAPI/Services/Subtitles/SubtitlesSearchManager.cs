using MoviesAPI.Helpers;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs;
using MoviesAPI.Services.Subtitles.Searchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    public class SubtitlesSearchManager
    {
        private readonly IEnumerable<ISubtitlesSearcher> subtitlesSearchers;
        internal SubtitlesSearchManager(IEnumerable<ISubtitlesSearcher> subtitlesSearchers)
        {
            this.subtitlesSearchers = subtitlesSearchers;
        }

        public async Task<IEnumerable<string>> GetAvailableSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var tasks = new List<Task<IEnumerable<string>>>();

            foreach (var subtitlesSearcher in subtitlesSearchers)
            {
                tasks.Add(subtitlesSearcher.GetAvailableSubtitlesUrlsAsync(imdbCode, subtitlesLanguage));
            }

            var resultDtos = await Task.WhenAll(tasks);

            return resultDtos?.Where(r => r != null).SelectMany(r => r);            
        }

        public Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl)
        {
            var subtitlesSearcher = subtitlesSearchers.SingleOrDefault(s => s.Match(subtitlesSourceUrl));

            return subtitlesSearcher?.GetSubtitlesAsync(subtitlesSourceUrl);
        }
    }
}
