using Medflix.Models;
using MedflixWinForms.Models;
using MedflixWinForms.Utils;
using System.Web;
using WebHostStreaming.Extensions;
using WebHostStreaming.Models;

namespace MedflixWinForms.Services
{
    public class MedflixApiService : HttpClientService
    {
        public async Task<string> GetSubtitlesFileAsync(string subtitlesSourceUrl)
        {
            var url = $"{Consts.WebHostUrl}/subtitles/file?sourceUrl={HttpUtility.HtmlEncode(subtitlesSourceUrl)}";
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
            var url = $"{Consts.WebHostUrl}/torrent/streamdownloadstate?base64TorrentUrl={base64Url}";
            return await GetAsync<DownloadState>(url);
        }

        public async Task SaveProgressionAsync(string mediaType, WatchedMediaDto watchedMedia)
        {
            var url = $"{Consts.WebHostUrl}/{mediaType}/watchedmedia";

            await PutAsync(url, watchedMedia);
        }
    }
}
