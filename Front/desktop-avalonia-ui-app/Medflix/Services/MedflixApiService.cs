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
    public class MedflixApiService : HttpClientService
    {
        const string HostUrl = "http://localhost:5000";
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

            await PutAsync(url, watchedMedia);
        }
    }
}
