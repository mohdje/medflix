using Medflix.Models.Media;

namespace Medflix.Models.VideoPlayer
{
    public class VideoPlayerParameters
    {
        public MediaSource[] MediaSources { get; set; }
        public SubtitlesSources[] SubtitlesSources { get; set; }
        public WatchMediaInfo WatchMedia { get; set; }
    }
}
