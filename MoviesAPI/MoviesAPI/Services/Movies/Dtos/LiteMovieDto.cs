﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Movies.Dtos
{
    public class LiteMovieDto
    {
        public string Title { get; set; }

        public string Year { get; set; }

        public string CoverImageUrl { get; set; }

        public string BackgroundImageUrl { get; set; }

        public double Rating { get; set; }

        public string Id { get; set; }

        public string Synopsis { get; set; }

        public string LogoImageUrl { get; set; }

    }
}