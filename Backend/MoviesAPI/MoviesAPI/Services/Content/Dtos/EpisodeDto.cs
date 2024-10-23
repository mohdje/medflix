using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Content.Dtos
{
    public class EpisodeDto
    {
        public int EpisodeNumber { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public int RunTime { get; set; }
        public string ImagePath { get; set; }
        public DateTime AirDate { get; set; }
    }
}
