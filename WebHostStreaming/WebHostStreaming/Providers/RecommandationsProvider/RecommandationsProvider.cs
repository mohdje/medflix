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
        IWatchedMediaProvider watchedMediaProvider;
        ISearchersProvider searchersProvider;
        public RecommandationsProvider(IWatchedMediaProvider watchedMediaProvider, ISearchersProvider searchersProvider)
        {
            this.watchedMediaProvider = watchedMediaProvider;
            this.searchersProvider = searchersProvider;
        }
        public async Task<IEnumerable<LiteContentDto>> GetMoviesRecommandationsAsync()
        {
            var watchedMovies = await watchedMediaProvider.GetWatchedMoviesAsync();
            var recommandationsRequest = BuildRecommandationsRequest(watchedMovies);

            if (recommandationsRequest == null)
                return new LiteContentDto[0];

            return await searchersProvider.MovieSearcher.GetRecommandationsAsync(
                recommandationsRequest.GenreIds,
                recommandationsRequest.MinDate,
                recommandationsRequest.MaxDate,
                recommandationsRequest.ExcludedMediasIds);
        }

        public async Task<IEnumerable<LiteContentDto>> GetSeriesRecommandationsAsync()
        {
            var watchedSeries = await watchedMediaProvider.GetWatchedSeriesAsync();
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
                var watchedMediasIds = watchedMedias.Select(media => media.Id).Distinct();
                var minDate = GetMinDate(watchedMedias);
                var maxDate = GetMaxDate(watchedMedias);

                var allGenresIds = watchedMedias.Where(media => media.Genres != null && media.Genres.Any()).TakeLast(3).Reverse().SelectMany(movie => movie.Genres.Select(genre => genre.Id));
                var genresCount = allGenresIds.Distinct().Select(genreId => new { Id = genreId, Count = allGenresIds.Count(gId => genreId == gId) });

                var selectedGenresIds = genresCount.OrderByDescending(g => g.Count).Select(g => g.Id.ToString()).Take(3);

                return new RecommandationsRequest(selectedGenresIds.ToArray(), minDate, maxDate, watchedMediasIds.ToArray());
            }
        }

        private string GetMinDate(IEnumerable<WatchedMediaDto> watchedMedias)
        {
            var year = watchedMedias.Min(media => media.Year);
            return $"{year}-01-01";
        }
        private string GetMaxDate(IEnumerable<WatchedMediaDto> watchedMedias)
        {
            var year = watchedMedias.Max(media => media.Year);
            return DateTime.Now.Year == year ? DateTime.Now.ToString("yyyy-MM-dd") : $"{year}-12-31";

        }
    }
}
