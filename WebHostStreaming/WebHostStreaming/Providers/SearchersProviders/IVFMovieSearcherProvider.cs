using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.VFMovies.VFMoviesSearchers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IVFMovieSearcherProvider
    {
        VFMoviesSearcher ActiveVFMovieSearcher { get; }
        Task<IEnumerable<ServiceInfo>> GetVFMoviesServicesInfo();
        void UpdateSelectedVFMovieSearcher(int selectedMovieServiceId);
    }
}
