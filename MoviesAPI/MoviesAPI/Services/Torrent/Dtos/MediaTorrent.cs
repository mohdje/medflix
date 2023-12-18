using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Torrent.Dtos
{ 
    public class MediaTorrent
    {
        public string DownloadUrl { get; set; }

        public string Quality { get; set; }

        public string LanguageVersion { get; set; }
    }
}
