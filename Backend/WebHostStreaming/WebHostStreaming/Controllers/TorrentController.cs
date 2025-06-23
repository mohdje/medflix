using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TorrentController : ControllerBase
    {
        ITorrentContentProvider torrentClientProvider;
        ITorrentHistoryProvider torrentHistoryProvider;
        ITorrentAutoDownloader torrentAutoDownloader;
        public TorrentController(
           ITorrentContentProvider torrentClientProvider,
           ITorrentHistoryProvider torrentHistoryProvider,
           ITorrentAutoDownloader torrentAutoDownloader)
        {
            this.torrentClientProvider = torrentClientProvider;
            this.torrentHistoryProvider = torrentHistoryProvider;
            this.torrentAutoDownloader = torrentAutoDownloader;
        }

        [HttpGet("stream/movies")]
        public async Task<IActionResult> GetStream(string base64TorrentUrl)
        {
            var torrentUrl = base64TorrentUrl.DecodeBase64();
            return await StreamData(torrentUrl, new VideoTorrentFileSelector());
        }

        [HttpGet("stream/series")]
        public async Task<IActionResult> GetStream(string base64TorrentUrl, int seasonNumber, int episodeNumber)
        {
            var torrentUrl = base64TorrentUrl.DecodeBase64();
            return await StreamData(torrentUrl, new SerieEpisodeTorrentFileSelector(seasonNumber, episodeNumber));
        }

        [HttpGet("stream/file")]
        public async Task<IActionResult> GetStream(string base64Url, string fileName)
        {
            var torrentUrl = base64Url.DecodeBase64();
            return await StreamData(torrentUrl, new ByNameTorrentFileSelector(fileName));
        }


        [HttpGet("streamdownloadstate")]
        public async Task<IActionResult> GetStreamDownloadState(string base64TorrentUrl)
        {
            var clientAppIdientifier = HttpContext.Request.GetClientAppIdentifier();

            if (string.IsNullOrEmpty(clientAppIdientifier))
                return Forbid();

            if (string.IsNullOrEmpty(base64TorrentUrl))
                return BadRequest();

            var url = base64TorrentUrl.DecodeBase64();

            var state = await torrentClientProvider.GetDownloadingStateAsync(clientAppIdientifier, url);
            if (state == null)
                return BadRequest();
            else
                return Ok(state);
        }

        [HttpGet("files")]
        public async Task<TorrentInfoDto> GetTorrentFiles(string base64TorrentUrl)
        {
            return null;
            //var url = base64TorrentUrl.DecodeBase64();
            //var files = await torrentClientProvider.GetTorrentFilesAsync(url);

            //var torrentInfoDto = new TorrentInfoDto()
            //{
            //    LastOpenedDateTime = DateTime.Now,
            //    Link = url
            //};

            //if (files != null)
            //{
            //    await torrentHistoryProvider.SaveTorrentFileHistoryAsync(new TorrentInfoDto() { LastOpenedDateTime = DateTime.Now, Link = url });

            //    torrentInfoDto.Files = files.ToArray();
            //}

            //return torrentInfoDto;
        }

        [HttpGet("history")]
        public async Task<IEnumerable<TorrentInfoDto>> GetTorrentFilesHistory()
        {
            var torrentFiles = await torrentHistoryProvider.GetTorrentFilesHistoryAsync();
            return torrentFiles?.OrderByDescending(f => f.LastOpenedDateTime);
        }

        private async Task<IActionResult> StreamData(string torrentUrl, ITorrentFileSelector torrentFileSelector)
        {
            torrentAutoDownloader.StopDownload();// stop auto downloader to prioritize the current request for streaming

            var clientAppIdientifier = HttpContext.Request.GetClientAppIdentifier();

            if (string.IsNullOrEmpty(clientAppIdientifier))
                return Forbid();

            var torrentStream = await torrentClientProvider.GetTorrentStreamAsync(clientAppIdientifier, torrentUrl, torrentFileSelector);
            if (torrentStream == null)
                return NotFound();

            var stream = await torrentStream.GetStreamAsync();
            if (stream == null)
                return NotFound();

            long firstByte = 0;

            var range = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();

            if (range != null && range.StartsWith("bytes="))
            {
                var parts = range.Substring("bytes=".Length).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                    firstByte = long.Parse(parts[0]);
            }

            stream.Seek(firstByte, SeekOrigin.Begin);

            var fileContentResult = File(stream, "video/mp4");
            fileContentResult.EnableRangeProcessing = true;

            AppLogger.LogInfo(clientAppIdientifier, $"Stream retrieved for TorrentManager : {torrentStream.MediaFileName}");

            return fileContentResult;
        }
    }
}
