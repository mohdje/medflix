

using System.Text.Json.Serialization;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbImages
    {
        public TmdbImage[] Logos { get; set; }
    }
    internal class TmdbImage
    {
        public string FilePath { get; set; }
        public float VoteAverage { get; set; }

        [JsonPropertyName("iso_639_1")]
        public string CountryCode { get; set; }
    }
}
