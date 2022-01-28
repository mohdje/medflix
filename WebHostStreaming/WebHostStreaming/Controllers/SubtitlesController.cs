using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Subtitles.DTOs;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubtitlesController : ControllerBase
    {
        SubtitlesSearcher subtitlesSearcher;
        public SubtitlesController(ISubtitlesSearcherProvider subtitlesSearcherProvider)
        {
            this.subtitlesSearcher = subtitlesSearcherProvider.GetActiveSubtitlesSearcher();
        }

        [HttpGet("available/{imdbCode}")]
        public async Task<IEnumerable<SubtitlesSearchResultDto>> GetAvailableSubtitles(string imdbCode)
        {
            var subtitlesDownloaders = new Task<SubtitlesSearchResultDto>[]
            {
                subtitlesSearcher.GetAvailableSubtitlesAsync(imdbCode, SubtitlesLanguage.French),
                subtitlesSearcher.GetAvailableSubtitlesAsync(imdbCode, SubtitlesLanguage.English)
            };

            var subtitles = new List<SubtitlesSearchResultDto>();

            await Task.WhenAll(subtitlesDownloaders).ContinueWith(s =>
            {
                try
                {
                    if (s?.Result != null)
                    {
                        foreach (var openSubtitleDto in s.Result)
                        {
                            if (openSubtitleDto?.SubtitlesSourceUrls != null && openSubtitleDto.SubtitlesSourceUrls.Any())
                                subtitles.Add(openSubtitleDto);
                        }
                    }
                }
                catch (Exception)
                {

                   
                }
               
            });

            return subtitles;
        }

        [HttpGet]
        public IEnumerable<SubtitlesDto> GetSubtitles([FromQuery(Name = "sourceUrl")] string sourceUrl)
        {
            return subtitlesSearcher.GetSubtitles(sourceUrl);
        }
    }
}
