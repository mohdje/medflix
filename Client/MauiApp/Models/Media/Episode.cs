using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.Media
{
    public class Episode
    {
        public int EpisodeNumber { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public int RunTime { get; set; }
        public string ImagePath { get; set; }
        public DateTime AirDate { get; set; }
    }
}
