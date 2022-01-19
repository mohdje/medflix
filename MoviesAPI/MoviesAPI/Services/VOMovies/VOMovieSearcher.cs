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

        public async Task<IEnumerable<MovieDto>> GetTopNetflixMovies()
        {
            var netflixMoviesNames = await Helpers.NetflixRequester.GetTopNetflixMoviesNameAsync();

            if (netflixMoviesNames == null || !netflixMoviesNames.Any())
                return null;

            var getMovieDtoTasks = new List<Task>();

            var movieDtos = new List<MovieDto>();

            foreach (var movieName in netflixMoviesNames)
            {
                getMovieDtoTasks.Add(new Task(() =>
                {
                    var searchResult = GetMoviesByNameAsync(movieName).Result;

                    var topMovie = searchResult?.FirstOrDefault(r => r.Title.Equals(movieName, StringComparison.OrdinalIgnoreCase));

                    if (topMovie != null) 
                        movieDtos.Add(topMovie);

                }));
            }

            getMovieDtoTasks.ForEach(t => t.Start());

            Task.WaitAll(getMovieDtoTasks.ToArray());

            return movieDtos;
        }
    }
}
