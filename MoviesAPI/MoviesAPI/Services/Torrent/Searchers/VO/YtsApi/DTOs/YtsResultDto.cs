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
    }

    class YtsMovieDto
    {
        public string Title { get; set; }

        public int Year { get; set; }

        public YtsTorrent[] Torrents { get; set; }
    }

    class YtsTorrent
    {
        public string Url { get; set; }
        public string Quality { get; set; }
    }


}
