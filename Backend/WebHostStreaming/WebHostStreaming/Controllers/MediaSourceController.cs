using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.Torrent.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MediaSourceController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        IVideoInfoProvider videoInfoProvider;

        public MediaSourceController(ISearchersProvider searchersProvider, IVideoInfoProvider videoInfoProvider)
        {
            this.searchersProvider = searchersProvider;
            this.videoInfoProvider = videoInfoProvider;
        }

        [HttpGet("movies/vf")]
        public async Task<IEnumerable<MediaSource>> SearchVFMovieSources(string mediaId, string title, int year)
        {
            if (TryGetMediaSources(mediaId, LanguageVersion.French, out var mediaSources))
                return mediaSources;

            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(mediaId);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = title;

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitle, year);

            return ToMediaSources(torrents, LanguageVersion.French);
        }

        [HttpGet("movies/vo")]
        public async Task<IEnumerable<MediaSource>> SearchVOMovieSources(string mediaId, string title, int year)
        {
            if (TryGetMediaSources(mediaId, LanguageVersion.Original, out var mediaSources))
                return mediaSources;

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(title, year);

            return ToMediaSources(torrents, LanguageVersion.Original);
        }

        [HttpGet("series/vo")]
        public async Task<IEnumerable<MediaSource>> SearchSerieVOSources(string mediaId, string imdbId, string title, int seasonNumber, int episodeNumber)
        {
            if (TryGetMediaSources(mediaId, LanguageVersion.Original, out var mediaSources, seasonNumber, episodeNumber))
                return mediaSources;

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsSerieAsync(title, imdbId, seasonNumber, episodeNumber);

            return ToMediaSources(torrents, LanguageVersion.Original);
        }

        [HttpGet("series/vf")]
        public async Task<IEnumerable<MediaSource>> SearchSerieVFSources(string mediaId, string title, int seasonNumber, int episodeNumber)
        {
            if (TryGetMediaSources(mediaId, LanguageVersion.French, out var mediaSources, seasonNumber, episodeNumber))
                return mediaSources;

            var frenchTitle = await searchersProvider.SeriesSearcher.GetSerieFrenchTitleAsync(mediaId);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = title;

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsSerieAsync(frenchTitle, seasonNumber, episodeNumber);

            return ToMediaSources(torrents, LanguageVersion.French);
        }

        private bool TryGetMediaSources(string mediaId, LanguageVersion language, out MediaSource[] mediaSources, int seasonNumber = 0, int episodeNumber = 0)
        {
            var videoInfo = videoInfoProvider.GetVideoInfo(mediaId, language, seasonNumber, episodeNumber);

            if (videoInfo != null)
            {
                mediaSources = 
                [
                    new MediaSource
                    {
                        Quality = videoInfo.Quality,
                        FilePath = videoInfo.FilePath,
                        Language = language.ToString()
                    }
                ];
            }
            else
                mediaSources = null;

            return mediaSources != null;
        }

        private IEnumerable<MediaSource> ToMediaSources(IEnumerable<MediaTorrent> torrents, LanguageVersion languageVersion)
        {
            if (torrents != null && torrents.Any())
                return torrents.Select(t => new MediaSource
                {
                    Quality = t.Quality,
                    TorrentUrl = t.DownloadUrl,
                    Language = languageVersion.ToString(),
                });

            return System.Array.Empty<MediaSource>();
        }
    }
}
