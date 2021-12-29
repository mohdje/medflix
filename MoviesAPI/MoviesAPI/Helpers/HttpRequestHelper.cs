
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoviesAPI.Helpers
{
    internal static class HttpRequestHelper
    {
        static HttpClient client;
        private static void InitHttpClient()
        {
            if (client == null)
            {
                var userAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";             
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }
        }
        #region Get
        public static async Task<string> GetAsync(Uri url)
        {
            return await PerformGetCallAsync(url);
        }

        public static async Task<T> GetAsync<T>(Uri url) where T : class
        {
            var result = await PerformGetCallAsync(url);

            return JsonHelper.ToObject<T>(result);
        }

        public static Task<string> GetAsync(string baseUrl, NameValueCollection parameters)
        {
            return PerformGetCallAsync(BuildUri(baseUrl, parameters));
        }

        public static async Task<T> GetAsync<T>(string baseUrl, NameValueCollection parameters) where T : class
        {
            var result = await PerformGetCallAsync(BuildUri(baseUrl, parameters));

            return JsonHelper.ToObject<T>(result);
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


        private static async Task<string> PerformGetCallAsync(Uri url)
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
        #endregion

    }
}
