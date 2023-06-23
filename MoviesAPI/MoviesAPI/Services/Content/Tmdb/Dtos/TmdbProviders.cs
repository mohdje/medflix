using Newtonsoft.Json;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbProviders
    {
        [JsonProperty("results")]
        public TmdbProvider[] Providers { get; set; }
    }

    internal class TmdbProvider
    {
        [JsonProperty("provider_id")]
        public int Id { get; set; }

        [JsonProperty("provider_name")]
        public string Name { get; set; }
    }
}
