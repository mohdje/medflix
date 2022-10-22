using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Helpers;

namespace MoviesAPI.Services.VOMovies.YtsHtml
{

    public class YtsHtmlService : VOMovieSearcher
    {
        IYtsHtmlUrlProvider htmlUrlProvider;

        internal YtsHtmlService(IYtsHtmlUrlProvider ytsHtmlUrlProvider)
        {
            htmlUrlProvider = ytsHtmlUrlProvider;
        }

        public override async Task<IEnumerable<MovieDto>> GetSuggestedMoviesAsync(int nbMovies)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetSuggestedMoviesUrl());

            var movieDtos = await GetYtsHtmlMovieLiteDtosAsync(doc.DocumentNode, true, true);

            return movieDtos.Where(m => !string.IsNullOrEmpty(m.Synopsis)).Take(nbMovies);
        }

        public override async Task<IEnumerable<MovieDto>> GetLastMoviesByGenreAsync(int nbMovies, string genre)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetLastMoviesByGenreUrl(genre));

            var movieDtos = await GetYtsHtmlMovieLiteDtosAsync(doc.DocumentNode);

            return movieDtos.Take(nbMovies);
        }

        public override async Task<IEnumerable<MovieDto>> GetMoviesByGenreAsync(string genre, int page)
        {
            var pageIndex = page <= 0 ? 1 : page;

            var doc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetMovieSearchByGenreUrl(genre, pageIndex));

            var movieDtos = await GetYtsHtmlMovieLiteDtosAsync(doc.DocumentNode);

            return movieDtos;
        }

        public override async Task<IEnumerable<MovieDto>> GetMoviesByNameAsync(string name)
        {
            var pageIndex = 0;
            bool keepSearch = true;

            var moviesSearchResult = new List<MovieDto>();

            var tokenizedName = name.Split(' ');

            while (keepSearch)
            {
                pageIndex++;

                var doc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetMovieSearchByNameUrl(name, pageIndex));

                var movieDtos = await GetYtsHtmlMovieLiteDtosAsync(doc.DocumentNode);

                if (movieDtos.Any())
                    moviesSearchResult.AddRange(movieDtos.Where(m => m.Title.ToLower().ContainsWords(tokenizedName)));
                else
                    keepSearch = false;
            }

            return moviesSearchResult;
        }

        public override async Task<MovieDto> GetMovieDetailsAsync(string movieId)
        {
            var doc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetMovieDetailsUrl(movieId));

            var movieTorrents = doc.DocumentNode.SelectSingleNode("//p/em[contains(text(), 'Available in')]").ParentNode;
            var movieTorrentsHtml = new HtmlAgilityPack.HtmlDocument();
            movieTorrentsHtml.LoadHtml(movieTorrents.InnerHtml);

            return new MovieDto()
            {
                Id = movieId,
                Title = doc.DocumentNode.SelectSingleNode("//div[@id='movie-info']//h1")?.InnerText,
                Year = doc.DocumentNode.SelectSingleNode("//div[@id='movie-info']//h2")?.InnerText,
                Duration = doc.DocumentNode.SelectSingleNode("//span[@title='Runtime']")?.ParentNode?.InnerText.Trim(),
                Genres = doc.DocumentNode.SelectNodes("//div[@id='movie-info']//h2")?.Last().InnerText.Replace("&nbsp;", string.Empty).Replace("/", ", "),
                ImdbCode = doc.DocumentNode.SelectSingleNode("//div[@id='movie-info']//a[@title='IMDb Rating']")?.Attributes["href"].Value.Split('/').SingleOrDefault(t => t.StartsWith("tt")),
                BackgroundImageUrl = htmlUrlProvider.GetImageUrl(doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'screenshot-group')]")?.Attributes["href"].Value),
                CoverImageUrl = htmlUrlProvider.GetImageUrl(doc.DocumentNode.SelectSingleNode("//div[@id='movie-poster']//img")?.Attributes["src"].Value),
                Rating = doc.DocumentNode.SelectSingleNode("//div[@class='rating-row']/a[@title='IMDb Rating']")?.ParentNode?.InnerText.Trim().Split('\n').First(),
                Synopsis = doc.DocumentNode.SelectSingleNode("//div[@id='synopsis']//p")?.InnerText,
                Director = doc.DocumentNode.SelectSingleNode("//div[@class='directors']//a[@class='name-cast']")?.InnerText,
                Cast = doc.DocumentNode.SelectNodes("//div[@class='actors']//a[@class='name-cast']")?.Select(n => n.InnerText).Aggregate((a, b) => a + ", " + b),
                YoutubeTrailerUrl = doc.DocumentNode.SelectSingleNode("//*[@id='playTrailer']")?.Attributes["href"].Value,
                Torrents = movieTorrentsHtml.DocumentNode.SelectNodes("//a[contains(@title, 'torrent') or contains(@title, 'Torrent')]")
                                                        .Select(n => new MovieTorrent()
                                                        {
                                                            DownloadUrl = htmlUrlProvider.GetTorrentUrl(n.Attributes["href"].Value),
                                                            Quality = n.InnerText
                                                        }).ToArray()
            };
        }

        private async Task<IEnumerable<MovieDto>> GetYtsHtmlMovieLiteDtosAsync(HtmlNode documentNode, bool withSynopsis = false, bool withBackgroundImage = false)
        {
            var movies = documentNode.SelectNodes("//div[contains(@class, 'browse-movie-wrap')]");

            if (movies == null)
                return new MovieDto[0];

            var movieDtos = new List<MovieDto>();
            var tasks = new List<Task<MovieDto>>();
            foreach (var movie in movies)
            {
                tasks.Add(Task.Run(async () =>
                {
                    return await GetYtsHtmlMovieLiteDtoAsync(movie.InnerHtml, withSynopsis, withBackgroundImage);
                }));
            }

            await Task.WhenAll(tasks).ContinueWith(t => movieDtos.AddRange(t.Result));

            return movieDtos;
        }

        private async Task<MovieDto> GetYtsHtmlMovieLiteDtoAsync(string movieHtml, bool withSynopsis = false, bool withBackgroundImage = false)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(movieHtml);

            var movieId = htmlUrlProvider.GetMovieId(doc.DocumentNode.SelectSingleNode("/*[contains(@class, 'browse-movie-link')]")?.Attributes["href"].Value);

            string synopsis = "";
            string backgroundImage = "";

            if (withSynopsis || withBackgroundImage)
            {
                var detailsDoc = await HttpRequester.GetHtmlDocumentAsync(htmlUrlProvider.GetMovieDetailsUrl(movieId));
                synopsis = withSynopsis ? detailsDoc.DocumentNode.SelectNodes("//div[@id='synopsis']//p")?.First().InnerText : string.Empty;
                backgroundImage = withBackgroundImage ? htmlUrlProvider.GetImageUrl(detailsDoc.DocumentNode.SelectSingleNode("//a[contains(@class, 'screenshot-group')]")?.Attributes["href"].Value) : string.Empty;
            }

            // XPATH : '/a[...]' = search a at fist level of doc, //img[...] search img recursivily in doc
            return new MovieDto()
            {
                CoverImageUrl = htmlUrlProvider.GetImageUrl(doc.DocumentNode.SelectSingleNode("//img[contains(@class, 'img-responsive')]")?.Attributes["src"].Value),
                Id = movieId,
                Synopsis = synopsis,
                BackgroundImageUrl = backgroundImage,
                Title = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'browse-movie-title')]")?.InnerText,
                Year = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'browse-movie-year')]")?.InnerText,
                Rating = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'rating')]")?.InnerText.Replace("/ 10", "").Trim()
            };
        }

        public override IEnumerable<string> GetMovieGenres()
        {
            return new string[] { "Thriller", "Sci-Fi", "Horror", "Romance", "Action", "Comedy", "Drama", "Crime", "Animation", "Adventure", "Fantasy" };

        }

        protected override string GetPingUrl()
        {
            return htmlUrlProvider.GetServiceUrl();
        }
    }
}
