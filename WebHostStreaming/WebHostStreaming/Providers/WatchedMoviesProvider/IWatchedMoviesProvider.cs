using MoviesAPI.Services.Movies.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IWatchedMoviesProvider
    {
        Task SaveWatchedMovieAsync(WatchedMovieDto movieToSave);
        Task<IEnumerable<WatchedMovieDto>> GetWatchedMoviesAsync();
    }
}
