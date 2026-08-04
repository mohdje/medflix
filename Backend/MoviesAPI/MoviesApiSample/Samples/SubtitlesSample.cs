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
            subtitlesSearchManager = MoviesAPIFactory.CreateSubstitlesSearchManager(AppContext.BaseDirectory);
        }

        public async Task Test()
        {
            var language = SubtitlesLanguage.French;
            var availableSubtitlesUrls = await subtitlesSearchManager.GetAvailableMovieSubtitlesUrlsAsync("tt1431045", language);
            //var availableSubtitlesUrls = await subtitlesSearchManager.GetAvailableSerieSubtitlesUrlsAsync(1, 5, "tt19854762", language);

            if (availableSubtitlesUrls == null || !availableSubtitlesUrls.Any())
            {
                Console.WriteLine("No subtitles found");
                return;
            }

            Console.WriteLine($"subtitles found:{language} - {string.Join(',', availableSubtitlesUrls)}");
            var file = await subtitlesSearchManager.DownloadSubtitlesFileAsync(availableSubtitlesUrls.FirstOrDefault());
            Console.WriteLine("file downloaded: " + file);
        }
    }
}
