
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
        const string apiKey = "a45ba9acb6490460d706a8cca2817a63";
        static void Main(string[] args)
        {
            Console.WriteLine("Test started");

            //SearchMovies("rocky");
            //GetMoviesOfToday();
            //GetPopularMovies();
            //GetPopularMoviesByGenre(16);
            //GetMoviesByGenre(18);
            //GetTopNetflixMovies();
            GetMovieDetails("436270");

            //GetGenres();

            //SearchVfTorrents("american girl", 2022);
            //SearchVoTorrents("dza ve", 2022);
            // GetSubtitles("tt5315212", SubtitlesLanguage.French);

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

        static async Task GetPopularMoviesByGenre(int genreId)
        {
            Console.WriteLine($"Popular movies by genre : {genreId}");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetPopularMoviesByGenre(genreId));
        }

        static async Task GetMoviesByGenre(int genreId)
        {
            Console.WriteLine($"Movies by genre : {genreId}");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetMoviesByGenre(genreId, 1));
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

            var genres = await movieSearcher.GetGenres();

            foreach (var genre in genres)
            {
                Console.WriteLine(genre.Name);
            }
        }

        static async Task GetTopNetflixMovies()
        {
            Console.WriteLine("Top Netflix movies");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);

            ShowMoviesList(await movieSearcher.GetTopNetflixMovies());
        }

        static async Task GetMovieDetails(string movieId)
        {
            Console.WriteLine("Movie Details");

            var movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(apiKey);


            var movie = await movieSearcher.GetMovieDetails(movieId);

            Console.WriteLine($"{movie.MovieId}. {movie.Title}  ({movie.Year}), {movie.Rating}, background:{movie.BackgroundImageUrl}, cover:{movie.CoverImageUrl}, synopsis: {movie.Synopsis}");
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
                    Console.WriteLine($"{movie.MovieId}. {movie.Title}  ({movie.Year}), {movie.Rating}, background:{movie.BackgroundImageUrl}, cover:{movie.CoverImageUrl}, synopsis: {movie.Synopsis}");
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
            Console.WriteLine($"Search vf torrents for {originalTitle}, {year}");
            var vfTorrentSearcher = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            var vfTorrents = await vfTorrentSearcher.SearchVoTorrentsAsync(originalTitle, year);

            if (vfTorrents == null || !vfTorrents.Any())
                Console.WriteLine("No torrent found");
            else
            {
                foreach (var vfTorrent in vfTorrents)
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
