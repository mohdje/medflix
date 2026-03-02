using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Services.Torrent.Dtos;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Torrent
{
    public class TorrentEpisodeDownloader : TorrentMediaDownloader<EpisodeLiteInfo>
    {
        readonly IBookmarkedSeriesProvider bookmarkedSeriesProvider;
        readonly IWatchedSeriesProvider watchedSeriesProvider;

        protected override string MediaType => "episodes";

        public TorrentEpisodeDownloader(
            ITorrentContentProvider torrentContentProvider,
            IVideoInfoProvider videoInfoProvider,
            ISearchersProvider searchersProvider,
            IWatchedSeriesProvider watchedSeriesProvider,
            IBookmarkedSeriesProvider bookmarkedSeriesProvider
        )
            : base(torrentContentProvider, videoInfoProvider, searchersProvider)
        {
            this.watchedSeriesProvider = watchedSeriesProvider;
            this.watchedSeriesProvider.SerieWatchedEpisodeAdded += async (s, watchedEpisode) =>
            {
                var bookmarkedSeries = bookmarkedSeriesProvider.GetBookmarkedSeries();
                if (bookmarkedSeries.Any(serie => serie.Id == watchedEpisode.Media.Id))
                    await TryAddEpisodeToDownloadListAsync(watchedEpisode.Media.Id);
            };

            this.bookmarkedSeriesProvider = bookmarkedSeriesProvider;
            this.bookmarkedSeriesProvider.SerieBookmarkAdded += async (s, serie) => await TryAddEpisodeToDownloadListAsync(serie.Id);
            this.bookmarkedSeriesProvider.SerieBookmarkDeleted += (s, seriesId) =>
            {
                voDownloadList.TryRemove(seriesId, out _);
                vfDownloadList.TryRemove(seriesId, out _);
            };
        }

        protected override async Task<IEnumerable<EpisodeLiteInfo>> GetMediasToDownloadAsync()
        {
            var bookmarkedSeries = bookmarkedSeriesProvider.GetBookmarkedSeries();
            var watchedSeries = watchedSeriesProvider.GetWatchedSeries();
            var episodesToDownload = new List<EpisodeLiteInfo>();

            foreach (var serie in bookmarkedSeries)
            {
                var nextEpisode = await GetNextEpisodeToWatchAsync(serie.Id, watchedSeries);
                if (nextEpisode != null)
                    episodesToDownload.Add(nextEpisode);
            }

            return episodesToDownload;
        }

        private async Task TryAddEpisodeToDownloadListAsync(string serieId)
        {
            var nextEpisode = await GetNextEpisodeToWatchAsync(serieId, watchedSeriesProvider.GetWatchedSeries());
            if (nextEpisode != null)
            {
                TryAddMediaToDownload(nextEpisode);
                OnMediaAddedToDownloadList();
            }
        }

        private async Task<EpisodeLiteInfo> GetNextEpisodeToWatchAsync(string serieId, IEnumerable<WatchedMediaDto> watchedSeries)
        {
            var serieInfo = await searchersProvider.SeriesSearcher.GetSerieDetailsAsync(serieId);
            if (serieInfo == null)
                return null;

            var watchedEpisodes = watchedSeries.Where(s => s.Media.Id == serieInfo.Id);
            if (!watchedEpisodes.Any())
            {
                return new EpisodeLiteInfo
                {
                    SerieId = serieInfo.Id,
                    SerieTitle = serieInfo.Title,
                    ImdbId = serieInfo.ImdbId,
                    SeasonNumber = 1,
                    EpisodeNumber = 1
                };
            }
            else
            {
                var lastSeasonWatched = watchedEpisodes.Max(e => e.SeasonNumber);
                var lastEpisodeWatched = watchedEpisodes.Where(e => e.SeasonNumber == lastSeasonWatched).Max(e => e.EpisodeNumber);
                if (lastSeasonWatched <= serieInfo.SeasonsCount)
                {
                    var episodes = await searchersProvider.SeriesSearcher.GetEpisodes(serieInfo.Id, lastSeasonWatched);
                    if (lastEpisodeWatched < episodes.Count())
                        return new EpisodeLiteInfo
                        {
                            SerieId = serieInfo.Id,
                            SerieTitle = serieInfo.Title,
                            ImdbId = watchedEpisodes.First().Media.ImdbId,
                            SeasonNumber = lastSeasonWatched,
                            EpisodeNumber = lastEpisodeWatched + 1
                        };
                    else if (lastSeasonWatched < serieInfo.SeasonsCount)
                        return new EpisodeLiteInfo
                        {
                            SerieId = serieInfo.Id,
                            SerieTitle = serieInfo.Title,
                            ImdbId = watchedEpisodes.First().Media.ImdbId,
                            SeasonNumber = lastSeasonWatched + 1,
                            EpisodeNumber = 1
                        };
                }
            }
            return null;
        }

        protected override bool VideoExists(EpisodeLiteInfo episode, LanguageVersion languageVersion)
        {
            return videoInfoProvider.GetEpisodeVideoInfo(episode.SerieId, languageVersion, episode.SeasonNumber, episode.EpisodeNumber) != null;
        }

        protected override string GetMediaId(EpisodeLiteInfo episode)
        {
            return episode.SerieId;
        }

        protected override string GetMediaInfo(EpisodeLiteInfo episode)
        {
            return $"{episode.SerieTitle} S{episode.SeasonNumber:D2}E{episode.EpisodeNumber:D2}";
        }

        protected override async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsAsync(EpisodeLiteInfo episode)
        {
            return await searchersProvider.TorrentSearchManager.SearchVoTorrentsSerieAsync(
                episode.SerieTitle,
                episode.ImdbId,
                episode.SeasonNumber,
                episode.EpisodeNumber);
        }

        protected override async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsAsync(EpisodeLiteInfo episode)
        {
            var frenchTitle = await searchersProvider.SeriesSearcher.GetSerieFrenchTitleAsync(episode.SerieId);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = episode.SerieTitle;

            return await searchersProvider.TorrentSearchManager.SearchVfTorrentsSerieAsync(
                frenchTitle,
                episode.SeasonNumber,
                episode.EpisodeNumber);
        }

        protected override IEnumerable<TorrentRequest> BuildTorrentRequests(string clientId, EpisodeLiteInfo episode, IEnumerable<MediaTorrent> torrents, LanguageVersion languageVersion)
        {
            foreach (var torrent in torrents)
                yield return new TorrentRequest(clientId, torrent.DownloadUrl, episode.SerieId, torrent.Quality, languageVersion, episode.SeasonNumber, episode.EpisodeNumber);
        }
    }
}
