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
        ISearchersProvider searchersProvider;
        public SubtitlesController(ISearchersProvider searchersProvider)
        {
            this.searchersProvider = searchersProvider;
        }

        [HttpGet("available/{imdbCode}")]
        public async Task<IEnumerable<SubtitlesSearchResultDto>> GetAvailableSubtitles(string imdbCode)
        {
            var subtitlesDownloaders = new Task<SubtitlesSearchResultDto>[]
            {
                searchersProvider.ActiveSubtitlesSearcher.GetAvailableSubtitlesAsync(imdbCode, SubtitlesLanguage.French),
                searchersProvider.ActiveSubtitlesSearcher.GetAvailableSubtitlesAsync(imdbCode, SubtitlesLanguage.English)
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
        public async Task<IEnumerable<SubtitlesDto>> GetSubtitles([FromQuery(Name = "sourceUrl")] string sourceUrl)
        {
            return await searchersProvider.ActiveSubtitlesSearcher.GetSubtitlesAsync(sourceUrl);
        }
    }
}
