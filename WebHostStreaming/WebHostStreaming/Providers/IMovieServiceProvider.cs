using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IMovieServiceProvider
    {
        IMovieService GetActiveMovieService();
        string GetActiveServiceTypeName();
        IEnumerable<string> GetAvailableMovieServices();
        void UpdateActiveMovieService(MovieServiceType newMovieServiceType);
    }
}
