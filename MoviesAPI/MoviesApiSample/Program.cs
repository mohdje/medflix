
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
            //var movies = new MoviesSample(Tokens.API_TOKEN);
            var series = new SeriesSample(Tokens.API_TOKEN);
            //var torrent = new TorrentSample();
            //var subtitles = new SubtitlesSample();

            //await movies.Test();
             await series.Test();
            //await torrent.Test();
            //await subtitles.GetSerieSubtitles(3, 5, "tt0773262", SubtitlesLanguage.English);

            Console.ReadKey();
        }

       
      

      
    }
}


