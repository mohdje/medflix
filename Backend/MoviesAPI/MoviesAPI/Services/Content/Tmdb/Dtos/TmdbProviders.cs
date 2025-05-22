
using System.Text.Json.Serialization;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbProviders
    {
        [JsonPropertyName("results")]
        public TmdbProvider[] Providers { get; set; }
    }

    internal class TmdbProvider
    {
        [JsonPropertyName("provider_id")]
        public int Id { get; set; }

        [JsonPropertyName("provider_name")]
        public string Name { get; set; }
    }
}
