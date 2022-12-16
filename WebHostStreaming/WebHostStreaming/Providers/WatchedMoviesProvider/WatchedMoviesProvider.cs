using MoviesAPI.Services.Movies.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class WatchedMoviesProvider : DataProvider, IWatchedMoviesProvider
    {
        protected override string FilePath()
        {
            return AppFiles.WatchedMovies;
        }

        protected override int MaxLimit()
        {
            return 30;
        }

        public async Task<IEnumerable<WatchedMovieDto>> GetWatchedMoviesAsync()
        {
            return await GetDataAsync<WatchedMovieDto>();
        }

        public async Task SaveWatchedMovieAsync(WatchedMovieDto movieToSave)
        {
            await SaveDataAsync(movieToSave, (m1, m2) => m1.Id == m2.Id, true);
        }

      
    }
}
