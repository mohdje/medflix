using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.CommonDtos
{
    public class SubtitlesDto
    {
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public string Text { get; set; }
    }
}
