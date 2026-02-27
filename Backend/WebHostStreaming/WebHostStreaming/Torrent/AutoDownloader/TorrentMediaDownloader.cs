using System.Threading.Tasks;
using WebHostStreaming.Providers;
using System.Linq;
using System;
using WebHostStreaming.Helpers;
using System.Collections.Generic;
using System.Threading;
using WebHostStreaming.Models;
using System.Collections.Concurrent;
using MoviesAPI.Services.Torrent.Dtos;

namespace WebHostStreaming.Torrent
{
    public abstract class TorrentMediaDownloader<T>(
        ITorrentContentProvider torrentContentProvider,
        IVideoInfoProvider videoInfoProvider,
        ISearchersProvider searchersProvider)
    {
        protected readonly ITorrentContentProvider torrentContentProvider = torrentContentProvider;
        protected readonly IVideoInfoProvider videoInfoProvider = videoInfoProvider;
        protected readonly ISearchersProvider searchersProvider = searchersProvider;

        readonly Dictionary<int, string> qualitiesRank = new()
            {
                { 0, "1080p" },
                { 1, "720p" },
                { 2, "BDRIP" },
                { 3, "WEBRIP" },
                { 4, "DVDRIP" }
            };

        protected readonly ConcurrentDictionary<string, T> voDownloadList = [];
        protected readonly ConcurrentDictionary<string, T> vfDownloadList = [];

        public bool HasMediasToDownload => !voDownloadList.IsEmpty || !vfDownloadList.IsEmpty;

        protected abstract Task<IEnumerable<T>> GetMediasToDownloadAsync();
        protected abstract bool VideoExists(T media, LanguageVersion languageVersion);
        protected abstract string GetMediaId(T media);
        protected abstract string GetMediaInfo(T media);

        protected abstract string MediaType { get; }

        protected abstract Task<IEnumerable<MediaTorrent>> SearchVoTorrentsAsync(T media);
        protected abstract Task<IEnumerable<MediaTorrent>> SearchVfTorrentsAsync(T media);
        protected abstract IEnumerable<TorrentRequest> BuildTorrentRequests(string clientId, T media, IEnumerable<MediaTorrent> torrents, LanguageVersion languageVersion);

        public event EventHandler MediaAddedToDownloadList;

        public async Task<bool> DownloadAsync(string clientId, CancellationToken cancellationToken)
        {
            await BuildDownloadListsAsync();

            if (voDownloadList.IsEmpty && vfDownloadList.IsEmpty)
            {
                AppLogger.LogInfo($"TorrentAutoDownloader.StartDownloadAsync(): No {MediaType} to download");
                return false;
            }

            var downloadSuccess = await DownloadMediasAsync(clientId, cancellationToken);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader.StartDownloadAsync(): All {MediaType} downloads completed successfully");
            else
                AppLogger.LogInfo($"TorrentAutoDownloader.StartDownloadAsync(): Some {MediaType} downloads failed. Check logs for details.");

            return downloadSuccess;
        }

        private async Task BuildDownloadListsAsync()
        {
            var mediasToDownload = await GetMediasToDownloadAsync();
            voDownloadList.Clear();
            vfDownloadList.Clear();

            foreach (var media in mediasToDownload)
                TryAddMediaToDownload(media);
        }

        protected void OnMediaAddedToDownloadList()
        {
            MediaAddedToDownloadList?.Invoke(this, EventArgs.Empty);
        }

        protected void TryAddMediaToDownload(T media)
        {
            var id = GetMediaId(media);
            if (!VideoExists(media, LanguageVersion.Original) && !voDownloadList.ContainsKey(id))
            {
                voDownloadList.TryAdd(id, media);
                AppLogger.LogInfo($"TorrentAutoDownloader: {GetMediaInfo(media)} added to VO download list");
            }

            if (!VideoExists(media, LanguageVersion.French) && !vfDownloadList.ContainsKey(id))
            {
                vfDownloadList.TryAdd(id, media);
                AppLogger.LogInfo($"TorrentAutoDownloader: {GetMediaInfo(media)} added to VF download list");
            }
        }

        private async Task<bool> DownloadMediasAsync(string clientId, CancellationToken cancellationToken)
        {
            try
            {
                (int voFailed, int vfFailed) = (0, 0);

                while (!voDownloadList.IsEmpty || !vfDownloadList.IsEmpty)
                {
                    if (!voDownloadList.IsEmpty)
                    {
                        if (voDownloadList.TryGetValue(voDownloadList.Keys.First(), out var mediaVO))
                        {
                            var voDownloadSuccess = await DownloadOriginalVersionAsync(clientId, mediaVO, cancellationToken);

                            if (!voDownloadSuccess)
                                voFailed++;

                            var id = GetMediaId(mediaVO);
                            voDownloadList.TryRemove(id, out _);
                        }
                    }

                    if (vfDownloadList.TryGetValue(vfDownloadList.Keys.First(), out var mediaVF))
                    {
                        var vfDownloadSuccess = await DownloadFrenchVersionAsync(clientId, mediaVF, cancellationToken);

                        if (!vfDownloadSuccess)
                            vfFailed++;

                        var id = GetMediaId(mediaVF);
                        vfDownloadList.TryRemove(id, out _);
                    }
                }

                var (voMessage, vfMessage) = (voFailed > 0 ? $"{voFailed} VO {MediaType} failed to download" : "", vfFailed > 0 ? $"{vfFailed} VF {MediaType} failed to download" : "");
                var logMessage = string.IsNullOrEmpty(voMessage) && string.IsNullOrEmpty(vfMessage)
                    ? $"No {MediaType} left to download"
                    : $"{voMessage} {vfMessage}";
                AppLogger.LogInfo($"TorrentAutoDownloader.DownloadMediasAsync(): {logMessage}");

                return voFailed == 0 && vfFailed == 0;
            }
            catch (TaskCanceledException)
            {
                AppLogger.LogInfo($"TorrentAutoDownloader.DownloadMediasAsync(): {MediaType} download cancelled");
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"TorrentAutoDownloader.DownloadMediasAsync(): {MediaType}", ex);
            }
            finally
            {
                AppLogger.LogInfo($"TorrentAutoDownloader.DownloadMediasAsync(): {MediaType} download ends");
            }

            return false;
        }

        private async Task<bool> DownloadOriginalVersionAsync(string clientId, T media, CancellationToken cancellationToken)
        {
            var mediaInfo = GetMediaInfo(media);
            AppLogger.LogInfo($"TorrentAutoDownloader: search VO torrents for {mediaInfo}");

            var torrents = await SearchVoTorrentsAsync(media);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VO torrents for {mediaInfo}");

            var torrentRequests = BuildTorrentRequests(clientId, media, torrents, LanguageVersion.Original);

            var downloadSuccess = await DownloadBestQualityAsync(torrentRequests, cancellationToken);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VO successfully downloaded for {mediaInfo}");
            else
                AppLogger.LogInfo($"TorrentAutoDownloader: failed to download VO for {mediaInfo}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadFrenchVersionAsync(string clientId, T media, CancellationToken cancellationToken)
        {
            var mediaInfo = GetMediaInfo(media);

            AppLogger.LogInfo($"TorrentAutoDownloader: search VF torrents for {mediaInfo}");

            var torrents = await SearchVfTorrentsAsync(media);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VF torrents for {mediaInfo}");

            var torrentRequests = BuildTorrentRequests(clientId, media, torrents, LanguageVersion.French);

            var downloadSuccess = await DownloadBestQualityAsync(torrentRequests, cancellationToken);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VF successfully downloaded for {mediaInfo}");
            else
                AppLogger.LogInfo($"TorrentAutoDownloader: failed to download VF for {mediaInfo}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadBestQualityAsync(IEnumerable<TorrentRequest> torrentRequests, CancellationToken cancellationToken)
        {
            if (torrentRequests == null || !torrentRequests.Any())
                return false;

            var sortedRequestsByQuality = torrentRequests.Where(tr => qualitiesRank.Values.Contains(tr.VideoInfo.Quality, StringComparer.OrdinalIgnoreCase))
                                        .OrderBy(request => qualitiesRank.FirstOrDefault(q => string.Equals(q.Value, request.VideoInfo.Quality, StringComparison.OrdinalIgnoreCase)).Key);

            foreach (var torrentRequest in sortedRequestsByQuality)
            {
                AppLogger.LogInfo($"TorrentAutoDownloader: try to download {torrentRequest.TorrentUrl} ({torrentRequest.VideoInfo.Quality})");

                var downloadSuccess = await torrentContentProvider.DownloadTorrentMediaAsync(torrentRequest, cancellationToken);
                if (downloadSuccess)
                    return true;
                else
                    AppLogger.LogInfo($"TorrentAutoDownloader: download failed for {torrentRequest.TorrentUrl}");
            }

            return false;
        }
    }
}
