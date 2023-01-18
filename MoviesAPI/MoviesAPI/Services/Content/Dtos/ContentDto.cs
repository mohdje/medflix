using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Content.Dtos
{
    public class ContentDto : LiteContentDto
    {

        public string Genre { get; set; }
        public string Director { get; set; }

        public string Cast { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public int Duration { get; set; }

        public string ImdbId { get; set; }

        public int SeasonsCount { get; set; }
    }
}
