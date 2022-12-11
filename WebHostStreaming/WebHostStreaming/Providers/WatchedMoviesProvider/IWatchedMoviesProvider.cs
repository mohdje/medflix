using MoviesAPI.Services.Movies.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface IWatchedMoviesProvider
    {
        Task SaveWatchedMovieAsync(LiteMovieDto movieToSave);
        Task<IEnumerable<LiteMovieDto>> GetWatchedMoviesAsync();
    }
}
