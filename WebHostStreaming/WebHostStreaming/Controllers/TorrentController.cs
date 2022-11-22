using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TorrentController : ControllerBase
    {
        ISearchersProvider searchersProvider;
        ITorrentVideoStreamProvider torrentVideoStreamProvider;
        public TorrentController(
           ISearchersProvider searchersProvider,
           ITorrentVideoStreamProvider torrentVideoStreamProvider)
        {
            this.searchersProvider = searchersProvider;
            this.torrentVideoStreamProvider = torrentVideoStreamProvider;
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
        public async Task<IActionResult> GetStream([FromQuery(Name = "url")] string url)
        {
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
                var streamDto = await torrentVideoStreamProvider.GetStreamAsync(url, offset, acceptedFormat);

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


    }
}
