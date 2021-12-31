
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
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.VFMovies;
using MoviesAPI.Services;

namespace MoviesApiSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test started");

            //SearchVOMovies("spider man", VOMovieService.YtsApiMx);
            //GetLastMoviesByGenre(VOMovieService.YtsApiMx, "Action");
            //GetSuggestedMovies(VOMovieService.YtsApiMx);

            //SearchVFMovies("inside man", 2006);

            //PingService();

            //GetSubtitles(SubtitlesService.OpenSubtitlesHtml);

            //GetVOMovieServicesInfo();
            //GetVFMovieServicesInfo();
            GetSubtitlesServicesInfo();

            Console.WriteLine("End");
            Console.ReadKey();
        }

        static void GetVOMovieServicesInfo()
        {
            Console.WriteLine("VOMovieServices");
            var serviceInfos = MoviesAPIServiceFactories.VOMovieSearcherFactory.GetServicesInfo();
            foreach (var info in serviceInfos)
            {
                Console.WriteLine($"{info.Description}, {info.Id} : {(info.Available ? "available" : "unaivalable")}");
            }
        }

        static void GetVFMovieServicesInfo()
        {
            Console.WriteLine("VFMovieServices");
            var serviceInfos = MoviesAPIServiceFactories.VFMovieSearcherFactory.GetServicesInfo();
            foreach (var info in serviceInfos)
            {
                Console.WriteLine($"{info.Description}, {info.Id} : {(info.Available ? "available" : "unaivalable")}");
            }
        }

        static void GetSubtitlesServicesInfo()
        {
            Console.WriteLine("SubtitlesServices");
            var serviceInfos = MoviesAPIServiceFactories.SubtitlesProviderFactory.GetServicesInfo();
            foreach (var info in serviceInfos)
            {
                Console.WriteLine($"{info.Description}, {info.Id} : {(info.Available ? "available" : "unaivalable")}");
            }
        }

        static void SearchVOMovies(string title, VOMovieService movieService)
        {
            Console.WriteLine($"Search VO movies for {title}");

            var moviesServiceProvider = MoviesAPIServiceFactories.VOMovieSearcherFactory.GetService(movieService);
            var watch = Stopwatch.StartNew();
            var movies = moviesServiceProvider.GetMoviesByNameAsync(title).Result;
            watch.Stop();

            Console.WriteLine($"execution time :{watch.Elapsed.TotalSeconds}");

            ShowMoviesList(movies);
        }

        static void GetLastMoviesByGenre(VOMovieService movieService, string genre)
        {
            Console.WriteLine("Get last movies by genre...");

            var moviesServiceProvider = MoviesAPIServiceFactories.VOMovieSearcherFactory.GetService(movieService);
            var watch = Stopwatch.StartNew();
            var movies = moviesServiceProvider.GetLastMoviesByGenreAsync(12, genre).Result;
            watch.Stop();
            Console.WriteLine($"execution time :{watch.Elapsed.TotalSeconds}");
            ShowMoviesList(movies);
        }

        static void GetSuggestedMovies(VOMovieService movieService)
        {
            Console.WriteLine("Get suggested movies...");

            var moviesServiceProvider = MoviesAPIServiceFactories.VOMovieSearcherFactory.GetService(movieService);
            var watch = Stopwatch.StartNew();
            var movies = moviesServiceProvider.GetSuggestedMoviesAsync(6).Result;
            watch.Stop();
            Console.WriteLine($"execution time :{watch.Elapsed.TotalSeconds}");
            ShowMoviesList(movies);
        }

        static void ShowMoviesList(IEnumerable<MovieDto> movies)
        {
            if (movies.Any())
            {
                Console.WriteLine($"Movies :");
                foreach (var movie in movies)
                {
                    Console.WriteLine($"+{movie.Title} ({movie.Year}), {movie.Id}");
                }
            }
            else
                Console.WriteLine("No movies found");
        }

        static void SearchVFMovies(string title, int year)
        {
            var vfSearcher = MoviesAPIServiceFactories.VFMovieSearcherFactory.GetService(VFMoviesService.OxTorrent);

            Console.WriteLine($"Search VF movies for {title}");

            var movies = vfSearcher.GetMovieTorrentsAsync(title, year, true).Result;


            //Console.WriteLine($"VF movies found for {title} :");
            //foreach (var movie in movies)
            //{
            //    Console.WriteLine($"-{movie.Title}, year {movie.Year}, quality {movie.Quality}");
            //}
            //Console.WriteLine();
        }

        static void GetSubtitles(SubtitlesService subtitlesService)
        {
            var subtitlesProvider = MoviesAPIServiceFactories.SubtitlesProviderFactory.GetService(subtitlesService);
            var result = subtitlesProvider.GetAvailableSubtitlesAsync("tt0816692", SubtitlesLanguage.French).Result;

            if (result == null)
                Console.WriteLine("No subtitles found");
            else
            {
                Console.WriteLine($"subtitles found:{result.Language} - {string.Join(',', result.SubtitlesIds)}");

                var subtitles = subtitlesProvider.GetSubtitles(result.SubtitlesIds[0], Environment.CurrentDirectory).ToArray();
                var length = subtitles.Length > 10 ? 10 : subtitles.Count();
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine($"{subtitles[i].StartTime} - {subtitles[i].EndTime} : {subtitles[i].Text}");
                }
            }
        }
    }
}
