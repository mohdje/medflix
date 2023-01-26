using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    public class OneomSearchResultDto
    {
        [JsonProperty("data")]
        public OneomDataSearchResultDto Data { get; set; }
    }

    public class OneomDataSearchResultDto
    {
        [JsonProperty("serials")]
        public OneomSerieDto[] Series { get; set; }

        [JsonProperty("serial")]
        public OneomSerieDto Serie { get; set; }
    }

    public class OneomSerieDto
    {
        public string Id { get; set; }
        public string Title { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbUrl { get; set; }

        public string ImdbId => !string.IsNullOrWhiteSpace(ImdbUrl) ? ImdbUrl.Split('/').Reverse().First() : null;

        [JsonProperty("ep")]
        public OneomEpisodeDto[] Episodes { get; set; }
    }

    public class OneomEpisodeDto
    {
        [JsonProperty("season")]
        public string Season { get; set; }
        [JsonProperty("ep")]
        public string Episode { get; set; }
        public OneomTorrentDto[] Torrent { get; set; }
    }

    public class OneomTorrentDto
    {
        public string Title { get; set; }

        public string Value { get; set; }
    }

}
