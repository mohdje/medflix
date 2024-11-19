
using MoviesAPI.Services.Content.Dtos;

namespace WebHostStreaming.Models
{
    public class WatchedMediaDto
    {
        public ContentDto Media { get; set; }
        public float TotalDuration { get; set; }
        public float CurrentTime { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string VideoSource { get; set; }
    }
}
