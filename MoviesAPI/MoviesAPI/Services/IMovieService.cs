using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieDto>> GetSuggestedMoviesAsync(int nbMovies);

        Task<IEnumerable<MovieDto>> GetLastMoviesByGenreAsync(int nbMovies, string genre);

        Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(string genre, int page);

        Task<IEnumerable<MovieDto>> GetMoviesByNameAsync(string name);

        Task<MovieDto> GetMoviesDetailsAsync(string movieId);

        IEnumerable<string> GetMovieGenres();
    }
}
