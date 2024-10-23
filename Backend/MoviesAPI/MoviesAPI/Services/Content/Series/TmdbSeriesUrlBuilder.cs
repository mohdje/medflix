using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Tmdb
{
    internal class TmdbSeriesUrlBuilder : TmdbUrlBuilder
    {
        public TmdbSeriesUrlBuilder(string apiKey) : base(apiKey, TmdbUrlMode.Series)
        {

        }
    }
}
