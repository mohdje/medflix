using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;

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
        public async Task<IActionResult> GetStream(string url)
        {
            var streamDto = await GetStreamDtoAsync(url, torrentFilePath => SelectFileByFormat(torrentFilePath, GetAcceptedFormat()));
            return streamDto != null ? File(streamDto.Stream, streamDto.ContentType, true) : NoContent();
        }

        [HttpGet("stream/series")]
        public async Task<IActionResult> GetStream(string url, int seasonNumber, int episodeNumber)
        {
            var streamDto = await GetStreamDtoAsync(url, torrentFilePath => SelectFileBySeasonAndEpisode(torrentFilePath, seasonNumber, episodeNumber));
            return streamDto != null ? File(streamDto.Stream, streamDto.ContentType, true) : NoContent();
        }

        [HttpGet("stream/file")]
        public async Task<IActionResult> GetStream(string url, string fileName)
        {
            var streamDto = await GetStreamDtoAsync(url, torrentFilePath => SelectFileByName(torrentFilePath, fileName));
            return streamDto != null ? File(streamDto.Stream, streamDto.ContentType, true) : NoContent();
        }


        [HttpGet("streamdownloadstate")]
        public IActionResult GetStreamDownloadState([FromQuery(Name = "torrentUrl")] string torrentUrl)
        {
            var state = torrentVideoStreamProvider.GetStreamDownloadingState(torrentUrl);

            if (state == null)
                return BadRequest();
            else
                return Ok(state);
        }

        [HttpGet("files")]
        public async Task<TorrentInfoDto> GetTorrentFiles([FromQuery(Name = "torrentUrl")] string torrentUrl)
        {
            var files = await torrentVideoStreamProvider.GetTorrentFilesAsync(torrentUrl);

            var torrentInfoDto = new TorrentInfoDto()
            {
                LastOpenedDateTime = DateTime.Now,
                Link = torrentUrl
            };

            if (files != null)
            {
                await torrentHistoryProvider.SaveTorrentFileHistoryAsync(new TorrentInfoDto() { LastOpenedDateTime = DateTime.Now, Link = torrentUrl });

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

        private async Task<StreamDto> GetStreamDtoAsync(string url, Func<string,bool> fileSelector)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            var rangeHeaderValue = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();

            int offset = 0;
            if (!string.IsNullOrEmpty(rangeHeaderValue))
                int.TryParse(rangeHeaderValue.Replace("bytes=", string.Empty).Split("-")[0], out offset);

            try
            {
                
                return await torrentVideoStreamProvider.GetStreamAsync(url, offset, fileSelector);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string GetAcceptedFormat()
        {
            var userAgent = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "User-Agent").Value.FirstOrDefault();
            return userAgent != null && userAgent.StartsWith("VLC") ? "*" : ".mp4";
        }

        private bool SelectFileByName(string filePath, string fileNameToSelect)
        {
            return Path.GetFileName(filePath) == fileNameToSelect;
        }

        private bool SelectFileBySeasonAndEpisode(string filePath, int seasonNumber, int episodeNumber)
        {
            var fileName = Path.GetFileName(filePath);

            var season = seasonNumber < 10 ? "0" + seasonNumber.ToString() : seasonNumber.ToString();
            var episode = episodeNumber < 10 ? "0" + episodeNumber.ToString() : episodeNumber.ToString();

            return fileName.Contains($"S{season}E{episode}");
        }

        private bool SelectFileByFormat(string filePath, string accepetedVideoExtension)
        {
            if (accepetedVideoExtension == "*")
                return filePath.EndsWith(".mp4") || filePath.EndsWith(".avi") || filePath.EndsWith(".mkv");
            else
                return filePath.EndsWith(accepetedVideoExtension);
        }
    }
}
