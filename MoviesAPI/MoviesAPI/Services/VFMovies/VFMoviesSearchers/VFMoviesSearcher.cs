using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.VFMovies.VFMoviesSearchers
{
    public abstract class VFMoviesSearcher : BaseService
    {
        public abstract Task<IEnumerable<MovieTorrent>> GetMovieTorrentsAsync(string title, int year, bool exactTitle);
    }
}
