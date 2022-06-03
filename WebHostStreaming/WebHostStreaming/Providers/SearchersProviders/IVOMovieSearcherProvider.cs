using MoviesAPI.Services;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.VOMovies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IVOMovieSearcherProvider
    {
        VOMovieSearcher ActiveVOMovieSearcher { get; }
        IEnumerable<ServiceInfo> GetVOMoviesServicesInfo();
        ServiceInfo GetSelectedVOMoviesServiceInfo(bool includeAvailabiltyState);
        void UpdateSelectedVOMovieSearcher(int selectedMovieServiceId);
    }
}
