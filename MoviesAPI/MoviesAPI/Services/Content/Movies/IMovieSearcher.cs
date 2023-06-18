using MoviesAPI.Services.Content.Dtos;
using MoviesAPI.Services.Tmdb.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Content
{
    public interface IMovieSearcher
    {
        Task<string> GetMovieFrenchTitleAsync(string movieId);
        Task<IEnumerable<LiteContentDto>> SearchMoviesAsync(string movieName);
        Task<IEnumerable<LiteContentDto>> GetMoviesOfTodayAsync();
        Task<IEnumerable<LiteContentDto>> GetPopularMoviesAsync();
        Task<IEnumerable<LiteContentDto>> GetRecommandationsAsync(string[] genreIds, string minDate, string maxDate, string[] excludedTmdbContentIds);
        Task<IEnumerable<LiteContentDto>> GetSimilarMoviesAsync(string movieId);
        Task<ContentDto> GetMovieDetailsAsync(string movieId);
        Task<IEnumerable<LiteContentDto>> GetMoviesByGenreAsync(int genreId, int page);
        Task<IEnumerable<LiteContentDto>> GetPopularMoviesByGenreAsync(int genreId);
        Task<IEnumerable<LiteContentDto>> GetPopularNetflixMoviesAsync();
        Task<IEnumerable<LiteContentDto>> GetPopularDisneyPlusMoviesAsync();
        Task<IEnumerable<LiteContentDto>> GetPopularAmazonPrimeMoviesAsync();
        Task<IEnumerable<Genre>> GetMovieGenresAsync();
    }
}
