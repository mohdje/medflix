using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Services.YtsApi.DTOs;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Helpers;

namespace MoviesAPI.Services.YtsApi
{
    public class YtsApiService : IVOMovieSearcher
    {
        IYtsApiUrlProvider ytsApiUrlProvider;

        private string listMovies = "list_movies.json";
        private string movieDetails = "movie_details.json";

        public YtsApiService(IYtsApiUrlProvider ytsApiUrlProvider)
        {
            this.ytsApiUrlProvider = ytsApiUrlProvider;
        }

        public async Task<IEnumerable<MovieDto>> GetSuggestedMoviesAsync(int nbMovies)
        {
            var parameters = new NameValueCollection();
            parameters.Add("limit", (50).ToString());
            parameters.Add("page", "1");
            parameters.Add("minimum_rating", "8");
            parameters.Add("sort_by", "year");
            parameters.Add("order_by", "desc");

            var requestResult = await HttpRequestHelper.GetAsync<YtsResultDto>(ytsApiUrlProvider.GetBaseApiUrl() + listMovies, parameters);

            return requestResult.Data.Movies.Where(m => !string.IsNullOrEmpty(m.Summary))
                                            .OrderBy(m => m.DateUploaded)
                                            .Reverse()
                                            .Take(nbMovies)
                                            .Select(m => ToMovieDto(m));
        }

        public async Task<IEnumerable<MovieDto>> GetLastMoviesByGenreAsync(int nbMovies, string genre)
        {
            var parameters = new NameValueCollection();
            parameters.Add("limit", (50).ToString());
            parameters.Add("page", "1");
            parameters.Add("minimum_rating", "7");
            parameters.Add("sort_by", "year");
            parameters.Add("order_by", "desc");
            parameters.Add("genre", genre);

            var requestResult = await HttpRequestHelper.GetAsync<YtsResultDto>(ytsApiUrlProvider.GetBaseApiUrl() + listMovies, parameters);

            return requestResult.Data.Movies.OrderBy(m => m.DateUploaded).Take(nbMovies).Select(m => ToMovieDto(m));
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(string genre, int page)
        {
            var parameters = new NameValueCollection();
            parameters.Add("limit", (20).ToString());
            parameters.Add("page", page.ToString());
            parameters.Add("sort_by", "year");
            parameters.Add("order_by", "desc");
            parameters.Add("genre", genre);

            var requestResult = await HttpRequestHelper.GetAsync<YtsResultDto>(ytsApiUrlProvider.GetBaseApiUrl() + listMovies, parameters);

            return requestResult.Data.Movies.Select(m => ToMovieDto(m));
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesByNameAsync(string name)
        {
            var parameters = new NameValueCollection();
            parameters.Add("query_term", name);
            parameters.Add("sort_by", "year");
            parameters.Add("order_by", "desc");

            var requestResult = await HttpRequestHelper.GetAsync<YtsResultDto>(ytsApiUrlProvider.GetBaseApiUrl() + listMovies, parameters);

            return requestResult.Data.Movies?.Select(m => ToMovieDto(m));
        }

        public async Task<MovieDto> GetMovieDetailsAsync(string movieId)
        {
            var parameters = new NameValueCollection();
            parameters.Add("movie_id", movieId);
            parameters.Add("with_cast", "true");
            parameters.Add("sort_by", "year");
            parameters.Add("order_by", "desc");

            var requestResult = await HttpRequestHelper.GetAsync<YtsResultDto>(ytsApiUrlProvider.GetBaseApiUrl() + movieDetails, parameters);
            var movieDto = ToMovieDto(requestResult?.Data?.Movie);

            if (movieDto != null)
                movieDto.Director = await GetDirector(movieDto.ImdbCode);

            return movieDto;
        }

        private async Task<string> GetDirector(string imdbCode)
        {
            var html = await HttpRequestHelper.GetAsync("https://www.imdb.com/title/" + imdbCode, null);

            if (html == null)
                return string.Empty;

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var directorSection = doc.DocumentNode.SelectSingleNode("//h4[contains(text(),'Director')]");

            if (directorSection == null)
                return string.Empty;

            doc.LoadHtml(directorSection.ParentNode.InnerHtml);

            var directorName = doc.DocumentNode.SelectSingleNode("/a")?.InnerText;

            return string.IsNullOrEmpty(directorName) ? string.Empty : directorName;
        }

        private MovieDto ToMovieDto(YtsMovieDto ytsMovieDto)
        {
            if (ytsMovieDto == null)
                return null;

            return new MovieDto()
            {
                Cast = ytsMovieDto.Cast != null ? string.Join(", ", ytsMovieDto.Cast?.Select(c => c.Name)) : string.Empty,
                BackgroundImageUrl = ytsMovieDto.BackgroundImageUrl,
                ImdbCode = ytsMovieDto.ImdbCode,
                CoverImageUrl = ytsMovieDto.CoverImageUrl,
                Genres = ytsMovieDto.Genres == null ? string.Empty : string.Join(", ", ytsMovieDto.Genres),
                Id = ytsMovieDto.Id.ToString(),
                Duration = ytsMovieDto.Runtime > 0 ? $"{TimeSpan.FromMinutes(ytsMovieDto.Runtime).Hours} h {TimeSpan.FromMinutes(ytsMovieDto.Runtime).Minutes} min" : string.Empty,
                Rating = ytsMovieDto.Rating.ToString(),
                Synopsis = ytsMovieDto.Summary,
                Title = ytsMovieDto.Title,
                Torrents = ytsMovieDto.Torrents.Select(t => new MovieTorrent()
                {
                    Quality = t.Quality,
                    DownloadUrl = t.Url
                }).ToArray(),
                Year = ytsMovieDto.Year.ToString(),
                YoutubeTrailerUrl = string.IsNullOrEmpty(ytsMovieDto.YoutubeTrailerUrl) ? string.Empty : "https://www.youtube.com/embed/" + ytsMovieDto.YoutubeTrailerUrl
            };
        }

        public IEnumerable<string> GetMovieGenres()
        {
            return new string[] { "Thriller", "Sci-Fi", "Horror", "Romance", "Action", "Comedy", "Drama", "Crime", "Animation", "Adventure", "Fantasy" };
        }
    }
}
