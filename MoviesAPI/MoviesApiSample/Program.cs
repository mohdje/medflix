
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MoviesAPI.Services.VOMovies;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services;
using System.Threading.Tasks;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Movies.Dtos;

namespace MoviesApiSample
{
    class Program
    {
        const string apiKey = Tokens.API_TOKEN;
        static void Main(string[] args)
        {
            Console.WriteLine("Test started");

            //SearchMovies("rocky");
            // GetMoviesOfToday();
           // GetPopularMovies();
            //GetPopularMoviesByGenre(16);
            //GetMoviesByGenre(10751);
            //GetTopNetflixMovies();
            // GetMovieDetails("436270");
            //GetPopularNetflixMovies();
            //GetPopularDisneyPlusMovies();
            //GetPopularAmazonPrimeMovies();


            //GetFrenchTitle("718930");
            //GetGenres();

            //SearchVfTorrents("american girl", 2022);
            //SearchVoTorrents("Project Gemini", 2022);
            GetSubtitles("tt1441105", SubtitlesLanguage.French);//tt16194408 "tt1441105"

            Console.ReadKey();
        }

        static async Task GetMoviesOfToday()
        {
            Console.WriteLine("Movies of the day");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetMoviesOfTodayAsync());
        }

        static async Task GetPopularMovies()
        {
            Console.WriteLine("Popular movies");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetPopularMoviesAsync());
        }

        static async Task GetPopularNetflixMovies()
        {
            Console.WriteLine("Popular Netflix movies");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetPopularNetflixMoviesAsync());
        }

        static async Task GetPopularDisneyPlusMovies()
        {
            Console.WriteLine("Popular DisneyPlus movies");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetPopularDisneyPlusMoviesAsync());
        }

        static async Task GetPopularAmazonPrimeMovies()
        {
            Console.WriteLine("Popular Amazon prime movies");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetPopularAmazonPrimeMoviesAsync());
        }

        static async Task GetPopularMoviesByGenre(int genreId)
        {
            Console.WriteLine($"Popular movies by genre : {genreId}");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetPopularMoviesByGenreAsync(genreId));
        }

        static async Task GetMoviesByGenre(int genreId)
        {
            Console.WriteLine($"Movies by genre : {genreId}");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetMoviesByGenreAsync(genreId, 1));
        }

        static async Task SearchMovies(string movieName)
        {
            Console.WriteLine($"Search movie: {movieName}");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.SearchMoviesAsync(movieName));
        }

        static async Task GetGenres()
        {
            Console.WriteLine("Genres");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            var genres = await movieSearcher.GetGenresAsync();

            foreach (var genre in genres)
            {
                Console.WriteLine(genre.Name);
            }
        }

        static async Task GetFrenchTitle(string movieId)
        {
            Console.WriteLine($"GetFrenchTitle {movieId}");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            var title = await movieSearcher.GetFrenchTitleAsync(movieId);

            Console.WriteLine(title);
        }

        static async Task GetTopNetflixMovies()
        {
            Console.WriteLine("Top Netflix movies");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetTopNetflixMoviesAsync());
        }

        static async Task GetMovieDetails(string movieId)
        {
            Console.WriteLine("Movie Details");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);


            var movie = await movieSearcher.GetMovieDetailsAsync(movieId);

            Console.WriteLine($"{movie.Id}. {movie.Title}  ({movie.Year}), {movie.Rating}, background:{movie.BackgroundImageUrl}, cover:{movie.CoverImageUrl}, synopsis: {movie.Synopsis}");
            Console.WriteLine($"Genre: {movie.Genre}");
            Console.WriteLine($"Director: {movie.Director}");
            Console.WriteLine($"Cast: {movie.Cast}");
            Console.WriteLine($"Duration: {movie.Duration}");
            Console.WriteLine($"Trailer: {movie.YoutubeTrailerUrl}");

        }

        static void ShowMoviesList(IEnumerable<LiteMovieDto> movies)
        {
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

        static async Task SearchVfTorrents(string frenchTitleMovie, int year)
        {
            Console.WriteLine($"Search vf torrents for {frenchTitleMovie}, {year}");
            var vfTorrentSearcher = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            var vfTorrents = await vfTorrentSearcher.SearchVfTorrentsAsync(frenchTitleMovie, year);

            if(vfTorrents == null || !vfTorrents.Any())
                Console.WriteLine("No torrent found");
            else
            {
                foreach (var vfTorrent in vfTorrents)
                {
                    Console.WriteLine($"Quanlity: {vfTorrent.Quality}, Url: {vfTorrent.DownloadUrl}");
                }
            }
        }

        static async Task SearchVoTorrents(string originalTitle, int year)
        {
            Console.WriteLine($"Search vo torrents for {originalTitle}, {year}");
            var voTorrentSearcher = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            var voTorrents = await voTorrentSearcher.SearchVoTorrentsAsync(originalTitle, year);

            if (voTorrents == null || !voTorrents.Any())
                Console.WriteLine("No torrent found");
            else
            {
                foreach (var vfTorrent in voTorrents)
                {
                    Console.WriteLine($"Quanlity: {vfTorrent.Quality}, Url: {vfTorrent.DownloadUrl}");
                }
            }
        }

        static async Task GetSubtitles(string imdbCode, SubtitlesLanguage language)
        {
            Console.WriteLine($"Search {language} subtitles for {imdbCode}");

            MoviesAPIFactory.Instance.SetSubtitlesFolder(AppContext.BaseDirectory);

            var subtitlesSearcher = await MoviesAPIFactory.Instance.CreateSubstitlesSearchManagerAsync();

            var availableSubtitlesUrls = await subtitlesSearcher.GetAvailableSubtitlesUrlsAsync(imdbCode, language);

            if (availableSubtitlesUrls == null || !availableSubtitlesUrls.Any())
                Console.WriteLine("No subtitles found");
            else
            {
                Console.WriteLine($"subtitles found:{language} - {string.Join(',', availableSubtitlesUrls)}");

                var subtitles = await subtitlesSearcher.GetSubtitlesAsync(availableSubtitlesUrls.Last());
                var counter = 0;
                foreach (var sub in subtitles)
                {
                    Console.WriteLine($"{sub.StartTime} - {sub.EndTime} : {sub.Text}");
                    counter++;

                    if (counter == 10)
                        break;
                }

            }
        }
    }
}
