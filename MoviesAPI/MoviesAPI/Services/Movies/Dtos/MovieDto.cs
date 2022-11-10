using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Movies.Dtos
{
    public class MovieDto : LiteMovieDto
    {

        public string Genre { get; set; }
        public string Director { get; set; }

        public string Cast { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public string Duration { get; set; }

        public string ImdbId { get; set; }
    }
}
