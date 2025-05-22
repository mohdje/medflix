using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    public class TmdbTranslations
    {
        public TmdbTranslation[] Translations { get; set; }
    }

    public class TmdbTranslation
    {
        [JsonPropertyName("iso_3166_1")]
        public string CountryCode { get; set; }
        public Data Data { get; set; }
    }
    public class Data
    {
        public string Title { get; set; }
        public string Name { get; set; }
    }
}
