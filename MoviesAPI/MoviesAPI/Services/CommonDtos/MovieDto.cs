using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.CommonDtos
{
    public class MovieDto
    {
        public string Title { get; set; }

        public string Year { get; set; }

        public string Genres { get; set; }

        public string CoverImageUrl { get; set; }

        public string BackgroundImageUrl { get; set; }

        public string Rating { get; set; }

        public MovieTorrent[] Torrents { get; set; }

        public string Synopsis { get; set; }

        public string Director { get; set; }

        public string Cast { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public string ImdbCode { get; set; }

        public string Id { get; set; }
    }

    public class MovieTorrent
    {
        public string DownloadUrl { get; set; }

        public string Quality { get; set; }

    }
}
