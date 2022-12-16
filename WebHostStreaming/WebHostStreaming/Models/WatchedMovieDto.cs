using MoviesAPI.Services.Movies.Dtos;

namespace WebHostStreaming.Models
{
    public class WatchedMovieDto : LiteMovieDto
    {
        public float Progression { get; set; }
    }
}
