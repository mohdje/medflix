using MoviesAPI.Extensions;
using MoviesAPI.Services.Content.Dtos;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbSearchResults
    {
        public TmdbSearchResult[] Results { get; set; }
        public int TotalPages { get; set; }
    }

    internal class TmdbSearchResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
       
        public string Name { get; set; }
       
        public string ReleaseDate { get; set; }
       
        public string FirstAirDate { get; set; }

       
        public int Year => !string.IsNullOrEmpty(FirstAirDate) ? FirstAirDate.GetYear() : ReleaseDate.GetYear();
       
        public string PosterPath { get; set; }
       
        public float VoteAverage { get; set; }
       
        public string BackdropPath { get; set; }
       
        public string Overview { get; set; }
       

        public int VoteCount { get; set; }
       
        public TmdbVideos Videos { get; set; }
       
        public Genre[] Genres { get; set; }
       
        public int? Runtime { get; set; }
       
        public string ImdbId { get; set; }

        [JsonPropertyName("number_of_seasons")]
        public int SeasonsCount { get; set; }
    }
}
