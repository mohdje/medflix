using MoviesAPI.Services;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.VFMovies;
using MoviesAPI.Services.VFMovies.VFMoviesSearchers;
using System.Collections.Generic;
using System.Linq;

namespace WebHostStreaming.Providers
{
    public class VFMovieSearcherProvider : IVFMovieSearcherProvider
    {
        private VFMoviesService selectedVFMovieServiceType = VFMoviesService.OxTorrent;
        private VFMovieSearcherFactory VFMovieSearcherFactory = new VFMovieSearcherFactory();
        public VFMoviesSearcher GetActiveVFMovieSearcher()
        {
            return VFMovieSearcherFactory.GetService(selectedVFMovieServiceType);
        }

        public IEnumerable<ServiceInfo> GetVFMoviesServicesInfo()
        {
            var servicesInfo = VFMovieSearcherFactory.GetServicesInfo(true);
            servicesInfo.SingleOrDefault(s => s.Id == (int)selectedVFMovieServiceType).Selected = true;
            return servicesInfo;
        }

        public void UpdateSelectedVFMovieSearcher(int selectedMovieServiceId)
        {
            selectedVFMovieServiceType = (VFMoviesService)selectedMovieServiceId;
        }
    }
}
