using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.VOMovies
{
    public abstract class VOMovieSearcher : BaseService
    {
        public abstract Task<IEnumerable<MovieDto>> GetSuggestedMoviesAsync(int nbMovies);

        public abstract Task<IEnumerable<MovieDto>> GetLastMoviesByGenreAsync(int nbMovies, string genre);

        public abstract Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(string genre, int page);

        public abstract Task<IEnumerable<MovieDto>> GetMoviesByNameAsync(string name);

        public abstract Task<MovieDto> GetMovieDetailsAsync(string movieId);

        public abstract IEnumerable<string> GetMovieGenres();
    }
}
