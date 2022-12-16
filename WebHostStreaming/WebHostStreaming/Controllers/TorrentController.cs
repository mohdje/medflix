using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("vf")]
        public async Task<IEnumerable<MovieTorrent>> SearchVFTorrents([FromQuery(Name = "movieId")] string movieId, [FromQuery(Name = "originalTitle")] string originalTitle, [FromQuery(Name = "year")] int year)
        {
            var frenchTitle = await searchersProvider.MovieSearcher.GetFrenchTitleAsync(movieId);

            return await searchersProvider.TorrentSearchManager.SearchVfTorrentsAsync(string.IsNullOrEmpty(frenchTitle) ? originalTitle : frenchTitle, year);
        }

        [HttpGet("vo")]
        public async Task<IEnumerable<MovieTorrent>> SearchVOTorrents([FromQuery(Name = "title")] string title, [FromQuery(Name = "year")] int year)
        {
            return await searchersProvider.TorrentSearchManager.SearchVoTorrentsAsync(title, year); ;
        }

        [HttpGet("stream")]
        public async Task<IActionResult> GetStream([FromQuery(Name = "url")] string url, [FromQuery(Name = "fileName")] string fileName)
        {
            url = Helpers.TestData.JungleBookTorrentUrl;
            if (string.IsNullOrEmpty(url))
                return NoContent();

            var rangeHeaderValue = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();
            var userAgent = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "User-Agent").Value.FirstOrDefault();

            int offset = 0;
            if (!string.IsNullOrEmpty(rangeHeaderValue))
                int.TryParse(rangeHeaderValue.Replace("bytes=", string.Empty).Split("-")[0], out offset);

            try
            {
                var acceptedFormat = userAgent.StartsWith("VLC") ? "*" : ".mp4";
                var streamDto = await torrentVideoStreamProvider.GetStreamAsync(url, offset, torrentFilePath => FileSelector(torrentFilePath, acceptedFormat, fileName));

                if (streamDto != null)
                    return File(streamDto.Stream, streamDto.ContentType, true);
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetStream error:" + ex);
                return NoContent();
            }
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
        public async Task<IEnumerable<string>> GetTorrentFiles([FromQuery(Name = "torrentUrl")] string torrentUrl)
        {
            var files = await torrentVideoStreamProvider.GetTorrentFilesAsync(torrentUrl);

            if (files != null)
            {
                await torrentHistoryProvider.SaveTorrentFileHistoryAsync(new TorrentInfoDto() { LastOpenedDateTime = DateTime.Now, Link = torrentUrl });
            }

            return files;
        }

        [HttpGet("history")]
        public async Task<IEnumerable<TorrentInfoDto>> GetTorrentFilesHistory()
        {
            var torrentFiles = await torrentHistoryProvider.GetTorrentFilesHistoryAsync();
            return torrentFiles?.OrderByDescending(f => f.LastOpenedDateTime);
        }

        private bool FileSelector(string filePath, string accepetedVideoExtension, string fileNameToSelect)
        {
            if (!string.IsNullOrEmpty(fileNameToSelect))
                return Path.GetFileName(filePath) == fileNameToSelect;

            if (accepetedVideoExtension == "*")
                return filePath.EndsWith(".mp4") || filePath.EndsWith(".avi") || filePath.EndsWith(".mkv");
            else
                return filePath.EndsWith(accepetedVideoExtension);
        }
    }
}
