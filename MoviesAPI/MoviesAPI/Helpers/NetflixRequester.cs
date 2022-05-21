using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    internal static class NetflixRequester
    {
        private const string baseUrl = "https://top10.netflix.com";
        public static async Task<string[]> GetTopNetflixMoviesNameAsync()
        {
            var buildId = GetBuildId();

            if (string.IsNullOrEmpty(buildId))
                return null;

            var topEnglishMovies = await GetMoviesNamesAsync(GetTopMoviesUrl(buildId, true));
            var topNonEnglishMovies = await GetMoviesNamesAsync(GetTopMoviesUrl(buildId, false));

            var result = new List<string>();

            if(topEnglishMovies != null)
                result.AddRange(topEnglishMovies);

            if(topNonEnglishMovies != null)
                result.AddRange(topNonEnglishMovies);

            return result.ToArray();
        }

        private static string GetBuildId()
        {
            var doc = HttpRequester.GetHtmlDocumentAsync(baseUrl + "/films").Result;

            var startIndex = doc.DocumentNode.InnerHtml.IndexOf("buildId");
            var lastIndex = doc.DocumentNode.InnerHtml.IndexOf(",", startIndex);

            return (startIndex > 0 && lastIndex > 0) ?
                doc.DocumentNode.InnerHtml.Substring(startIndex, lastIndex - startIndex).Replace("\"", "").Replace(":", "").Replace("buildId", "")
                : null;
        }

        private static string GetTopMoviesUrl(string buildId, bool english)
        {
            var origin = english ? "films" : "films-non-english";
            return $"https://top10.netflix.com/_next/data/{buildId}/{origin}.json";
        }

        private static async Task<string[]> GetMoviesNamesAsync(string url)
        {
            var topMovies = await HttpRequester.GetAsync<NetflixMovieDto>(new Uri(url));

            return topMovies.PageProps.Data.WeeklyTopTen.Select(m => m.Name).ToArray();
        }
    }

    internal class NetflixMovieDto
    {
        public PageProps PageProps { get; set; }
    }

    internal class PageProps
    {
        public Data Data { get; set; }
        public PathParams PathParams { get; set; }
    }

    internal class PathParams
    {
        public string Week { get; set; }
    }

    internal class Data
    {
        public WeeklyTopTen[] WeeklyTopTen { get; set; }
    }

    internal class WeeklyTopTen
    {
        public string Name { get; set; }
    }
}
