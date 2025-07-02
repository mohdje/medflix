using System.Collections.Generic;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IWatchedMoviesProvider
    {
        void SaveWatchedMovie(WatchedMediaDto movieToSave);
        WatchedMediaDto GetWatchedMovie(int movieId);
        IEnumerable<WatchedMediaDto> GetWatchedMovies();
    }
}
