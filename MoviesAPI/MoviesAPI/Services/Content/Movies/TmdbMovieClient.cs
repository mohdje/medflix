using MoviesAPI.Services.Tmdb;
using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Services.Tmdb.Dtos;

namespace MoviesAPI.Services.Content
{
    internal class TmdbMovieClient : TmdbClient, IMovieSearcher
    {
        public TmdbMovieClient(string apiKey) : base(new TmdbMovieUrlBuilder(apiKey))
        {

        }

        public async Task<ContentDto> GetMovieDetailsAsync(string movieId)
        {
            return await GetContentDetailsAsync(movieId);
        }

        public async Task<IEnumerable<LiteContentDto>> GetMoviesByGenreAsync(int genreId, int page)
        {
            return await GetContentByGenreAsync(genreId, page);
        }

        public async Task<IEnumerable<LiteContentDto>> GetMoviesOfTodayAsync()
        {
            return await GetTrendingOfTodayAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularAmazonPrimeMoviesAsync()
        {
            return await GetPopularAmazonPrimeContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularDisneyPlusMoviesAsync()
        {
            return await GetPopularDisneyPlusContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularMoviesAsync()
        {
            return await GetPopularContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularMoviesByGenreAsync(int genreId)
        {
            return await GetPopularContentByGenreAsync(genreId);
        }

        public async Task<IEnumerable<LiteContentDto>> GetPopularNetflixMoviesAsync()
        {
            return await GetPopularNetflixContentAsync();
        }

        public async Task<IEnumerable<LiteContentDto>> GetRecommandationsAsync(string movieId)
        {
            return await GetRecommandedContentAsync(movieId);
        }

        public async Task<IEnumerable<LiteContentDto>> GetSimilarMoviesAsync(string movieId)
        {
            return await GetSimilarContentAsync(movieId);
        }

        public async Task<IEnumerable<LiteContentDto>> SearchMoviesAsync(string movieName)
        {
            return await SearchContentAsync(movieName);
        }

        public async Task<string> GetMovieFrenchTitleAsync(string movieId)
        {
            return await GetFrenchTitleAsync(movieId);
        }

        public async Task<IEnumerable<Genre>> GetMovieGenresAsync()
        {
            var genres = await GetGenresAsync();

            var genreIdsToRemove = new int[] { 10402, 9648, 10770, 10752, 37, 36 };

            return genres != null ? genres.Where(g => !genreIdsToRemove.Contains(g.Id)) : new Genre[0];
        }
    }
}
