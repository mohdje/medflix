using MoviesAPI.Services.VFMovies.VFMoviesSearchers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.VFMovies
{

    public class VFMovieSearcherFactory : ServiceFactory<VFMoviesService, VFMoviesSearcher>
    {
        public override VFMoviesSearcher GetService(VFMoviesService serviceType)
        {
            switch (serviceType)
            {
                case VFMoviesService.OxTorrent:
                    return new VFMovieOxTorrentSearcher();
                default:
                    return null;
            }
        }
    }
}
