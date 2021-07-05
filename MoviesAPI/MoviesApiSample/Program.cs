
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Collections.Generic;

namespace MoviesApiSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test started");

            var movieName = "Inside man";
            //  SearchVOMovies(movieName);


            var search = true;
            while (search)
            {
                SearchVFMovies(movieName);
                Console.WriteLine("New search ? (y for yes)");
                var a = Console.ReadKey();

                if (a.Key != ConsoleKey.Y)
                    search = false;

            }

            Console.WriteLine("End");
            Console.ReadKey();
        }

        static void SearchVOMovies(string title)
        {
            var moviesServiceProvider = MoviesAPI.Services.VOMovieSearcherFactory.GetMovieService(MoviesAPI.Services.MovieServiceType.YtsHtmlLtd);

            Console.WriteLine($"Search VO movies for {title}");

            var movies = moviesServiceProvider.GetMoviesByNameAsync(title).Result;

            Console.WriteLine($"VO movies found for {title} :");
            foreach (var movie in movies)
            {
                Console.WriteLine( $"-{movie.Id}, {movie.Title} - {movie.Year}");
            }
            Console.WriteLine();
        }

        static void SearchVFMovies(string title)
        {
            var vfSearcher = new VfTorrentSearcher.MonTorrentMovieSearcher();

            Console.WriteLine($"Search VF movies for {title}");

            var movies = vfSearcher.SearchVfAsync(title).Result;


            Console.WriteLine($"VF movies found for {title} :");
            foreach (var movie in movies)
            {
                Console.WriteLine($"-{movie.Title}, year {movie.Year}, quality {movie.Quality}");
            }
            Console.WriteLine();
        }

     


    }
}
