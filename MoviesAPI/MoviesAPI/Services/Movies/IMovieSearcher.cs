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
        Task<IEnumerable<LiteMovieDto>> GetSimilarMovies(string movieId);
        Task<MovieDto> GetMovieDetails(string movieId);
        Task<IEnumerable<LiteMovieDto>> GetMoviesByGenre(int genreId, int page);
        Task<IEnumerable<LiteMovieDto>> GetPopularMoviesByGenre(int genreId);
        Task<IEnumerable<LiteMovieDto>> GetTopNetflixMovies();
        Task<IEnumerable<Genre>> GetGenres();
    }
}
