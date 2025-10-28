using Medflix.Models.VideoPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.Media
{
    public class EpisodeSelectedEventArgs
    {
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public bool IsLastEpisodeOfSeason { get; set; }
        public WatchMediaInfo WatchMedia { get; set; }
    }
}
