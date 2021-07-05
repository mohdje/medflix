using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public class VOMovieSearcherProvider : IVOMovieSearcherProvider
    {  
        private MovieServiceType movieServiceType = MovieServiceType.YtsApiMx;
        public IVOMovieSearcher GetActiveVOMovieSearcher()
        {
            return VOMovieSearcherFactory.GetMovieService(movieServiceType);
        }
        public string GetActiveServiceTypeName()
        {
            return movieServiceType.ToString();
        }
        public IEnumerable<string> GetAvailableVOMovieSearchers()
        {
            return VOMovieSearcherFactory.GetAvailableMovieServices();
        }

        public void UpdateActiveVOMovieSearcher(MovieServiceType newMovieServiceType)
        {
            movieServiceType = newMovieServiceType;
        }
    }
}
