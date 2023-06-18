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
    internal class SeriesSample
    {
        ISeriesSearcher seriesSearcher;
        public SeriesSample(string apiKey)
        {
            seriesSearcher = MoviesAPIFactory.Instance.CreateSeriesSearcher(apiKey);
        }

        public async Task Test()
        {
            //await SearchSeries("patriot act");
            //await GetSeriesOfToday();

            //await GetPopularSeries();
            //await GetPopularSeriesByGenre(16);
            //await GetSeriesByGenre(10767);
            await GetSerieDetails("119051");
            //await GetPopularNetflixSeries();
            //await GetPopularDisneyPlusSeries();
            //await GetPopularAmazonPrimeSeries();

           // await GetRecommandedSeries();

           // await GetFrenchTitle("119051");
           // await GetGenres();

            //GetSerieEpisodesBySeason("1405", 2);
        }
        private async Task GetSeriesOfToday()
        {
            ShowSeriesList(await seriesSearcher.GetSeriesOfTodayAsync(), "Series of the day");
        }
        private async Task GetPopularSeries()
        {
            ShowSeriesList(await seriesSearcher.GetPopularSeriesAsync(), "Popular Series");
        }

        private async Task GetRecommandedSeries()
        {
            var genres = new string[] { "80", "35" };
            var minDate = "2018-01-01";
            var maxDate = "2022-12-31";
            var exludedMovies = new string[] { "72879", "4614" };
            ShowSeriesList(await seriesSearcher.GetRecommandationsAsync(genres, minDate, maxDate, exludedMovies), "Recommanded series");
        }

        private async Task GetPopularNetflixSeries()
        {
            ShowSeriesList(await seriesSearcher.GetPopularNetflixSeriesAsync(), "Popular Netflix series");
        }

        private async Task GetPopularDisneyPlusSeries()
        {
            ShowSeriesList(await seriesSearcher.GetPopularDisneyPlusSeriesAsync(), "Popular DisneyPlus series");
        }

        private async Task GetPopularAmazonPrimeSeries()
        {
            ShowSeriesList(await seriesSearcher.GetPopularAmazonPrimeSeriesAsync(), "Popular Amazon prime series");
        }

        private async Task GetPopularSeriesByGenre(int genreId)
        {
            ShowSeriesList(await seriesSearcher.GetPopularSeriesByGenreAsync(genreId), $"Popular series by genre : {genreId}");
        }

        private async Task GetSeriesByGenre(int genreId)
        {
            ShowSeriesList(await seriesSearcher.GetSeriesByGenreAsync(genreId, 1), $"Series by genre : {genreId}");
        }

        private async Task SearchSeries(string movieName)
        {
            ShowSeriesList(await seriesSearcher.SearchSeriesAsync(movieName), $"Search serie: {movieName}");
        }

        private async Task GetGenres()
        {
            Console.WriteLine("Genres");

            var genres = await seriesSearcher.GetSerieGenresAsync();

            foreach (var genre in genres)
            {
                Console.WriteLine(genre.Name);
            }
        }

        private async Task GetFrenchTitle(string serieId)
        {
            Console.WriteLine($"GetFrenchTitle {serieId}");

            var title = await seriesSearcher.GetSerieFrenchTitleAsync(serieId);

            Console.WriteLine(title);
        }

        private async Task GetSerieDetails(string serieId)
        {
            Console.WriteLine("Serie Details");

            var serie = await seriesSearcher.GetSerieDetailsAsync(serieId);

            Console.WriteLine($"{serie.Id}. {serie.Title}  ({serie.Year}), {serie.Rating}, background:{serie.BackgroundImageUrl}, cover:{serie.CoverImageUrl}, synopsis: {serie.Synopsis}");
            Console.WriteLine($"Genre: {serie.Genres.Select(genre => genre.Name).Aggregate((a, b) => $"{a}, {b}")}");
            Console.WriteLine($"Director: {serie.Director}");
            Console.WriteLine($"Cast: {serie.Cast}");
            Console.WriteLine($"Trailer: {serie.YoutubeTrailerUrl}");
            Console.WriteLine($"Seasons: {serie.SeasonsCount}");
            Console.WriteLine($"ImdbId: {serie.ImdbId}");

        }

        private async Task GetSerieEpisodesBySeason(string serieId, int seasonNumber)
        {
            Console.WriteLine("Serie Episodes");

            var episodes = await seriesSearcher.GetEpisodes(serieId, seasonNumber);

            foreach (var episode in episodes)
            {
                Console.WriteLine($"Name: {episode.Name}");
                Console.WriteLine($"EpisodeNumber: {episode.EpisodeNumber}");
                Console.WriteLine($"RunTime: {episode.RunTime}");
                Console.WriteLine($"Overview: {episode.Overview}");
                Console.WriteLine($"ImagePath: {episode.ImagePath}");
                Console.WriteLine("#############");

            }


        }

        private void ShowSeriesList(IEnumerable<LiteContentDto> series, string operationInfo)
        {
            Console.WriteLine(operationInfo);

            if (series == null || !series.Any())
                Console.WriteLine("No result");
            else
            {
                foreach (var serie in series)
                {
                    Console.WriteLine($"{serie.Id}. {serie.Title}  ({serie.Year}), {serie.Rating}, background:{serie.BackgroundImageUrl}, cover:{serie.CoverImageUrl}, logo:{serie.LogoImageUrl}, synopsis: {serie.Synopsis}");
                }
            }
        }
    }
}
