using MoviesAPI.Services;
using MoviesAPI.Services.Content;
using MoviesAPI.Services.Content.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesApiSample.Samples
{
    internal class MoviesSample
    {
        IMovieSearcher movieSearcher;
        public MoviesSample(string apiKey)
        {
            movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);
        }

        public async Task Test()
        {
            //await SearchMovies("glass onion");
            //await GetMoviesOfToday();

            //await GetPopularMovies();
            //await GetPopularMoviesByGenre(16);
            //await GetMoviesByGenre(10751);
            // await GetMovieDetails("436270");
            //await GetPopularNetflixMovies();
            //await GetPopularDisneyPlusMovies();
            //await GetPopularAmazonPrimeMovies();
            //await GetRecommandedMovies();
            //await GetFrenchTitle("661374");
            //await GetGenres();
           // await GetPlatforms();
            await GetMoviesByPlatform(2);
        }
        private async Task GetMoviesOfToday()
        {
            ShowMoviesList(await movieSearcher.GetMoviesOfTodayAsync(), "Movies of the day");
        }

        private async Task GetPopularMovies()
        {
            ShowMoviesList(await movieSearcher.GetPopularMoviesAsync(), "Popular movies");
        }

        private async Task GetRecommandedMovies()
        {
            var genres = new string[] { "16", "12", "28" };
            var minDate = "2015-01-01";
            var maxDate = "2022-12-31";
            var exludedMovies = new string[] { "436270", "1033219" }; 
            ShowMoviesList(await movieSearcher.GetRecommandationsAsync(genres, minDate, maxDate, exludedMovies), "Recommanded movies");
        }

        private async Task GetPopularNetflixMovies()
        {
            ShowMoviesList(await movieSearcher.GetPopularNetflixMoviesAsync(), "Popular Netflix movies");
        }

        private async Task GetPopularDisneyPlusMovies()
        {
            ShowMoviesList(await movieSearcher.GetPopularDisneyPlusMoviesAsync(), "Popular DisneyPlus movies");
        }

        private async Task GetPopularAmazonPrimeMovies()
        {
            ShowMoviesList(await movieSearcher.GetPopularAmazonPrimeMoviesAsync(), "Popular Amazon prime movies");
        }

        private async Task GetPopularMoviesByGenre(int genreId)
        {
            ShowMoviesList(await movieSearcher.GetPopularMoviesByGenreAsync(genreId), $"Popular movies by genre : {genreId}");
        }

        private async Task GetMoviesByGenre(int genreId)
        {
            ShowMoviesList(await movieSearcher.GetMoviesByGenreAsync(genreId, 1), $"Movies by genre : {genreId}");
        }

        private async Task GetMoviesByPlatform(int platformId)
        {
            ShowMoviesList(await movieSearcher.GetMoviesByPlatformAsync(platformId, 1), $"Movies by platform : {platformId}");
        }

        private async Task SearchMovies(string movieName)
        {
            ShowMoviesList(await movieSearcher.SearchMoviesAsync(movieName), $"Search movie: {movieName}");
        }

        private async Task GetGenres()
        {
            Console.WriteLine("Genres");

            var genres = await movieSearcher.GetMovieGenresAsync();

            foreach (var genre in genres)
            {
                Console.WriteLine(genre.Name);
            }
        }

        private async Task GetPlatforms()
        {
            Console.WriteLine("Platforms");

            var platforms = await movieSearcher.GetMoviePlatformsAsync();

            foreach (var platform in platforms)
            {
                Console.WriteLine($"{platform.Id}:{platform.Name}");
            }
        }

        private async Task GetFrenchTitle(string movieId)
        {
            Console.WriteLine($"GetFrenchTitle {movieId}");

            var title = await movieSearcher.GetMovieFrenchTitleAsync(movieId);

            Console.WriteLine(title);
        }

        private async Task GetMovieDetails(string movieId)
        {
            Console.WriteLine("Movie Details");

            var movie = await movieSearcher.GetMovieDetailsAsync(movieId);

            Console.WriteLine($"{movie.Id}. {movie.Title}  ({movie.Year}), {movie.Rating}, background:{movie.BackgroundImageUrl}, cover:{movie.CoverImageUrl}, synopsis: {movie.Synopsis}");
            Console.WriteLine($"Genre: {movie.Genres.Select(genre => genre.Name).Aggregate((a,b) => $"{a}, {b}")}");
            Console.WriteLine($"Director: {movie.Director}");
            Console.WriteLine($"Cast: {movie.Cast}");
            Console.WriteLine($"Duration: {movie.Duration}");
            Console.WriteLine($"Trailer: {movie.YoutubeTrailerUrl}");
            Console.WriteLine($"ImdbId: {movie.ImdbId}");

        }

        private void ShowMoviesList(IEnumerable<LiteContentDto> movies, string operationInfo)
        {
            Console.WriteLine(operationInfo);

            if (movies == null || !movies.Any())
                Console.WriteLine("No result");
            else
            {
                foreach (var movie in movies)
                {
                    Console.WriteLine($"{movie.Id}. {movie.Title}  ({movie.Year}), {movie.Rating}, background:{movie.BackgroundImageUrl}, cover:{movie.CoverImageUrl}, logo:{movie.LogoImageUrl}, synopsis: {movie.Synopsis}");
                }
            }
        }

    }
}
