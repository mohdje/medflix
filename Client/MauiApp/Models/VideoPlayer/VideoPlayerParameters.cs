using Medflix.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.VideoPlayer
{
    public class VideoPlayerParameters
    {
        public TorrentSources[] TorrentSources { get; set; }
        public SubtitlesSources[] SubtitlesSources { get; set; }
        public WatchMediaInfo WatchMedia { get; set; }

    }
}
