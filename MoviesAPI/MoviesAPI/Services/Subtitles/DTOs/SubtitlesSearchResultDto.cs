using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Subtitles.DTOs
{
    public class SubtitlesSearchResultDto
    {
        public string Language { get; set; }

        public string[] SubtitlesSourceUrls { get; set; }
    }
}
