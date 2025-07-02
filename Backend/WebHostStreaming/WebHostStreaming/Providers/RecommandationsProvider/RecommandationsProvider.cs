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
            var recommandationsRequest = BuildRecommandationsRequest(watchedMovies);

            if (recommandationsRequest == null)
                return Array.Empty<LiteContentDto>();

            return await searchersProvider.MovieSearcher.GetRecommandationsAsync(
                recommandationsRequest.GenreIds,
                recommandationsRequest.MinDate,
                recommandationsRequest.MaxDate,
                recommandationsRequest.ExcludedMediasIds);
        }

        public async Task<IEnumerable<LiteContentDto>> GetSeriesRecommandationsAsync()
        {
            var watchedSeries = watchedSeriesProvider.GetWatchedSeries();
            var recommandationsRequest = BuildRecommandationsRequest(watchedSeries);

            if (recommandationsRequest == null)
                return new LiteContentDto[0];

            return await searchersProvider.SeriesSearcher.GetRecommandationsAsync(
                recommandationsRequest.GenreIds,
                recommandationsRequest.MinDate,
                recommandationsRequest.MaxDate,
                recommandationsRequest.ExcludedMediasIds);
        }

        private RecommandationsRequest BuildRecommandationsRequest(IEnumerable<WatchedMediaDto> watchedMedias)
        {
            if (watchedMedias == null || !watchedMedias.Any())
                return null;
            else
            {
                var watchedMediasIds = watchedMedias.Select(w => w.Media.Id).Distinct();
                var minDate = GetMinDate(watchedMedias);
                var maxDate = GetMaxDate(watchedMedias);

                var allGenresIds = watchedMedias.Where(w => w.Media.Genres != null && w.Media.Genres.Any()).TakeLast(3).Reverse().SelectMany(w => w.Media.Genres.Select(genre => genre.Id));
                var genresCount = allGenresIds.Distinct().Select(genreId => new { Id = genreId, Count = allGenresIds.Count(gId => genreId == gId) });

                var selectedGenresIds = genresCount.OrderByDescending(g => g.Count).Select(g => g.Id.ToString()).Take(3);

                return new RecommandationsRequest(selectedGenresIds.ToArray(), minDate, maxDate, watchedMediasIds.ToArray());
            }
        }

        private string GetMinDate(IEnumerable<WatchedMediaDto> watchedMedias)
        {
            var year = watchedMedias.Min(w => w.Media.Year);
            return $"{year}-01-01";
        }
        private string GetMaxDate(IEnumerable<WatchedMediaDto> watchedMedias)
        {
            var year = watchedMedias.Max(w => w.Media.Year);
            return DateTime.Now.Year == year ? DateTime.Now.ToString("yyyy-MM-dd") : $"{year}-12-31";

        }
    }
}
