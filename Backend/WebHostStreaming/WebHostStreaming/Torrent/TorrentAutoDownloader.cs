using System.Threading.Tasks;
using WebHostStreaming.Providers;
using MoviesAPI.Services.Content.Dtos;
using System.Linq;
using System;
using WebHostStreaming.Helpers;
using System.Collections.Generic;
using System.Threading;
using WebHostStreaming.Models;
using MoviesAPI.Services.Torrent.Dtos;

namespace WebHostStreaming.Torrent
{
    public class TorrentAutoDownloader : ITorrentAutoDownloader
    {
        IVideoInfoProvider videoInfoProvider;
        ISearchersProvider searchersProvider;
        ITorrentContentProvider torrentContentProvider;

        readonly string[] qualitiesRank = { "1080p", "720p", "BDRIP", "WEBRIP", "DVDRIP" };
        const string TorrentAutoDownloaderIdentifier = "Auto-Downloader";
        bool running;
        List<LiteContentDto> moviesToDownload = new List<LiteContentDto>();
        CancellationTokenSource downloadCancellationTokenSource;

        public TorrentAutoDownloader(
            IVideoInfoProvider videoInfoProvider,
            ISearchersProvider searchersProvider,
            ITorrentContentProvider torrentContentProvider)
        {
            this.videoInfoProvider = videoInfoProvider;
            this.searchersProvider = searchersProvider;
            this.torrentContentProvider = torrentContentProvider;
            torrentContentProvider.OnNoActiveTorrentClient += (s, e) => DownloadMoviesAsync();
        }

        public void AddToDownloadList(LiteContentDto mediaToDownload)
        {
            AppLogger.LogInfo($"TorrentAutoDownloader: Add to the list {mediaToDownload.Title} {mediaToDownload.Year}");

            lock (moviesToDownload)
            {
                if (!moviesToDownload.Any(movie => movie.Id == mediaToDownload.Id))
                    moviesToDownload.Add(mediaToDownload);
            }

            DownloadMoviesAsync();
        }

        public void AddToDownloadList(IEnumerable<LiteContentDto> mediasToDownload)
        {
            lock (moviesToDownload)
            {
                foreach (var movieToDownload in mediasToDownload)
                {
                    AppLogger.LogInfo($"TorrentAutoDownloader: Add to the list {movieToDownload.Title} {movieToDownload.Year}");
                    if (!moviesToDownload.Any(movie => movie.Id == movieToDownload.Id))
                        moviesToDownload.Add(movieToDownload);
                }
            }

            DownloadMoviesAsync();
        }

        public void RemoveFromDownloadList(LiteContentDto movieToDownload)
        {
            AppLogger.LogInfo($"TorrentAutoDownloader: Remove from the list {movieToDownload.Title} {movieToDownload.Year}");

            lock (moviesToDownload)
            {
                moviesToDownload.RemoveAll(m => m.Id == movieToDownload.Id);
            }
        }

        public void StopDownload()
        {
            downloadCancellationTokenSource?.Cancel();
        }

        private async Task DownloadMoviesAsync()
        {
            if (running)
                return;

            AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Starts");

            running = true;

            var failedDownloadMovies = new List<LiteContentDto>();

            downloadCancellationTokenSource = new CancellationTokenSource();

            var stackMoviesToDownload = new Stack<LiteContentDto>(moviesToDownload);

            while (stackMoviesToDownload.Any() && !downloadCancellationTokenSource.IsCancellationRequested)
            {
                var movieToDownload = stackMoviesToDownload.Pop();

                //if the movie is not in the list anymore, skip it
                if (!moviesToDownload.Any(m => m.Id == movieToDownload.Id))
                    continue;

                var voDownloadSuccess = await DownloadOriginalVersionAsync(movieToDownload);
                var vfDownloadSuccess = await DownloadFrenchVersionAsync(movieToDownload);

                if (voDownloadSuccess && vfDownloadSuccess && moviesToDownload.Any(m => m.Id == movieToDownload.Id))
                    moviesToDownload.RemoveAll(m => m.Id == movieToDownload.Id);
            }

            if (moviesToDownload.Any() && !downloadCancellationTokenSource.IsCancellationRequested)
            {
                AppLogger.LogInfo($"TorrentAutoDownloader.DownloadMoviesAsync(): {moviesToDownload.Count} movies still present in downloading list, retry in 1 hour");
                var timer = new Timer(_ =>
                {
                    DownloadMoviesAsync();
                }, null, TimeSpan.FromHours(1), Timeout.InfiniteTimeSpan);
            }

            running = false;

            AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Ends");
        }

        private async Task<bool> DownloadOriginalVersionAsync(LiteContentDto movie)
        { 
            if (downloadCancellationTokenSource.IsCancellationRequested)
                return false;

            var videoInfo = videoInfoProvider.GetVideoInfo(movie.Id, LanguageVersion.Original);
            if (videoInfo != null)
                return true;

            AppLogger.LogInfo($"TorrentAutoDownloader: search VO torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(movie.Title, movie.Year);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VO torrents for {movie.Title} {movie.Year}");

            var torrentRequests = torrents.Select(t => new TorrentRequest(TorrentAutoDownloaderIdentifier, t.DownloadUrl, movie.Id, t.Quality, LanguageVersion.Original));
 
            var downloadSuccess = await DownloadBestQualityAsync(torrentRequests);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VO successfully downloaded for {movie.Title} {movie.Year}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadFrenchVersionAsync(LiteContentDto movie)
        { 
            if (downloadCancellationTokenSource.IsCancellationRequested)
                return false;

            var videoInfo = videoInfoProvider.GetVideoInfo(movie.Id, LanguageVersion.French);
            if (videoInfo != null)
               return true;

            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(movie.Id);
            if (string.IsNullOrEmpty(frenchTitle))
               frenchTitle = movie.Title;

            AppLogger.LogInfo($"TorrentAutoDownloader: search VF torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitle, movie.Year);
          
            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VF torrents for {movie.Title} {movie.Year}");

            var torrentRequests = torrents.Select(t => new TorrentRequest(TorrentAutoDownloaderIdentifier, t.DownloadUrl, movie.Id, t.Quality, LanguageVersion.French));

            var downloadSuccess = await DownloadBestQualityAsync(torrentRequests);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VF successfully downloaded for {movie.Title} {movie.Year}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadBestQualityAsync(IEnumerable<TorrentRequest> torrentRequests)
        {
            if (torrentRequests == null || !torrentRequests.Any())
                return false;

            var sortedRequestsByQuality = torrentRequests.Where(tr => qualitiesRank.Contains(tr.VideoInfo.Quality, StringComparer.OrdinalIgnoreCase))
                                        .OrderBy(request => Array.IndexOf(qualitiesRank.Select(q => q.ToLower()).ToArray(), request.VideoInfo.Quality.ToLower()));

            foreach (var torrentRequest in sortedRequestsByQuality)
            {
                if (downloadCancellationTokenSource.IsCancellationRequested)
                    return false;

                AppLogger.LogInfo($"TorrentAutoDownloader: try to download {torrentRequest.TorrentUrl} ({torrentRequest.VideoInfo.Quality})");

                var downloadSuccess = await torrentContentProvider.DownloadTorrentMediaAsync(torrentRequest, downloadCancellationTokenSource.Token);
                if (downloadSuccess)
                    return true;
                else
                    AppLogger.LogInfo($"TorrentAutoDownloader: download failed for {torrentRequest.TorrentUrl}");
            }

            return false;
        }
    }
}
