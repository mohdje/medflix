using Medflix.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Services
{
    public abstract class HttpClientService
    {
        static HttpClient httpClient;
        const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.115 Safari/537.36";

        public HttpClientService()
        {
            if(httpClient == null)
                httpClient = new HttpClient();
        }

        private void SetHeaders(MedflixHttpHeaders headers)
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Authorization = null;

            if (headers != null)
            {
                foreach (var header in headers.DefaultHeaders)
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

                if (headers.Authorization != null)
                    httpClient.DefaultRequestHeaders.Authorization = headers.Authorization;

                if (headers.Accept != null)
                    httpClient.DefaultRequestHeaders.Accept.Add(headers.Accept);
            }
            else
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
        }
        protected async Task<T> GetAsync<T>(string uri, MedflixHttpHeaders headers = null)
        {
            SetHeaders(headers);

            try
            {
                return await httpClient.GetFromJsonAsync<T>(uri);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        protected async Task<bool> DownloadAsync(string uri, string filePath, MedflixHttpHeaders headers = null)
        {
            SetHeaders(headers);

            try
            {
                var bytes = await httpClient.GetByteArrayAsync(uri);
                if (bytes != null && bytes.Any())
                {
                    File.WriteAllBytes(filePath, bytes);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
        protected async Task PutAsync<T>(string uri, T dataObject, MedflixHttpHeaders headers = null)
        {
            SetHeaders(headers);

            try
            {
                await httpClient.PutAsJsonAsync(uri, dataObject);
            }
            catch (Exception)
            {
            }
        }
    }
}
