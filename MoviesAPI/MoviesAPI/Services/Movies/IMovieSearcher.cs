using MoviesAPI.Services.Movies.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Movies
{
    public interface IMovieSearcher
    {
        Task<string> GetFrenchTitleAsync(string movieId);
        Task<IEnumerable<LiteMovieDto>> SearchMoviesAsync(string movieName);
        Task<IEnumerable<LiteMovieDto>> GetMoviesOfTodayAsync();
        Task<IEnumerable<LiteMovieDto>> GetPopularMoviesAsync();
        Task<IEnumerable<LiteMovieDto>> GetRecommandationsAsync(string movieId);
        Task<IEnumerable<LiteMovieDto>> GetSimilarMoviesAsync(string movieId);
        Task<MovieDto> GetMovieDetailsAsync(string movieId);
        Task<IEnumerable<LiteMovieDto>> GetMoviesByGenreAsync(int genreId, int page);
        Task<IEnumerable<LiteMovieDto>> GetPopularMoviesByGenreAsync(int genreId);
        Task<IEnumerable<LiteMovieDto>> GetTopNetflixMoviesAsync();
        Task<IEnumerable<LiteMovieDto>> GetPopularNetflixMoviesAsync();
        Task<IEnumerable<LiteMovieDto>> GetPopularDisneyPlusMoviesAsync();
        Task<IEnumerable<LiteMovieDto>> GetPopularAmazonPrimeMoviesAsync();
        Task<IEnumerable<Genre>> GetGenresAsync();
    }
}
