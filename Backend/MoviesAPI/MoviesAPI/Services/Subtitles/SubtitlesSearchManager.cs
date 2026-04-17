using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.Searchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    public class SubtitlesSearchManager
    {
        private readonly IEnumerable<ISubtitlesMovieSearcher> subtitlesMovieSearchers;
        private readonly IEnumerable<ISubtitlesSerieSearcher> subtitlesSerieSearchers;
        internal SubtitlesSearchManager(IEnumerable<ISubtitlesMovieSearcher> subtitlesMovieSearchers, IEnumerable<ISubtitlesSerieSearcher> subtitlesSerieSearchers)
        {
            this.subtitlesMovieSearchers = subtitlesMovieSearchers;
            this.subtitlesSerieSearchers = subtitlesSerieSearchers;
        }

        public async Task<IEnumerable<string>> GetAvailableMovieSubtitlesUrlsAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var tasks = new List<Task<IEnumerable<string>>>();

            foreach (var subtitlesSearcher in subtitlesMovieSearchers)
            {
                tasks.Add(subtitlesSearcher.GetAvailableMovieSubtitlesUrlsAsync(imdbCode, subtitlesLanguage));
            }

            var resultDtos = await Task.WhenAll(tasks);

            return resultDtos?.Where(r => r != null).SelectMany(r => r);
        }

        public async Task<IEnumerable<string>> GetAvailableSerieSubtitlesUrlsAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage subtitlesLanguage)
        {
            var tasks = new List<Task<IEnumerable<string>>>();

            foreach (var subtitlesSearcher in subtitlesSerieSearchers)
            {
                tasks.Add(subtitlesSearcher.GetAvailableSerieSubtitlesUrlsAsync(seasonNumber, episodeNumber, imdbCode, subtitlesLanguage));
            }

            var resultDtos = await Task.WhenAll(tasks);

            return resultDtos?.Where(r => r != null).SelectMany(r => r);
        }

        public async Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl)
        {
            var subtitlesMoviesSearcher = subtitlesMovieSearchers.SingleOrDefault(s => s.Match(subtitlesSourceUrl));

            if (subtitlesMoviesSearcher != null)
                return await subtitlesMoviesSearcher.GetSubtitlesAsync(subtitlesSourceUrl);

            var subtitlesSerieSearcher = subtitlesSerieSearchers.SingleOrDefault(s => s.Match(subtitlesSourceUrl));

            if (subtitlesSerieSearcher != null)
                return await subtitlesSerieSearcher.GetSubtitlesAsync(subtitlesSourceUrl);

            return [];
        }
    }
}
