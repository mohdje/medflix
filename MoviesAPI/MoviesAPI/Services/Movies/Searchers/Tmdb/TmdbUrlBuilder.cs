using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Movies
{
    internal class TmdbUrlBuilder
    {
        private readonly string apiKey;

        private const string BaseUrl = "https://api.themoviedb.org/3";

        public TmdbUrlBuilder(string apiKey)
        {
            this.apiKey = apiKey;
        }
        public string BuildSearchUrl(string title)
        {
            return $"{BaseUrl}/search/movie?api_key={apiKey}&language=en-US&query={title}&page=1&include_adult=false";
        }

        public string BuildTranslationUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/movie/{tmdbMovieId}/translations?api_key={apiKey}";
        }

        public string BuildRecommandationsUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/movie/{tmdbMovieId}/recommendations?api_key={apiKey}&page=1";
        }

        public string BuildSimilarMoviesUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/movie/{tmdbMovieId}/similar?api_key={apiKey}&page=1";
        }

        public string BuildMoviesOfTodayUrl()
        {
            return $"{BaseUrl}/trending/movie/day?api_key={apiKey}";
        }

        public string BuildPopularMoviesUrl()
        {
            return $"{BaseUrl}/movie/popular?api_key={apiKey}";
        }

        public string BuildMoviesByGenreUrl(int genreId, int page)
        {
            var today = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            return $"{BaseUrl}/discover/movie?api_key={apiKey}&sort_by=release_date.desc&include_adult=false&with_genres={genreId}&release_date.lte={today}&page={page}&vote_count.gte=15";
        }

        public string BuildCoverImageUrl(string imageName)
        {

            return string.IsNullOrEmpty(imageName) ? null : $"https://image.tmdb.org/t/p/w300_and_h450_bestv2/{imageName.Replace("/","")}";
        }

        public string BuildBackgroundImageUrl(string imageName)
        {
            return string.IsNullOrEmpty(imageName) ? null : $"https://image.tmdb.org/t/p/w1920_and_h1080_bestv2/{imageName.Replace("/", "")}";
        }

        public string BuildMovieDetailsUrl(string tmdbMovieId, string language = null)
        {
            var languageParam = string.IsNullOrEmpty(language) ? "en-US" : language;

            return $"{BaseUrl}/movie/{tmdbMovieId}?api_key={apiKey}&language={languageParam}&append_to_response=videos";
        }

        public string BuildMovieImagesUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/movie/{tmdbMovieId}/images?api_key={apiKey}";
        }

        public string BuilGenresListUrl()
        {
            return $"{BaseUrl}/genre/movie/list?api_key={apiKey}";
        }

        public string BuildPopularMoviesByGenreUrl(int genreId)
        {
            var date = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");

            return $"{BaseUrl}/discover/movie?api_key={apiKey}&sort_by=popularity.desc&include_adult=false&include_video=false&page=1&release_date.lte={date}&vote_average.gte=5&with_genres={genreId}&vote_count.gte=15";
        }

        public string BuildGetCreditsUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/movie/{tmdbMovieId}/credits?api_key={apiKey}";
        }

        //public string BuildPopularMoviesByPlatformUrl(int platformId)
        //{
        //    var date = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");

        //    return $"{BaseUrl}/discover/movie?api_key={apiKey}&sort_by=popularity.desc&include_adult=false&include_video=false&page=1&release_date.lte={date}&vote_average.gte=5&with_genres={genreId}&vote_count.gte=15";
        //}
    }
}
