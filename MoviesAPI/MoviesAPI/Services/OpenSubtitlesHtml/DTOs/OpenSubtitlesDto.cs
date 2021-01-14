using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.OpenSubtitlesHtml.DTOs
{
    public class OpenSubtitlesDto
    {
        public string Language { get; set; }

        public string[] SubtitlesIds { get; set; }
    }
}
