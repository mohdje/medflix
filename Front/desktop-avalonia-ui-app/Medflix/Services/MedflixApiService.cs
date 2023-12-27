using Medflix.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebHostStreaming.Extensions;
using WebHostStreaming.Models;

namespace Medflix.Services
{
    public class MedflixApiService : HttpClientService
    {
        const string HostUrl = "http://localhost:5000";
        public async Task<string> GetSubtitlesFileAsync(string subtitlesSourceUrl)
        {
           
            var url = $"{HostUrl}/subtitles/file?sourceUrl={HttpUtility.HtmlEncode(subtitlesSourceUrl)}";
            var subtitlesFilePath = await GetAsync(url);
            if(!string.IsNullOrEmpty(subtitlesFilePath))
            {
                var vlcSubtitlesFilePath = Path.Combine(Path.GetDirectoryName(subtitlesFilePath), subtitlesSourceUrl.ToMD5Hash());
                try
                {
                    File.Copy(subtitlesFilePath, vlcSubtitlesFilePath, true);
                    return vlcSubtitlesFilePath;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
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
