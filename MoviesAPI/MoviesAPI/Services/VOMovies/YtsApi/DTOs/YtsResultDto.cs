using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.VOMovies.YtsApi.DTOs
{
    class YtsResultDto
    {
        public YtsDataDto Data { get; set; }
    }

    class YtsDataDto
    {
        public YtsMovieDto[] Movies { get; set; }
        public YtsMovieDto Movie { get; set; }

    }

}
