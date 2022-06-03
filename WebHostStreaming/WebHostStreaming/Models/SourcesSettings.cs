using MoviesAPI.Services;

namespace WebHostStreaming.Models
{
    public class SourcesSettings
    {
        public VFMoviesService VFMoviesService { get; set; }
        public VOMovieService VOMovieService { get; set; }
        public SubtitlesService SubtitlesService { get; set; }
    }
}
