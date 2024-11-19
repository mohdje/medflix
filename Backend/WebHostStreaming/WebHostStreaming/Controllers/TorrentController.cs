using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        ITorrentClientProvider torrentClientProvider;
        ITorrentHistoryProvider torrentHistoryProvider;
        public TorrentController(
           ISearchersProvider searchersProvider,
           ITorrentClientProvider torrentClientProvider,
           ITorrentHistoryProvider torrentHistoryProvider)
        {
            this.searchersProvider = searchersProvider;
            this.torrentClientProvider = torrentClientProvider;
            this.torrentHistoryProvider = torrentHistoryProvider;
        }

        [HttpGet("stream/movies")]
        public async Task<IActionResult> GetStream(string base64TorrentUrl)
        {
            var torrentUrl = base64TorrentUrl.DecodeBase64();
            return await StreamData(torrentUrl, RequestFromVLC() ? new VideoTorrentFileSelector() : new Mp4TorrentFileSelector());
        }

        [HttpGet("stream/series")]
        public async Task<IActionResult> GetStream(string base64TorrentUrl, int seasonNumber, int episodeNumber)
        {
            var torrentUrl = base64TorrentUrl.DecodeBase64();
            return await StreamData(torrentUrl, RequestFromVLC() ? new SerieEpisodeTorrentFileSelector(seasonNumber, episodeNumber) : new SerieEpisodeMp4TorrentFileSelector(seasonNumber, episodeNumber));
        }

        [HttpGet("stream/file")]
        public async Task<IActionResult> GetStream(string base64Url, string fileName)
        {
            var torrentUrl = base64Url.DecodeBase64();
            return await StreamData(torrentUrl, new ByNameTorrentFileSelector(fileName));
        }


        [HttpGet("streamdownloadstate")]
        public IActionResult GetStreamDownloadState(string base64TorrentUrl)
        {
            if (string.IsNullOrEmpty(base64TorrentUrl))
                return BadRequest();

            var url = base64TorrentUrl.DecodeBase64();
            var state = torrentClientProvider.GetStreamDownloadingState(url);

            if (state == null)
                return BadRequest();
            else
                return Ok(state);
        }

        [HttpGet("files")]
        public async Task<TorrentInfoDto> GetTorrentFiles(string base64TorrentUrl)
        {
            var url = base64TorrentUrl.DecodeBase64();
            var files = await torrentClientProvider.GetTorrentFilesAsync(url);

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


        private async Task<IActionResult> StreamData(string torrentUrl, ITorrentFileSelector torrentFileSelector)
        {
            var stream = await torrentClientProvider.GetTorrentStreamAsync(torrentUrl, torrentFileSelector);

            if (stream == null)
                return NoContent();

            long firstByte = 0;
          
            var range = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();

            if (range != null && range.StartsWith("bytes="))
            {
                var parts = range.Substring("bytes=".Length).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    firstByte = long.Parse(parts[0]);
                }
            }

            stream.Seek(firstByte, SeekOrigin.Begin);

            var contentType = torrentClientProvider.GetContentType(torrentUrl);

            var fileContentResult = File(stream, contentType);
            fileContentResult.EnableRangeProcessing = true;

            return fileContentResult;
        }
        private bool RequestFromVLC()
        {
            var userAgent = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "User-Agent").Value.FirstOrDefault();
            return userAgent != null && userAgent.StartsWith("VLC");
        }
    }
}
