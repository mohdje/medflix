using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Movies
{
    internal class TmdbVideos
    {
       public TmdbVideo[] Results { get; set; }
    }
    internal class TmdbVideo
    {
        public string Type { get; set; }
        public string Site { get; set; }
        public string Key { get; set; }
    }
}
