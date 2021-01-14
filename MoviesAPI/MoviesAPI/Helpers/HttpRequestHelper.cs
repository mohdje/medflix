using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoviesAPI.Helpers
{
    internal static class HttpRequestHelper
    {
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

            if(parameters != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Add(parameters);
                uriBuilder.Query = query.ToString();
            }

            return new Uri(uriBuilder.ToString());
        }

       
        private static async Task<string> PerformGetCallAsync(Uri url)
        {
            string result = null;
            try
            {
                using (var client = new HttpClient())
                {
                    var userAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.88 Safari/537.36";
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    result = await client.GetStringAsync(url);
                }
                    
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        #endregion

        #region Post


        public static string Post(Uri url, byte[] input)
        {
            var result = PerformPostCall(url, input);
            return System.Text.Encoding.UTF8.GetString(result);
        }

        public static T Post<T>(Uri url, byte[] input) where T : class
        {
            var result = PerformPostCall(url, input);
            var text = System.Text.Encoding.UTF8.GetString(result);

            return JsonHelper.ToObject<T>(text);
        }

        private static byte[] PerformPostCall(Uri url, byte[] input)
        {
            if (url == null || input == null)
                return null;

            byte[] data = null;

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                try
                {
                    data = webClient.UploadData(url, input);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return data;
        }
        #endregion

    }
}
