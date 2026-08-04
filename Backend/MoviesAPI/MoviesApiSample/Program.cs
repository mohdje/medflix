
using System;
using System.Threading.Tasks;
using MoviesApiSample.Samples;

namespace MoviesApiSample
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var test = args[0];
            var tmdbToken = args[1];

            if (test == "subtitles")
            {
                await new SubtitlesSample().Test();
                return;
            }
            else if (test == "movies")
            {
                await new MoviesSample(tmdbToken).Test();
                return;
            }
            else if (test == "series")
            {
                await new SeriesSample(tmdbToken).Test();
                return;
            }
            else if (test == "torrent")
            {
                await new TorrentSample().Test();
                return;
            }
        }
    }
}


