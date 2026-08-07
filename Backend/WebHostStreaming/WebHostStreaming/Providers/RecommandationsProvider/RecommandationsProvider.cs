using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class RecommandationsProvider : IRecommandationsProvider
    {
        IWatchedMoviesProvider watchedMoviesProvider;
        IWatchedSeriesProvider watchedSeriesProvider;
        ISearchersProvider searchersProvider;
        public RecommandationsProvider(IWatchedMoviesProvider watchedMoviesProvider, IWatchedSeriesProvider watchedSeriesProvider, ISearchersProvider searchersProvider)
        {
            this.watchedMoviesProvider = watchedMoviesProvider;
            this.watchedSeriesProvider = watchedSeriesProvider;
            this.searchersProvider = searchersProvider;
        }
        public async Task<IEnumerable<LiteContentDto>> GetMoviesRecommandationsAsync()
        {
            var watchedMovies = watchedMoviesProvider.GetWatchedMovies();
            return await GetRecommandationsAsync(watchedMovies, searchersProvider.MovieSearcher.GetSimilarMoviesAsync);
        }

        public async Task<IEnumerable<LiteContentDto>> GetSeriesRecommandationsAsync()
        {
            var watchedSeries = watchedSeriesProvider.GetWatchedSeries();
            return await GetRecommandationsAsync(watchedSeries, searchersProvider.SeriesSearcher.GetSimilarSeriesAsync);
        }

        private async Task<IEnumerable<LiteContentDto>> GetRecommandationsAsync(IEnumerable<WatchedMediaDto> watchedMedias, Func<string, Task<IEnumerable<LiteContentDto>>> GetSimilarMediasAsync)
        {
            var getSimilarMediasTasks = watchedMedias.DistinctBy(wm => wm.Media.Id).Take(3).Select(wm => GetSimilarMediasAsync(wm.Media.Id));

            var similarMedias = await Task.WhenAll(getSimilarMediasTasks);

            return similarMedias.SelectMany(m => m)
                .Where(m => watchedMedias.Any(wm => wm.Media.Id != m.Id))
                .OrderByDescending(m => m.Rating)
                .Take(15);
        }
    }
}
