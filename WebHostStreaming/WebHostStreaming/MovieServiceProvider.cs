using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming
{
    public static class MovieServiceProvider
    {
        private static MovieServiceType movieServiceType = MovieServiceType.YtsApiMx;
        public static IMovieService MovieService => MovieServiceFactory.GetMovieService(movieServiceType);
        public static string ActiveServiceTypeName => movieServiceType.ToString();

        public static void UpdateMovieService(MovieServiceType newMovieServiceType)
        {
            movieServiceType = newMovieServiceType;
        }

    }
}
