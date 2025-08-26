using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace MoviesAPI.Helpers
{
    internal static class HttpRequester
    {
        static HttpClient _httpClient;
        static HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.115 Safari/537.36");
                    _httpClient.Timeout = TimeSpan.FromSeconds(7);
                }
                return _httpClient;
            }
        }

        public static async Task<string> GetAsync(Uri url)
        {
            return await PerformGetStringCallAsync(url);
        }

        public static async Task<T> GetAsync<T>(string url, NameValueCollection parameters = null, JsonNamingPolicy jsonNamingPolicy = null) where T : class
        {
            var uri = parameters == null ? new Uri(url) : BuildUri(url, parameters);
            var json = await PerformGetStringCallAsync(uri);

            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = jsonNamingPolicy ?? JsonNamingPolicy.SnakeCaseLower,
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task<T> PostAsync<T>(string url, Dictionary<string, object> body) where T : class
        {
            var uri = new Uri(url);

            var content = JsonContent.Create(body);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await HttpClient.PostAsync(uri, content);

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<byte[]> DownloadAsync(Uri url, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders)
        {
            if(httpRequestHeaders != null)
            {
                foreach (var header in httpRequestHeaders)
                    HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await PerformGetCallAsync(url);

            var bytes = new byte[0];

            if (response != null)
                bytes = await response.ReadAsByteArrayAsync();

            if (httpRequestHeaders != null)
            {
                foreach (var header in httpRequestHeaders)
                    HttpClient.DefaultRequestHeaders.Remove(header.Key);
            }

            return bytes;
        }

        public static async Task<HtmlAgilityPack.HtmlDocument> GetHtmlDocumentAsync(string url)
        {
           
            var htmlResult = await GetAsync(new Uri(url));

            if (string.IsNullOrEmpty(htmlResult))
                return null;

            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(htmlResult);

            return htmlDocument;
        }

        private static Uri BuildUri(string url, NameValueCollection parameters)
        {
            var uriBuilder = new UriBuilder(url);

            if (parameters != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Add(parameters);
                uriBuilder.Query = query.ToString();
            }

            return new Uri(uriBuilder.ToString());
        }


        private static async Task<string> PerformGetStringCallAsync(Uri url)
        {
            var response = await PerformGetCallAsync(url);

            if(response == null)
                return null;

            return await response.ReadAsStringAsync();
        }

        private static async Task<HttpContent> PerformGetCallAsync(Uri url)
        {
            try
            {
                var response = await HttpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK)
                    return response.Content;
                else if (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Moved)
                    return await PerformGetCallAsync(response.Headers.Location);
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
