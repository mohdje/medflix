using Medflix.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.VideoPlayer
{
    public class TorrentSources
    {
        public string Language { get; set; }

        public MediaTorrent[] Torrents { get; set; }
    }
}
