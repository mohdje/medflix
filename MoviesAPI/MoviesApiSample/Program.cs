
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

namespace MoviesApiSample
{
    class Program
    {
       
        static async Task Main(string[] args)
        {
            Console.WriteLine("Test started");
            var movies = new MoviesSample(Tokens.API_TOKEN);
            var series = new SeriesSample(Tokens.API_TOKEN);
            var torrent = new TorrentSample();

            //await movies.Test();
           // await series.Test();
            await torrent.Test();
            Console.ReadKey();
        }

       
        static async Task GetSubtitles(string imdbCode, SubtitlesLanguage language)
        {
            Console.WriteLine($"Search {language} subtitles for {imdbCode}");

            MoviesAPIFactory.Instance.SetSubtitlesFolder(AppContext.BaseDirectory);

            var subtitlesSearcher = await MoviesAPIFactory.Instance.CreateSubstitlesSearchManagerAsync();

            var availableSubtitlesUrls = await subtitlesSearcher.GetAvailableSubtitlesUrlsAsync(imdbCode, language);

            if (availableSubtitlesUrls == null || !availableSubtitlesUrls.Any())
                Console.WriteLine("No subtitles found");
            else
            {
                Console.WriteLine($"subtitles found:{language} - {string.Join(',', availableSubtitlesUrls)}");

                var subtitles = await subtitlesSearcher.GetSubtitlesAsync(availableSubtitlesUrls.Last());
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


