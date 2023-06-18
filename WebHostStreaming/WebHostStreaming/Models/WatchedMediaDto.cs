
using MoviesAPI.Services.Content.Dtos;

namespace WebHostStreaming.Models
{
    public class WatchedMediaDto
    {
        public string Id { get; set; }
        public string CoverImageUrl { get; set; }
        public string Synopsis { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public float Rating { get; set; }
        public float TotalDuration { get; set; }
        public float CurrentTime { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public Genre[] Genres { get; set; }
    }
}
