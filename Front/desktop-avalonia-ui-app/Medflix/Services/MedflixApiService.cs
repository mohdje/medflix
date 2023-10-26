using Medflix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace Medflix.Services
{
    public class MedflixApiService
    {
        static HttpClient HttpClient;
        const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.115 Safari/537.36";
        const string HostUrl = "http://localhost:5000";
        public MedflixApiService()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            HttpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<Subtitle[]> GetSubtitlesAsync(string subtitlesSourceUrl)
        {
            var url = $"{HostUrl}/subtitles?sourceUrl={subtitlesSourceUrl}";
            return await GetAsync<Subtitle[]>(url);
        }

        public async Task<DownloadState> GetDownloadStateAsync(string base64Url)
        {
            var url = $"{HostUrl}/torrent/streamdownloadstate?base64TorrentUrl={base64Url}";
            return await GetAsync<DownloadState>(url);
        }

        public async Task SaveProgressionAsync(string mediaType, WatchedMediaDto watchedMedia)
        {
            var url = $"{HostUrl}/{mediaType}/watchedmedia";

            await HttpClient.PutAsJsonAsync(url, watchedMedia);
        }

        private async Task<T> GetAsync<T>(string uri)
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<T>(uri);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
