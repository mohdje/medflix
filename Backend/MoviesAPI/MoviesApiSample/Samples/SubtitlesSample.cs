using MoviesAPI.Services;
using MoviesAPI.Services.Subtitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApiSample.Samples
{
    internal class SubtitlesSample
    {
        SubtitlesSearchManager subtitlesSearchManager;
        public SubtitlesSample()
        {
            MoviesAPIFactory.Instance.SetSubtitlesFolder(AppContext.BaseDirectory);
            subtitlesSearchManager = MoviesAPIFactory.Instance.CreateSubstitlesSearchManagerAsync().Result;
        }

        public async Task GetMovieSubtitles(string imdbCode, SubtitlesLanguage language)
        {
            Console.WriteLine($"Search {language} subtitles for {imdbCode}");

            var availableSubtitlesUrls = await subtitlesSearchManager.GetAvailableMovieSubtitlesUrlsAsync(imdbCode, language);

            if (availableSubtitlesUrls == null || !availableSubtitlesUrls.Any())
                Console.WriteLine("No subtitles found");
            else
            {
                Console.WriteLine($"subtitles found:{language} - {string.Join(',', availableSubtitlesUrls)}");

                var subtitles = await subtitlesSearchManager.GetSubtitlesAsync(availableSubtitlesUrls.Last());
                var counter = 0;
                foreach (var sub in subtitles)
                {
                    Console.WriteLine($"{sub.StartTime} - {sub.EndTime} : {sub.Text}");
                    counter++;

                    if (counter == 10)
                        break;
                }

            }
        }

        public async Task GetSerieSubtitles(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage language)
        {
            Console.WriteLine($"Search {language} subtitles for {imdbCode}");

            var availableSubtitlesUrls = await subtitlesSearchManager.GetAvailableSerieSubtitlesUrlsAsync(seasonNumber, episodeNumber, imdbCode, language);

            if (availableSubtitlesUrls == null || !availableSubtitlesUrls.Any())
                Console.WriteLine("No subtitles found");
            else
            {
                Console.WriteLine($"subtitles found:{language} - {string.Join(',', availableSubtitlesUrls)}");

                var subtitles = await subtitlesSearchManager.GetSubtitlesAsync(availableSubtitlesUrls.Last());
                var counter = 0;
                foreach (var sub in subtitles)
                {
                    Console.WriteLine($"{sub.StartTime} - {sub.EndTime} : {sub.Text}");
                    counter++;

                    if (counter == 10)
                        break;
                }

            }
        }
    }
}
