
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoviesAPI.Services.VOMovies;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services;
using System.Threading.Tasks;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Content.Dtos;
using MoviesApiSample.Samples;

namespace MoviesApiSample
{
    class Program
    {
       
        static async Task Main(string[] args)
        {
            Console.WriteLine("Test started");

           //await new MoviesSample(Tokens.API_TOKEN).Test();
            await new SeriesSample(Tokens.API_TOKEN).Test();
           // await new TorrentSample().Test();
            //await new SubtitlesSample().GetSerieSubtitles(3, 5, "tt0773262", SubtitlesLanguage.English);
            //await new SubtitlesSample().GetMovieSubtitles("tt2906216", SubtitlesLanguage.English);

            Console.ReadKey();
        }
    }
}


