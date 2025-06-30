using System.Threading.Tasks;
using WebHostStreaming.Providers;
using WebHostStreaming.Providers.AvailableVideosListProvider;
using MoviesAPI.Services.Content.Dtos;
using System.Linq;
using System;
using WebHostStreaming.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using System.Collections.Generic;
using System.Threading;


namespace WebHostStreaming.Torrent
{
    public class TorrentAutoDownloader : ITorrentAutoDownloader
    {
        IAvailableVideosListProvider availableVideosListProvider;
        ISearchersProvider searchersProvider;
        ITorrentContentProvider torrentContentProvider;

        readonly string[] qualitiesRank = { "2160p", "1080p", "720p", "WEBRIP" };
        const string TorrentAutoDownloaderIdentifier = "Auto-Downloader";
        bool running;
        List<LiteContentDto> moviesToDownload = new List<LiteContentDto>();
        CancellationTokenSource downloadCancellationTokenSource;

        public TorrentAutoDownloader(
            IAvailableVideosListProvider availableVideosListProvider,
            ISearchersProvider searchersProvider,
            ITorrentContentProvider torrentContentProvider)
        {
            this.availableVideosListProvider = availableVideosListProvider;
            this.searchersProvider = searchersProvider;
            this.torrentContentProvider = torrentContentProvider;
            torrentContentProvider.OnNoActiveTorrentClient += (s, e) => DownloadMoviesAsync();
        }

        public void AddToDownloadList(LiteContentDto movieToDownload)
        {
            AppLogger.LogInfo($"TorrentAutoDownloader: Add to the list {movieToDownload.Title} {movieToDownload.Year}");

            lock (moviesToDownload)
            {
                if (!moviesToDownload.Any(movie => movie.Id == movieToDownload.Id))
                    moviesToDownload.Add(movieToDownload);
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
                }, null, TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan);
            }

            running = false;

            AppLogger.LogInfo("TorrentAutoDownloader.DownloadMoviesAsync(): Ends");
        }

        private async Task<bool> DownloadOriginalVersionAsync(LiteContentDto movie)
        { 
            if (downloadCancellationTokenSource.IsCancellationRequested)
                return false;

            var videoPath = availableVideosListProvider.GetVoMovieSource(movie.Title, movie.Year);
            if (!string.IsNullOrEmpty(videoPath))
                return true;

            AppLogger.LogInfo($"TorrentAutoDownloader: search VO torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(movie.Title, movie.Year);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VO torrents for {movie.Title} {movie.Year}");

            var downloadSuccess = await DownloadBestQualityAsync(torrents);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VO successfully downloaded for {movie.Title} {movie.Year}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadFrenchVersionAsync(LiteContentDto movie)
        { 
            if (downloadCancellationTokenSource.IsCancellationRequested)
                return false;

            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(movie.Id);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = movie.Title;

            var videoPath = availableVideosListProvider.GetVfMovieSource(movie.Title, frenchTitle, movie.Year);
            if (!string.IsNullOrEmpty(videoPath))
                return true;

            AppLogger.LogInfo($"TorrentAutoDownloader: search VF torrents for {movie.Title} {movie.Year}");

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitle, movie.Year);

            AppLogger.LogInfo($"TorrentAutoDownloader: found {torrents.Count()} VF torrents for {movie.Title} {movie.Year}");

            var downloadSuccess = await DownloadBestQualityAsync(torrents);

            if (downloadSuccess)
                AppLogger.LogInfo($"TorrentAutoDownloader: VF successfully downloaded for {movie.Title} {movie.Year}");

            return downloadSuccess;
        }

        private async Task<bool> DownloadBestQualityAsync(IEnumerable<MediaTorrent> torrents)
        {
            if (torrents == null || !torrents.Any())
                return false;

            var sortedTorrentsByQuality = torrents.OrderBy(t => Array.IndexOf(qualitiesRank, t.Quality));

            foreach (var torrent in sortedTorrentsByQuality)
            {
                if (downloadCancellationTokenSource.IsCancellationRequested)
                    return false;

                AppLogger.LogInfo($"TorrentAutoDownloader: try to download {torrent.DownloadUrl} ({torrent.Quality})");

                var downloadSuccess = await torrentContentProvider.DownloadTorrentMediaAsync(torrent.DownloadUrl, TorrentAutoDownloaderIdentifier, new VideoTorrentFileSelector(), downloadCancellationTokenSource.Token);
                if (downloadSuccess)
                    return true;
                else
                    AppLogger.LogInfo($"TorrentAutoDownloader: download failed for {torrent.DownloadUrl}");
            }

            return false;
        }
    }
}
