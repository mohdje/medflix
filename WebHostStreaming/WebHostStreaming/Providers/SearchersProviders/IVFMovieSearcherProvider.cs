using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.VFMovies.VFMoviesSearchers;
using System.Collections.Generic;

namespace WebHostStreaming.Providers
{
    public interface IVFMovieSearcherProvider
    {
        VFMoviesSearcher ActiveVFMovieSearcher { get; }
        IEnumerable<ServiceInfo> GetVFMoviesServicesInfo();
        void UpdateSelectedVFMovieSearcher(int selectedMovieServiceId);
    }
}
