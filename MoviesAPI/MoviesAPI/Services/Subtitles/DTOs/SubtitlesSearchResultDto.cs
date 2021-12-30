using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs
{
    public class SubtitlesSearchResultDto
    {
        public string Language { get; set; }

        public string[] SubtitlesIds { get; set; }
    }
}
