using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MoviesAPI.Extensions;
using MoviesAPI.Services.Content.Dtos;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbSearchResults
    {
        public TmdbSearchResult[] Results { get; set; }
    }

    internal class TmdbSearchResult
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("first_air_date")]
        public string FirstAirDate { get; set; }

        public int Year => !string.IsNullOrEmpty(FirstAirDate) ? FirstAirDate.GetYear() : ReleaseDate.GetYear();

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("vote_average")]
        public float VoteAverage { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }
        public string Overview { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        public TmdbVideos Videos { get; set; }

        public Genre[] Genres { get; set; }
        public int? Runtime { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("number_of_seasons")]
        public int SeasonsCount { get; set; }
    }
}
