
using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Utils;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using System.Text.Json.Serialization;
using Medflix.Models;
using System.Text.Json;
using System.Diagnostics.Metrics;
using System.Net.Http;


namespace Medflix.Services
{
    public class MedflixApiService
    {
        static HttpClient httpClient;
        const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.5005.115 Safari/537.36";

        private static MedflixApiService _instance;
        public static MedflixApiService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MedflixApiService();

                return _instance;
            }
        }

        string hostServiceUrl = string.Empty;

        string mediaType = Consts.Movies;

        public event EventHandler ContextChanged;


        private MedflixApiService()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }
        }

        public async Task<bool> SetHostServiceAddressAsync(string serviceAddress)
        {
            try
            {
                var url = $"http://{serviceAddress}";

                var response = await httpClient.GetAsync($"{url}/application/ping");

                if (response.IsSuccessStatusCode)
                {
                    hostServiceUrl = url;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void SwitchToMoviesMode()
        {
            if (mediaType != Consts.Movies)
            {
                mediaType = Consts.Movies;
                ContextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SwitchToSeriesMode()
        {
            if (mediaType != Consts.Series)
            {
                mediaType = Consts.Series;
                ContextChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        #region Media fetch operations
        public async Task<IEnumerable<MediaDetails>> GetMediasOfTodaysAsync()
        {
            return await GetMediaListAsync($"{hostServiceUrl}/{mediaType}/mediasoftoday");
        }

        public async Task<IEnumerable<MediaDetails>> GetPopularMediasAsync()
        {
            return await GetMediaListAsync($"{hostServiceUrl}/{mediaType}/popular");
        }

        public async Task<IEnumerable<MediaDetails>> GetPopularNetflixAsync()
        {
            return await GetMediaListAsync($"{hostServiceUrl}/{mediaType}/netflix");
        }

        public async Task<IEnumerable<MediaDetails>> GetPopularAmazonPrimeAsync()
        {
            return await GetMediaListAsync($"{hostServiceUrl}/{mediaType}/amazonprime");
        }

        public async Task<IEnumerable<MediaDetails>> GetPopularDisneyPlusAsync()
        {
            return await GetMediaListAsync($"{hostServiceUrl}/{mediaType}/disneyplus");
        }

        public async Task<IEnumerable<MediaDetails>> GetPopularAppleTvAsync()
        {
            return await GetMediaListAsync($"{hostServiceUrl}/{mediaType}/appletv");
        }

        public async Task<IEnumerable<MediaDetails>> GetRecommandationsAsync()
        {
            return await GetMediaListAsync($"{hostServiceUrl}/{mediaType}/recommandations");
        }

        private async Task<IEnumerable<MediaDetails>> GetMediaListAsync(string url)
        {
            var list = await GetAsync<IEnumerable<MediaDetails>>(url);
            return list?.Take(10);
        }
        public async Task<IEnumerable<MediaDetails>> SearchMedia(string text)
        {
            return await GetAsync<IEnumerable<MediaDetails>>($"{hostServiceUrl}/{mediaType}/search?t={text}");
        }

        public async Task<IEnumerable<MediaDetails>> GetSimilarMediasAsync(string mediaId)
        {
            return await GetAsync<IEnumerable<MediaDetails>>($"{hostServiceUrl}/{mediaType}/similar/{mediaId}");
        }

        public async Task<MediaDetails> GetMediaDetailsAsync(string mediaId)
        {
            return await GetAsync<MediaDetails>($"{hostServiceUrl}/{mediaType}/details/{mediaId}");
        }

        public async Task<IEnumerable<Episode>> GetEpisodesAsync(string mediaId, int seasonNumber)
        {
            return await GetAsync<IEnumerable<Episode>>($"{hostServiceUrl}/{mediaType}/episodes/{mediaId}/{seasonNumber}");
        }

        public async Task<IEnumerable<string>> GetAvailableFrenchSubtitlesAsync(string imdbId, int? seasonNumber = null, int? episodeNumber = null)
        {
            var queyString = BuildQueryString(imdbId: imdbId, seasonNumber: seasonNumber, episodeNumber: episodeNumber);

            return await GetAsync<IEnumerable<string>>($"{hostServiceUrl}/subtitles/{mediaType}/fr?{queyString}");
        }

        public async Task<IEnumerable<string>> GetAvailableEnglishSubtitlesAsync(string imdbId, int? seasonNumber = null, int? episodeNumber = null)
        {
            var queyString = BuildQueryString(imdbId: imdbId, seasonNumber: seasonNumber, episodeNumber: episodeNumber);

            return await GetAsync<IEnumerable<string>>($"{hostServiceUrl}/subtitles/{mediaType}/en?{queyString}");
        }

        public async Task<IEnumerable<MediaSource>> GetAvailableVOSources(string title, int? year = null, string? imdbId = null, int? seasonNumber = null, int? episodeNumber = null)
        {
            var queyString = BuildQueryString(title: title, year: year, imdbId: imdbId, seasonNumber: seasonNumber, episodeNumber: episodeNumber);

            return await GetAsync<IEnumerable<MediaSource>>($"{hostServiceUrl}/mediasource/{mediaType}/vo?{queyString}");
        }

        public async Task<IEnumerable<MediaSource>> GetAvailableVFSources(string title, int? year = null, string? mediaId = null, int? seasonNumber = null, int? episodeNumber = null)
        {
            var queyString = BuildQueryString(title: title, year: year, mediaId: mediaId, seasonNumber: seasonNumber, episodeNumber: episodeNumber);

            return await GetAsync<IEnumerable<MediaSource>>($"{hostServiceUrl}/mediasource/{mediaType}/vf?{queyString}");
        }

        public async Task<IEnumerable<WatchMediaInfo>> GetWatchMediaHistory()
        {
            return await GetAsync<IEnumerable<WatchMediaInfo>>($"{hostServiceUrl}/{mediaType}/watchedmedia");
        }

        public async Task<WatchMediaInfo> GetWatchMediaInfo(string mediaId)
        {
            var watchMediaHistory = await GetWatchMediaHistory();

            var lastEpisodeWatched = watchMediaHistory.FirstOrDefault(w => w.Media.Id == mediaId);

            return lastEpisodeWatched;
        }

        public async Task<WatchMediaInfo> GetEpisodeWatchMediaInfo(string mediaId, int seasonNumber, int episodeNumber)
        {
            var queryString = BuildQueryString(seasonNumber: seasonNumber, episodeNumber: episodeNumber);

            return await GetAsync<WatchMediaInfo>($"{hostServiceUrl}/{mediaType}/watchedmedia/{mediaId}?{queryString}");
        }

        public async Task<IEnumerable<MediaDetails>> GetBookmarkedMedias()
        {
            return await GetAsync<IEnumerable<MediaDetails>>($"{hostServiceUrl}/{mediaType}/bookmarks");
        }

        public async Task<bool> BookmarkMedia(MediaDetails media)
        {
            var result = await httpClient.PutAsJsonAsync($"{hostServiceUrl}/{mediaType}/bookmarks", media);

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveBookmarkMedia(string mediaId)
        {
            var result = await httpClient.DeleteAsync($"{hostServiceUrl}/{mediaType}/bookmarks?id={mediaId}");

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> IsMediaBookmarked(string mediaId)
        {
            var response = await httpClient.GetAsync($"{hostServiceUrl}/{mediaType}/bookmarks/exists?id={mediaId}");

            if (!response.IsSuccessStatusCode)
                return false;

            var data = await response.Content.ReadAsStringAsync();

            return Boolean.Parse(data);
        }
        #endregion

        #region Video player operations

        public async Task<DownloadState> GetDownloadStateAsync(string streamUrl)
        {
            var uri = new Uri(streamUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            var base64TorrentUrl = query["base64TorrentUrl"];
            return await GetAsync<DownloadState>($"{hostServiceUrl}/torrent/streamdownloadstate?base64TorrentUrl={base64TorrentUrl}");
        }

        public async Task SaveProgressionAsync(WatchMediaInfo videoPlayerMedia)
        {
            await PutAsync($"{hostServiceUrl}/{mediaType}/watchedmedia", videoPlayerMedia);
        }

        public async Task<IEnumerable<Subtitles>> GetSubtitlesAsync(string url)
        {
            return await GetAsync<IEnumerable<Subtitles>>($"{hostServiceUrl}/subtitles?sourceUrl={url}");
        }

        public string BuildStreamUrl(string mediaUrl, int? seasonNumber, int? episodeNumber)
        {
            // mediaUrl = TestData.TorrentUrl;
            // return TestData.Mp4Url;
            if (mediaUrl.StartsWith("http") || mediaUrl.StartsWith("magnet"))
            {
                var queryString = $"base64TorrentUrl={ToBase64(mediaUrl)}&";
                queryString += BuildQueryString(seasonNumber: seasonNumber, episodeNumber: episodeNumber);

                return $"{hostServiceUrl}/torrent/stream/{mediaType}?{queryString}";
            }
            else
            {
                var queryString = $"base64VideoPath={ToBase64(mediaUrl)}";
                return $"{hostServiceUrl}/application/video?{queryString}";
            }
        }

        #endregion

        private string BuildQueryString(string? title = null, int? year = null, string? imdbId = null, string? mediaId = null, int? seasonNumber = null, int? episodeNumber = null)
        {
            var queryString = new List<string>();

            if (!string.IsNullOrEmpty(title))
                queryString.Add($"title={Uri.EscapeDataString(title)}");

            if (year.HasValue)
                queryString.Add($"year={year}");

            if (!string.IsNullOrEmpty(imdbId))
                queryString.Add($"imdbId={imdbId}");

            if (!string.IsNullOrEmpty(mediaId))
                queryString.Add($"mediaId={mediaId}");

            if (seasonNumber.HasValue)
                queryString.Add($"seasonNumber={seasonNumber}");

            if (episodeNumber.HasValue)
                queryString.Add($"episodeNumber={episodeNumber}");

            return String.Join("&", queryString);
        }
        private string ToBase64(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private async Task<T> GetAsync<T>(string url)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<T>(url);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        private async Task PutAsync<T>(string uri, T dataObject)
        {
            try
            {
                await httpClient.PutAsJsonAsync(uri, dataObject);
            }
            catch (Exception ex)
            {
            }
        }
    }
}