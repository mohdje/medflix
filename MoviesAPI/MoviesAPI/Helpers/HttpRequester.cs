
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoviesAPI.Helpers
{
    internal static class HttpRequester
    {
        static HttpClient client;
        const string userAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
        private static void InitHttpClient()
        {
            if (client == null)
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }
        }
        #region Get
        public static async Task<string> GetAsync(Uri url)
        {
            return await PerformGetStringCallAsync(url);
        }

        public static async Task<T> GetAsync<T>(Uri url) where T : class
        {
            var result = await PerformGetStringCallAsync(url);

            return JsonHelper.ToObject<T>(result);
        }

        public static Task<string> GetAsync(string baseUrl, NameValueCollection parameters)
        {
            return PerformGetStringCallAsync(BuildUri(baseUrl, parameters));
        }

        public static async Task<T> GetAsync<T>(string baseUrl, NameValueCollection parameters) where T : class
        {
            var result = await PerformGetStringCallAsync(BuildUri(baseUrl, parameters));

            return JsonHelper.ToObject<T>(result);
        }

        public static async Task<byte[]> DownloadAsync(Uri url, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders, bool keepHeaders)
        {
            foreach (var header in httpRequestHeaders)
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

            var result = await DownloadFile(url);

            if (!keepHeaders)
            {
                foreach (var header in httpRequestHeaders)
                    client.DefaultRequestHeaders.Remove(header.Key);
            }

            return result;
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
            try
            {
                InitHttpClient();
                return await client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<byte[]> DownloadFile(Uri url)
        {
            try
            {
                InitHttpClient();
                var response = await client.GetAsync(url);

                var responseStream = response.Content.ReadAsStream();
                var data = new byte[responseStream.Length];

                responseStream.Read(data, 0, data.Length);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
