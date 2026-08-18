using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Providers;
using WebHostStreaming.Converters;
using MedflixAPI.Services.Subtitles;
using System.IO;
using WebHostStreaming.Extensions;
using WebHostStreaming.Models;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubtitlesController : ControllerBase
    {
        private readonly ISearchersProvider searchersProvider;
        private readonly ISubtitlesConverter subtitlesConverter;

        public SubtitlesController(ISearchersProvider searchersProvider, ISubtitlesConverter subtitlesConverter)
        {
            this.searchersProvider = searchersProvider;
            this.subtitlesConverter = subtitlesConverter;
        }

        [HttpGet("movies/fr")]
        public async Task<IEnumerable<string>> GetAvailableFrenchSubtitlesUrls(string imdbId)
        {
            return await searchersProvider.SubtitlesSearchManager.GetAvailableMovieSubtitlesUrlsAsync(imdbId, SubtitlesLanguage.French);
        }

        [HttpGet("movies/en")]
        public async Task<IEnumerable<string>> GetAvailableEnglishSubtitlesUrls(string imdbId)
        {
            return await searchersProvider.SubtitlesSearchManager.GetAvailableMovieSubtitlesUrlsAsync(imdbId, SubtitlesLanguage.English);
        }

        [HttpGet("series/fr")]
        public async Task<IEnumerable<string>> GetAvailableFrenchSubtitlesUrls(int seasonNumber, int episodeNumber, string imdbId)
        {
            return await searchersProvider.SubtitlesSearchManager.GetAvailableSerieSubtitlesUrlsAsync(seasonNumber, episodeNumber, imdbId, SubtitlesLanguage.French);
        }

        [HttpGet("series/en")]
        public async Task<IEnumerable<string>> GetAvailableEnglishSubtitlesUrls(int seasonNumber, int episodeNumber, string imdbId)
        {
            return await searchersProvider.SubtitlesSearchManager.GetAvailableSerieSubtitlesUrlsAsync(seasonNumber, episodeNumber, imdbId, SubtitlesLanguage.English);
        }

        [HttpGet]
        public async Task<IEnumerable<SubtitlesDto>> GetSubtitles(string sourceUrl)
        {
            var filePath = await searchersProvider.SubtitlesSearchManager.DownloadSubtitlesFileAsync(sourceUrl);
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                var subtitles = await subtitlesConverter.ToSubtitlesDtoAsync(filePath);
                System.IO.File.Delete(filePath);
                return subtitles;
            }
            return [];
        }

        [HttpGet("file/{subtitlesFileUrlBase64}.srt")]
        public async Task<IActionResult> GetSubtitlesFile([FromRoute] string subtitlesFileUrlBase64)
        {
            var url = subtitlesFileUrlBase64.DecodeBase64();
            var filePath = await searchersProvider.SubtitlesSearchManager.DownloadSubtitlesFileAsync(url);

            if (string.IsNullOrEmpty(filePath))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            System.IO.File.Delete(filePath);

            var fileName = Path.GetFileName(filePath);
            Response.Headers.ContentDisposition = $"inline; filename=\"{fileName}\"";

            return File(bytes, "application/x-subrip;", fileName);
        }
    }
}
