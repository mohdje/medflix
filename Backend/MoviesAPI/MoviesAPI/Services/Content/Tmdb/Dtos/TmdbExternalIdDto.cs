using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Tmdb.Dtos
{
    internal class TmdbExternalIdDto
    {
        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }
    }
}
