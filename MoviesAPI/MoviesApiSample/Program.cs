
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
            var movies = new Movies(Tokens.API_TOKEN);
            var series = new Series(Tokens.API_TOKEN);

            await movies.Test();
            //await series.Test();

            Console.ReadKey();
        }

        static async Task SearchVfTorrents(string frenchTitleMovie, int year)
        {
            Console.WriteLine($"Search vf torrents for {frenchTitleMovie}, {year}");
            var vfTorrentSearcher = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            var vfTorrents = await vfTorrentSearcher.SearchVfTorrentsAsync(frenchTitleMovie, year);

            if(vfTorrents == null || !vfTorrents.Any())
                Console.WriteLine("No torrent found");
            else
            {
                foreach (var vfTorrent in vfTorrents)
                {
                    Console.WriteLine($"Quanlity: {vfTorrent.Quality}, Url: {vfTorrent.DownloadUrl}");
                }
            }
        }

        static async Task SearchVoTorrents(string originalTitle, int year)
        {
            Console.WriteLine($"Search vo torrents for {originalTitle}, {year}");
            var voTorrentSearcher = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            var voTorrents = await voTorrentSearcher.SearchVoTorrentsAsync(originalTitle, year);

            if (voTorrents == null || !voTorrents.Any())
                Console.WriteLine("No torrent found");
            else
            {
                foreach (var vfTorrent in voTorrents)
                {
                    Console.WriteLine($"Quanlity: {vfTorrent.Quality}, Url: {vfTorrent.DownloadUrl}");
                }
            }
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
