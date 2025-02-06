
using System;
using System.Threading.Tasks;
using MoviesAPI.Services.Subtitles;
using MoviesApiSample.Samples;

namespace MoviesApiSample
{
    class Program
    {
       
        static async Task Main(string[] args)
        {
            Console.WriteLine("Test started");

           //await new MoviesSample(Tokens.API_TOKEN).Test();
           // await new SeriesSample(Tokens.API_TOKEN).Test();
          //  await new TorrentSample().Test();
           // await new SubtitlesSample().GetSerieSubtitles(1, 5, "tt11280740", SubtitlesLanguage.English);
              await new SubtitlesSample().GetMovieSubtitles("tt6263850", SubtitlesLanguage.French);

            Console.ReadKey();
        }
    }
}


