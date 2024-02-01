using WebHostStreaming.Models;

namespace Medflix.Models
{
    public class VideoPlayerOptions
    {
        public VideoOption[] Sources { get; set; }
        public SubtitleOption[] Subtitles { get; set; }
        public int ResumeToTime { get; set; }
        public WatchedMediaDto WatchedMedia { get; set; }
        public string MediaType { get; set; }
    }
}
