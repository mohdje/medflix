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
                case VFMoviesService.ZeTorrents:
                    return new VFMoviesZeTorrentsSearcher();
                case VFMoviesService.Torrent911:
                    return new VFMoviesTorrent911Searcher();
                default:
                    return null;
            }
        }
    }
}
