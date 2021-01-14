using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using MoviesAPI.Services.CommonDtos;

namespace MoviesAPITest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("api call");

            //MoviesServiceTest();
            // SubtitlesServiceTest();
            List<string> a = null;

         

            Console.WriteLine("fin");

    
            Console.ReadKey();
        }

        private static void MoviesServiceTest()
        {
            var service = MoviesAPI.Services.MovieServiceFactory.GetMovieService(MoviesAPI.Services.MovieServiceType.YtsApiMx);

            //var a = service.GetSuggestedMoviesAsync(4);
            //var b = service.GetLastMoviesByGenreAsync(4, "thriller");
            //var c = service.GetMoviesByGenreAsync("thriller", 1);
            //var c2 = service.GetMoviesByGenreAsync("thriller", 2);
            //var d = service.GetMoviesByNameAsync("cop");
            ////var e = service.GetMoviesDetailsAsync("110");//api servcie 
            //var e = service.GetMoviesDetailsAsync("interstellar-2014");//html servcie 

            //System.Threading.Tasks.Task.WaitAll(new Task[] { a, b, c, c2, d, e });

            var tasks = new List<Task<IEnumerable<MovieDto>>>();
            foreach (var genre in service.GetMovieGenres())
            {
                tasks.Add(service.GetMoviesByGenreAsync(genre, 1));
            }

            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

        }

        private static void SubtitlesServiceTest()
        {
            var service = new MoviesAPI.Services.OpenSubtitlesHtml.OpenSubtitlesHtmlService();
            //var a = await service.DownloadSubtitle();//"783180", "fre" "eng"
            // var a = await service.GetAvailableSubtitlesAsync("tt6723592", "eng");//"783180", "fre" "eng"

            var b = @"C:\Users\mohamed\Downloads\Compressed";
            var c = service.GetSubtitles("7916834", b);


            //var c = MoviesAPI.Helpers.SubtitlesConverter.SubtitlesToDto(@"C:\Users\mohamed\Desktop\Projects\Streaming\data\Tenet.German BluRay.srt", false);

        }


    }
}
