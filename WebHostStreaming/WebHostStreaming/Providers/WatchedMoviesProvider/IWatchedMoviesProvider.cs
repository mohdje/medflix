using MoviesAPI.Services.Movies.Dtos;
using System.Collections.Generic;

namespace WebHostStreaming.Providers
{
    public interface IWatchedMoviesProvider
    {
        void SaveWatchedMovie(LiteMovieDto movieToSave);
        IEnumerable<LiteMovieDto> GetWatchedMovies();
    }
}
