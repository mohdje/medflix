using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MoviesAPI.Services.Movies.Dtos
{
    internal class TmdbSearchResults
    {
        public TmdbSearchResult[] Results { get; set; }
    }

    internal class TmdbSearchResult
    {
        public string Id { get; set; }
        public string Title { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        public int Year => int.Parse(ReleaseDate.Split('-')[0]);

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("vote_average")]
        public string VoteAverage { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }
        public string Overview { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        public TmdbVideos Videos { get; set; }

        public Genre[] Genres { get; set; }
        public string Runtime { get; set; }

    }
}
