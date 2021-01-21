using MoviesAPI.Services;
using MoviesAPI.Services.OpenSubtitlesHtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public static class MovieServiceProvider
    {
        private static MovieServiceType movieServiceType = MovieServiceType.YtsApiMx;
        public static IMovieService MovieService => MovieServiceFactory.GetMovieService(movieServiceType);

        public static OpenSubtitlesHtmlService MovieSubtitlesService => new OpenSubtitlesHtmlService();
        public static string ActiveServiceTypeName => movieServiceType.ToString();

        public static IEnumerable<string> AvailableMovieServices => MovieServiceFactory.GetAvailableMovieServices();

        public static void UpdateActiveMovieService(MovieServiceType newMovieServiceType)
        {
            movieServiceType = newMovieServiceType;
        }
    }
}
