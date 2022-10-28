using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Movies
{
    internal class TmdbCredits
    {
        public Cast[] Cast { get; set; }
        public Crew[] Crew { get; set; }
    }

    internal class Cast
    {
        public string Name { get; set; }
    }

    internal class Crew
    {
        public string Job { get; set; }
        public string Name { get; set; }
    }
}
