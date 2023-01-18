using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Tmdb
{
    internal abstract class TmdbUrlBuilder
    {
        private readonly string apiKey;
        private readonly string ContentMode;

        private const string BaseUrl = "https://api.themoviedb.org/3";
        private const string BaseImageUrl = "https://image.tmdb.org/t/p";

        public TmdbUrlBuilder(string apiKey, TmdbUrlMode tmdbUrlMode)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException("apiKey cannot be null or empty");

            this.apiKey = apiKey;
            this.ContentMode = GetContentMode(tmdbUrlMode);
        }

        private string GetContentMode(TmdbUrlMode tmdbUrlMode)
        {
            switch (tmdbUrlMode)
            {
                case TmdbUrlMode.Movies:
                    return "movie";
                case TmdbUrlMode.Series:
                    return "tv";
                default:
                    return string.Empty;
            }
        }
        public string BuildSearchUrl(string title)
        {
            return $"{BaseUrl}/search/{ContentMode}?api_key={apiKey}&language=en-US&query={title}&page=1&include_adult=false";
        }

        public string BuildTranslationUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/{ContentMode}/{tmdbMovieId}/translations?api_key={apiKey}";
        }

        public string BuildRecommandationsUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/{ContentMode}/{tmdbMovieId}/recommendations?api_key={apiKey}&page=1";
        }

        public string BuildSimilarContentUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/{ContentMode}/{tmdbMovieId}/similar?api_key={apiKey}&page=1";
        }

        public string BuildTrendingOfTodayUrl()
        {
            return $"{BaseUrl}/trending/{ContentMode}/day?api_key={apiKey}";
        }

        public string BuildPopularContentUrl()
        {
            return $"{BaseUrl}/{ContentMode}/popular?api_key={apiKey}";
        }

        public string BuildContentByGenreUrl(int genreId, int page)
        {
            var today = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            return $"{BaseUrl}/discover/{ContentMode}?api_key={apiKey}&sort_by=release_date.desc&include_adult=false&with_genres={genreId}&release_date.lte={today}&page={page}&vote_count.gte=15";
        }

        public string BuildCoverImageUrl(string imageName)
        {

            return string.IsNullOrEmpty(imageName) ? null : $"{BaseImageUrl}/w300_and_h450_bestv2/{imageName.Replace("/","")}";
        }

        public string BuildBackgroundImageUrl(string imageName)
        {
            return string.IsNullOrEmpty(imageName) ? null : $"{BaseImageUrl}/w1920_and_h1080_bestv2/{imageName.Replace("/", "")}";
        }

        public string BuildLogoImageUrl(string imageName)
        {
            return string.IsNullOrEmpty(imageName) ? null : $"{BaseImageUrl}/w500/{imageName.Replace("/", "")}";
        }

        public string BuildContentDetailsUrl(string tmdbMovieId, string language = null)
        {
            var languageParam = string.IsNullOrEmpty(language) ? "en-US" : language;

            return $"{BaseUrl}/{ContentMode}/{tmdbMovieId}?api_key={apiKey}&language={languageParam}&append_to_response=videos";
        }

        public string BuildContentImagesUrl(string tmdbMovieId)
        {
            return $"{BaseUrl}/{ContentMode}/{tmdbMovieId}/images?api_key={apiKey}";
        }

        public string BuilGenresListUrl()
        {
            return $"{BaseUrl}/genre/{ContentMode}/list?api_key={apiKey}";
        }

        public string BuildPopularContentByGenreUrl(int genreId)
        {
            var date = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");

            return $"{BaseUrl}/discover/{ContentMode}?api_key={apiKey}&sort_by=popularity.desc&include_adult=false&include_video=false&page=1&release_date.lte={date}&vote_average.gte=5&with_genres={genreId}&vote_count.gte=15";
        }

        public string BuildGetCreditsUrl(string tmdbContentId)
        {
            return $"{BaseUrl}/{ContentMode}/{tmdbContentId}/credits?api_key={apiKey}";
        }

        public string BuildGetImagesUrl(string tmdbContentId)
        {
            return $"{BaseUrl}/{ContentMode}/{tmdbContentId}/images?api_key={apiKey}";
        }

        public string BuildPopularContentByPlatformUrl(int platformId)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");

            return $"{BaseUrl}/discover/{ContentMode}?api_key={apiKey}&sort_by=popularity.desc&include_adult=false&include_video=false&page=1&release_date.lte={date}&with_watch_providers={platformId}&watch_region=US";
        }

        public string BuildEpisodesBySeasonUrl(string tmdbContentId, int seasonNumber)
        {
            return $"{BaseUrl}/tv/{tmdbContentId}/season/{seasonNumber}?api_key={apiKey}";
        }
    }
}
