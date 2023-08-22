using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TorrentController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        ITorrentContentProvider torrentVideoStreamProvider;
        ITorrentHistoryProvider torrentHistoryProvider;
        public TorrentController(
           ISearchersProvider searchersProvider,
           ITorrentContentProvider torrentVideoStreamProvider,
           ITorrentHistoryProvider torrentHistoryProvider)
        {
            this.searchersProvider = searchersProvider;
            this.torrentVideoStreamProvider = torrentVideoStreamProvider;
            this.torrentHistoryProvider = torrentHistoryProvider;
        }

        [HttpGet("movies/vf")]
        public async Task<IEnumerable<MediaTorrent>> SearchVFMovieTorrents(string mediaId, string title, int year)
        {
            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(mediaId);

            return await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(string.IsNullOrEmpty(frenchTitle) ? title : frenchTitle, year);
        }

        [HttpGet("series/vf")]
        public async Task<IEnumerable<MediaTorrent>> SearchVFSerieTorrents(string mediaId, string title, int season, int episode)
        {
            var frenchTitle = await searchersProvider.SeriesSearcher.GetSerieFrenchTitleAsync(mediaId);

            return await searchersProvider.TorrentSearchManager.SearchVfTorrentsSerieAsync(string.IsNullOrEmpty(frenchTitle) ? title : frenchTitle, season, episode);
        }

        [HttpGet("movies/vo")]
        public async Task<IEnumerable<MediaTorrent>> SearchVOMovieTorrents(string title, int year)
        {
            return await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(title, year); ;
        }

        [HttpGet("series/vo")]
        public async Task<IEnumerable<MediaTorrent>> SearchVOSerieTorrents(string title, string imdbid, int season, int episode)
        {
            return await searchersProvider.TorrentSearchManager.SearchVoTorrentsSerieAsync(title, imdbid, season, episode); ;
        }

        [HttpGet("stream/movies")]
        public async Task<IActionResult> GetStream(string base64Url)
        {
            var streamDto = await GetStreamDtoAsync(base64Url.DecodeBase64(), RequestFromVLC() ? new VideoTorrentFileSelector() : new Mp4TorrentFileSelector());
            return streamDto != null ? File(streamDto.Stream, streamDto.ContentType, true) : NoContent();
        }


        [HttpGet("stream/series")]
        public async Task<IActionResult> GetStream(string base64Url, int seasonNumber, int episodeNumber)
        {
            var streamDto = await GetStreamDtoAsync(base64Url.DecodeBase64(), RequestFromVLC() ? new SerieEpisodeTorrentFileSelector(seasonNumber, episodeNumber) : new SerieEpisodeMp4TorrentFileSelector(seasonNumber, episodeNumber));
            return streamDto != null ? File(streamDto.Stream, streamDto.ContentType, true) : NoContent();
        }

        [HttpGet("stream/file")]
        public async Task<IActionResult> GetStream(string base64Url, string fileName)
        {
            var streamDto = await GetStreamDtoAsync(base64Url.DecodeBase64(), new ByNameTorrentFileSelector(fileName));
            return streamDto != null ? File(streamDto.Stream, streamDto.ContentType, true) : NoContent();
        }


        [HttpGet("streamdownloadstate")]
        public IActionResult GetStreamDownloadState(string base64TorrentUrl)
        {
            var state = torrentVideoStreamProvider.GetStreamDownloadingState(base64TorrentUrl.DecodeBase64());

            if (state == null)
                return BadRequest();
            else
                return Ok(state);
        }

        [HttpGet("files")]
        public async Task<TorrentInfoDto> GetTorrentFiles(string base64TorrentUrl)
        {
            var url = base64TorrentUrl.DecodeBase64();
            var files = await torrentVideoStreamProvider.GetTorrentFilesAsync(url);

            var torrentInfoDto = new TorrentInfoDto()
            {
                LastOpenedDateTime = DateTime.Now,
                Link = url
            };

            if (files != null)
            {
                await torrentHistoryProvider.SaveTorrentFileHistoryAsync(new TorrentInfoDto() { LastOpenedDateTime = DateTime.Now, Link = url });

                torrentInfoDto.Files = files.ToArray();
            }

            return torrentInfoDto;
        }

        [HttpGet("history")]
        public async Task<IEnumerable<TorrentInfoDto>> GetTorrentFilesHistory()
        {
            var torrentFiles = await torrentHistoryProvider.GetTorrentFilesHistoryAsync();
            return torrentFiles?.OrderByDescending(f => f.LastOpenedDateTime);
        }

        private async Task<StreamDto> GetStreamDtoAsync(string url, ITorrentFileSelector torrentFileSelector)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var rangeHeaderValue = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();

            int offset = 0;
            if (!string.IsNullOrEmpty(rangeHeaderValue))
                int.TryParse(rangeHeaderValue.Replace("bytes=", string.Empty).Split("-")[0], out offset);

            try
            {
                
                return await torrentVideoStreamProvider.GetStreamAsync(url, offset, torrentFileSelector);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool RequestFromVLC()
        {
            var userAgent = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "User-Agent").Value.FirstOrDefault();
            return userAgent != null && userAgent.StartsWith("VLC");
        }
    }
}
