using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IWatchedMediaProvider
    {
        Task SaveWatchedMovieAsync(WatchedMediaDto movieToSave);
        Task<IEnumerable<WatchedMediaDto>> GetWatchedMoviesAsync();
        Task SaveWatchedSerieAsync(WatchedMediaDto movieToSave);
        Task<IEnumerable<WatchedMediaDto>> GetWatchedSeriesAsync();
    }
}
