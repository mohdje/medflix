using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        ITorrentAutoDownloader torrentAutoDownloader;
        public TorrentController(
           ITorrentContentProvider torrentClientProvider,
           ITorrentAutoDownloader torrentAutoDownloader)
        {
            this.torrentClientProvider = torrentClientProvider;
            this.torrentAutoDownloader = torrentAutoDownloader;
        }

        [HttpGet("stream/movies")]
        public async Task<IActionResult> GetStream(string base64TorrentUrl, string mediaId, string language, string quality)
        {
            if (!Enum.TryParse<LanguageVersion>(language, true, out var languageVersion))
                return BadRequest("Invalid language value");

            var clientAppIdientifier = HttpContext.Request.GetClientAppIdentifier();

            if (string.IsNullOrEmpty(clientAppIdientifier))
                return Forbid();

            var torrentUrl = base64TorrentUrl.DecodeBase64();

            return await StreamData(new TorrentRequest(clientAppIdientifier, torrentUrl, mediaId, quality, languageVersion));
        }

        [HttpGet("stream/series")]
        public async Task<IActionResult> GetStream(string base64TorrentUrl, int seasonNumber, int episodeNumber, string mediaId, string language, string quality)
        {
            if (!Enum.TryParse<LanguageVersion>(language, true, out var languageVersion))
                return BadRequest("Invalid languageVersion value");

            var clientAppIdientifier = HttpContext.Request.GetClientAppIdentifier();

            if (string.IsNullOrEmpty(clientAppIdientifier))
                return Forbid();

            var torrentUrl = base64TorrentUrl.DecodeBase64();
            return await StreamData(new TorrentRequest(clientAppIdientifier, torrentUrl, mediaId, quality, languageVersion, seasonNumber, episodeNumber));
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

        private async Task<IActionResult> StreamData(TorrentRequest torrentRequest)
        {
            torrentAutoDownloader.StopDownload();// stop auto downloader to prioritize the current request for streaming

            var torrentStream = await torrentClientProvider.GetTorrentStreamAsync(torrentRequest);
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

            AppLogger.LogInfo(torrentRequest.ClientAppId, $"Stream retrieved for TorrentManager : {torrentStream.MediaFileName}");

            return fileContentResult;
        }
    }
}
