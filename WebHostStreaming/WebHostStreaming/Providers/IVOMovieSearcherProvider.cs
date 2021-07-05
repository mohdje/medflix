using MoviesAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IVOMovieSearcherProvider
    {
        IVOMovieSearcher GetActiveVOMovieSearcher();
        string GetActiveServiceTypeName();
        IEnumerable<string> GetAvailableVOMovieSearchers();
        void UpdateActiveVOMovieSearcher(MovieServiceType newMovieServiceType);
    }
}
