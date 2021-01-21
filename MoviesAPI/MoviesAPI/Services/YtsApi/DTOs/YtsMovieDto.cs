using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsApi.DTOs
{
    public class YtsMovieDto
    {
        public int Id { get; set; }

        [JsonProperty("background_image")]
        public string BackgroundImageUrl { get; set; }

         [JsonProperty("description_full")]
        public string Summary { get; set; }

        public string[] Genres { get; set; }

        [JsonProperty("imdb_code")]
        public string ImdbCode { get; set; }

        [JsonProperty("medium_cover_image")]    
        public string CoverImageUrl { get; set; }

        public float Rating { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }
        
        [JsonProperty("yt_trailer_code")]
        public string YoutubeTrailerUrl { get; set; }

        [JsonProperty("date_uploaded_unix")]
        public int DateUploaded { get; set; }

        public YtsTorrent[] Torrents { get; set; }

        public Cast[] Cast { get; set; }

        public int Runtime { get; set; }
    }

    public class YtsTorrent
    {
        public string Url { get; set; }
        public string Quality { get; set; }
    }

    public class Cast
    {
        public string Name { get; set; }
    }
}
