
using MoviesAPI.Services.Content.Dtos;

namespace WebHostStreaming.Models
{
    public class WatchedMediaDto : LiteContentDto
    {
        public float Progression { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
    }
}
