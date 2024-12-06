using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.Media
{
    public class MediaDetails
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }

        public int Year { get; set; }

        public string CoverImageUrl { get; set; }

        public string BackgroundImageUrl { get; set; }

        public double Rating { get; set; }

        public string Id { get; set; }

        public string Synopsis { get; set; }

        public string LogoImageUrl { get; set; }
        public Category[] Genres { get; set; }
        public string Director { get; set; }

        public string Cast { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public int Duration { get; set; }
        public int SeasonsCount { get; set; }
    }
}
