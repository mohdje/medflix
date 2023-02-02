using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class WatchedMediaProvider : DataProvider, IWatchedMediaProvider
    {
        private readonly string WatchedMoviesFile = AppFiles.WatchedMovies;
        private readonly string WatchedSeriesFile = AppFiles.WatchedSeries;

        protected override int MaxLimit()
        {
            return 30;
        }

        public async Task<IEnumerable<WatchedMediaDto>> GetWatchedMoviesAsync()
        {
            return await GetDataAsync<WatchedMediaDto>(WatchedMoviesFile);
        }

        public async Task SaveWatchedMovieAsync(WatchedMediaDto movieToSave)
        {
            await SaveDataAsync(WatchedMoviesFile, movieToSave, (m1, m2) => m1.Id == m2.Id, true);
        }

        public async Task SaveWatchedSerieAsync(WatchedMediaDto serieToSave)
        {
            await SaveDataAsync(WatchedSeriesFile, serieToSave, (m1, m2) => m1.Id == m2.Id && m1.SeasonNumber == m2.SeasonNumber && m1.EpisodeNumber == m2.EpisodeNumber, true);
        }

        public async Task<IEnumerable<WatchedMediaDto>> GetWatchedSeriesAsync()
        {
            return await GetDataAsync<WatchedMediaDto>(WatchedSeriesFile);
        }
    }
}
