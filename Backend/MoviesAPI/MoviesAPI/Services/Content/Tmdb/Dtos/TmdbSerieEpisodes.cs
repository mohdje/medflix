
using System;
using System.Text.Json.Serialization;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbSerieEpisodes
    {
        public TmdbEpisode[] Episodes { get; set; }
    }

    internal class TmdbEpisode
    {
        public int EpisodeNumber { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public int? Runtime { get; set; }

        [JsonPropertyName("still_path")]
        public string ImagePath { get; set; }

        public DateTime AirDate { get; set; }

    }
}
