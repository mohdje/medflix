using System.Collections.Generic;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using System.Linq;

namespace WebHostStreaming.Providers
{
    public class WatchedMoviesProvider : DataStoreProvider<WatchedMediaDto>, IWatchedMoviesProvider
    {
        protected override int MaxLimit => 30;

        protected override string FilePath => AppFiles.WatchedMovies;

        public WatchedMediaDto GetWatchedMovie(int movieId)
        {
            return Data.FirstOrDefault(wm => wm.Media.Id == movieId.ToString());
        }

        public IEnumerable<WatchedMediaDto> GetWatchedMovies()
        {
            return Data.Reverse().Take(MaxLimit);
        }

        public void SaveWatchedMovie(WatchedMediaDto watchedMovie)
        {
            if (Data.Any(wm => wm.Media.Id == watchedMovie.Media.Id))
                UpdateData(watchedMovie);
            else
                AddData(watchedMovie);
        }
    }
}
