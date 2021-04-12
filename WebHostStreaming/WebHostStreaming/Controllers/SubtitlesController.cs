using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;
using MoviesAPI.Services.OpenSubtitlesHtml;
using MoviesAPI.Services.OpenSubtitlesHtml.DTOs;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubtitlesController : ControllerBase
    {
        OpenSubtitlesHtmlService subtitlesService;
        public SubtitlesController()
        {
            subtitlesService = new OpenSubtitlesHtmlService();
        }

        [HttpGet("available/{imdbCode}")]
        public async Task<IEnumerable<OpenSubtitlesDto>> GetAvailableSubtitles(string imdbCode)
        {
            var subtitlesDownloaders = new Task<OpenSubtitlesDto>[]
            {
                subtitlesService.GetAvailableSubtitlesAsync(imdbCode, "fre", "French"),
                subtitlesService.GetAvailableSubtitlesAsync(imdbCode, "eng", "English")
            };

            var subtitles = new List<OpenSubtitlesDto>();

            await Task.WhenAll(subtitlesDownloaders).ContinueWith(s =>
            {
                if (s?.Result != null)
                {
                    foreach (var openSubtitleDto in s.Result)
                    {
                        if (openSubtitleDto?.SubtitlesIds != null && openSubtitleDto.SubtitlesIds.Any())
                            subtitles.Add(openSubtitleDto);
                    }
                }
            });

            return subtitles;
        }

        [HttpGet("{subtitlesId}")]
        public IEnumerable<SubtitlesDto> GetSubtitles(string subtitlesId)
        {
            return subtitlesService.GetSubtitles(subtitlesId, Helpers.AppFolders.SubtitlesFolder);
        }
    }
}
