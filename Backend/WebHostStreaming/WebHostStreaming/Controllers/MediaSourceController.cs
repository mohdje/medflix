using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Extensions;
using MoviesAPI.Services.Torrent.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;
using WebHostStreaming.Providers.AvailableVideosListProvider;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MediaSourceController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        IAvailableVideosListProvider availableVideosListProvider;

        public MediaSourceController(ISearchersProvider searchersProvider, IAvailableVideosListProvider availableVideosListProvider)
        {
            this.searchersProvider = searchersProvider;
            this.availableVideosListProvider = availableVideosListProvider;
        }

        [HttpGet("movies/vf")]
        public async Task<IEnumerable<MediaSource>> SearchVFMovieSources(string mediaId, string title, int year)
        {
            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(mediaId);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = title;

            var videoPath = await availableVideosListProvider.GetVfMovieSource(title, frenchTitle, year);

            if (!string.IsNullOrEmpty(videoPath))
                return ToMediaSources(videoPath);

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitle, year);

            return ToMediaSources(torrents);
        }

        [HttpGet("movies/vo")]
        public async Task<IEnumerable<MediaSource>> SearchVOMovieSources(string title, int year)
        {
            var videoPath = await availableVideosListProvider.GetVoMovieSource(title, year);

            if (!string.IsNullOrEmpty(videoPath))
                return ToMediaSources(videoPath);

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(title, year);

            return ToMediaSources(torrents);
        }

        [HttpGet("series/vo")]
        public async Task<IEnumerable<MediaSource>> SearchSerieVOSources(string imdbId, string title, int seasonNumber, int episodeNumber)
        {
            var videoPath = await availableVideosListProvider.GetVoSerieSource(title, seasonNumber, episodeNumber);

            if (!string.IsNullOrEmpty(videoPath))
                return ToMediaSources(videoPath);

            var torrents = await searchersProvider.TorrentSearchManager.SearchVoTorrentsSerieAsync(title, imdbId, seasonNumber, episodeNumber);

            return ToMediaSources(torrents);
        }

        [HttpGet("series/vf")]
        public async Task<IEnumerable<MediaSource>> SearchSerieVFSources(string mediaId, string title, int seasonNumber, int episodeNumber)
        {
            var frenchTitle = await searchersProvider.SeriesSearcher.GetSerieFrenchTitleAsync(mediaId);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = title;

            var videoPath = await availableVideosListProvider.GetVfSerieSource(title, frenchTitle, seasonNumber, episodeNumber);
            if (!string.IsNullOrEmpty(videoPath))
                return ToMediaSources(videoPath);

            var torrents = await searchersProvider.TorrentSearchManager.SearchVfTorrentsSerieAsync(frenchTitle, seasonNumber, episodeNumber);

            return ToMediaSources(torrents);
        }

        private IEnumerable<MediaSource> ToMediaSources(IEnumerable<MediaTorrent> torrents)
        {
            if (torrents != null && torrents.Any())
                return torrents.Select(t => new MediaSource
                {
                    Quality = t.Quality,
                    TorrentUrl = t.DownloadUrl
                });

            return System.Array.Empty<MediaSource>();
        }

        private IEnumerable<MediaSource> ToMediaSources(string videoPath)
        {
            return new MediaSource[]
            {
                new MediaSource
                {
                    Quality = videoPath.GetVideoQuality(),
                    FilePath = videoPath
                }
            };
        }
    }
}
