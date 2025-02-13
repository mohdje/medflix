using MoviesAPI.Services;
using MoviesAPI.Services.Subtitles;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesApiSample.Samples
{
    internal class SubtitlesSample
    {
        SubtitlesSearchManager subtitlesSearchManager;
        public SubtitlesSample()
        {
            subtitlesSearchManager = MoviesAPIFactory.Instance.CreateSubstitlesSearchManager(AppContext.BaseDirectory);
        }

        public async Task Test()
        {
            //await GetMovieSubtitlesAsync("tt1431045", SubtitlesLanguage.French);
            await GetSerieSubtitlesAsync(1, 5, "tt11280740", SubtitlesLanguage.English);
            //await DisplaySubtitles("https://yts-subs.com/subtitles/deadpool-2016-english-yify-280634");
        }

        private async Task GetMovieSubtitlesAsync(string imdbCode, SubtitlesLanguage language)
        {
            Console.WriteLine($"Search {language} subtitles for {imdbCode}");

            var availableSubtitlesUrls = await subtitlesSearchManager.GetAvailableMovieSubtitlesUrlsAsync(imdbCode, language);

            if (availableSubtitlesUrls == null || !availableSubtitlesUrls.Any())
                Console.WriteLine("No subtitles found");
            else
            {
                Console.WriteLine($"subtitles found:{language} - {string.Join(',', availableSubtitlesUrls)}");

                await DisplaySubtitles(availableSubtitlesUrls.First());
            }
        }


        private async Task GetSerieSubtitlesAsync(int seasonNumber, int episodeNumber, string imdbCode, SubtitlesLanguage language)
        {
            Console.WriteLine($"Search {language} subtitles for {imdbCode}");

            var availableSubtitlesUrls = await subtitlesSearchManager.GetAvailableSerieSubtitlesUrlsAsync(seasonNumber, episodeNumber, imdbCode, language);

            if (availableSubtitlesUrls == null || !availableSubtitlesUrls.Any())
                Console.WriteLine("No subtitles found");
            else
            {
                Console.WriteLine($"subtitles found:{language} - {string.Join(',', availableSubtitlesUrls)}");

                await DisplaySubtitles(availableSubtitlesUrls.First());
            }
        }

        private async Task DisplaySubtitles(string subtitlesSourceUrl)
        {
            var subtitles = await subtitlesSearchManager.GetSubtitlesAsync(subtitlesSourceUrl);

            if(subtitles == null || !subtitles.Any())
                Console.WriteLine($"Enable to get subtitles from {subtitlesSourceUrl}");

            foreach (var sub in subtitles.Take(15))
            {
                Console.WriteLine($"{sub.StartTime} - {sub.EndTime} : {sub.Text}");
            }
        }
    }
}
