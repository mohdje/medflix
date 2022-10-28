using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Torrent.Dtos
{ 
    public class MovieTorrent
    {
        public string DownloadUrl { get; set; }

        public string Quality { get; set; }
    }
}
