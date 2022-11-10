using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Movies
{
    internal class TmdbImages
    {
        public TmdbImage[] Logos { get; set; }
    }
    internal class TmdbImage
    {
        [JsonProperty("file_path")]
        public string FilePath { get; set; }
        [JsonProperty("vote_average")]
        public float VoteAverage { get; set; }

        [JsonProperty("iso_639_1")]
        public string CountryCode { get; set; }
    }
}
