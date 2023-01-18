using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Content.Dtos
{
    public class LiteContentDto
    {
        public string Title { get; set; }

        public int Year { get; set; }

        public string CoverImageUrl { get; set; }

        public string BackgroundImageUrl { get; set; }

        public double Rating { get; set; }

        public string Id { get; set; }

        public string Synopsis { get; set; }

        public string LogoImageUrl { get; set; }

    }
}
