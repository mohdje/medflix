using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbSerieEpisodes
    {
        public TmdbEpisode[] Episodes { get; set; }
    }

    internal class TmdbEpisode
    {
        [JsonProperty("episode_number")]
        public int EpisodeNumber { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public int? RunTime { get; set; }

        [JsonProperty("still_path")]
        public string ImagePath { get; set; }

        [JsonProperty("air_date")]
        public DateTime AirDate { get; set; }

    }
}
