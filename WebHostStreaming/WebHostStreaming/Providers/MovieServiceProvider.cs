using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public class MovieServiceProvider : IMovieServiceProvider
    {  
        private MovieServiceType movieServiceType = MovieServiceType.YtsApiMx;
        public IMovieService GetActiveMovieService()
        {
            return MovieServiceFactory.GetMovieService(movieServiceType);
        }
        public string GetActiveServiceTypeName()
        {
            return movieServiceType.ToString();
        }
        public IEnumerable<string> GetAvailableMovieServices()
        {
            return MovieServiceFactory.GetAvailableMovieServices();
        }

        public void UpdateActiveMovieService(MovieServiceType newMovieServiceType)
        {
            movieServiceType = newMovieServiceType;
        }
    }
}
