using MoviesAPI.Services;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.VOMovies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public class VOMovieSearcherProvider : IVOMovieSearcherProvider
    {  
        private VOMovieService selectedVOMovieServiceType = VOMovieService.YtsApiMx;
        private VOMovieSearcherFactory VOMovieSearcherFactory = new VOMovieSearcherFactory();
        public VOMovieSearcher GetActiveVOMovieSearcher()
        {
            return VOMovieSearcherFactory.GetService(selectedVOMovieServiceType);
        }

        public ServiceInfo GetSelectedVOMoviesServiceInfo(bool includeAvailabiltyState)
        {
            return VOMovieSearcherFactory.GetServiceInfo(selectedVOMovieServiceType, includeAvailabiltyState); 
        }

        public IEnumerable<ServiceInfo> GetVOMoviesServicesInfo()
        {
            var servicesInfo = VOMovieSearcherFactory.GetServicesInfo(true);
            servicesInfo.SingleOrDefault(s => s.Id == (int)selectedVOMovieServiceType).Selected = true;
            return servicesInfo;
        }

        public void UpdateSelectedVOMovieSearcher(int selectedMovieServiceId)
        {
            selectedVOMovieServiceType = (VOMovieService)selectedMovieServiceId;
        }
    }
}
