using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Tmdb
{
    internal class TmdbMovieUrlBuilder : TmdbUrlBuilder
    {
        public TmdbMovieUrlBuilder(string apiKey) : base(apiKey, TmdbUrlMode.Movies)
        {

        }
    }
}
