using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.OpenSubtitlesHtml.DTOs
{
    public class OpenSubtitleMovieIdDto
    {
        [JsonProperty("id")]
        public string OpenSubtitleMovieId { get; set; }
    }
}
