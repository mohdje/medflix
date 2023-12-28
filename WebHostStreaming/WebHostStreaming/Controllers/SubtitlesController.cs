using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Subtitles.DTOs;
using WebHostStreaming.Models;

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

        [HttpGet("movies")]
        public async Task<IEnumerable<SubtitlesSourcesDto>> GetAvailableSubtitles(string imdbCode)
        {
            var frenchSubtitlesSourcesDto = await searchersProvider.SubtitlesSearchManager.GetAvailableMovieSubtitlesUrlsAsync(imdbCode, SubtitlesLanguage.French);
            var englishSubtitlesSourcesDto = await searchersProvider.SubtitlesSearchManager.GetAvailableMovieSubtitlesUrlsAsync(imdbCode, SubtitlesLanguage.English);

            return BuildSubtitlesSourcesDto(frenchSubtitlesSourcesDto, englishSubtitlesSourcesDto);
        }

        [HttpGet("series")]
        public async Task<IEnumerable<SubtitlesSourcesDto>> GetAvailableSubtitles(int seasonNumber, int episodeNumber, string imdbCode)
        {
            var frenchSubtitlesSourcesDto = await searchersProvider.SubtitlesSearchManager.GetAvailableSerieSubtitlesUrlsAsync(seasonNumber, episodeNumber, imdbCode, SubtitlesLanguage.French);
            var englishSubtitlesSourcesDto = await searchersProvider.SubtitlesSearchManager.GetAvailableSerieSubtitlesUrlsAsync(seasonNumber, episodeNumber, imdbCode, SubtitlesLanguage.English);

            return BuildSubtitlesSourcesDto(frenchSubtitlesSourcesDto, englishSubtitlesSourcesDto);
        }

        [HttpGet]
        public async Task<IEnumerable<SubtitlesDto>> GetSubtitles([FromQuery(Name = "sourceUrl")] string sourceUrl)
        {
            return await searchersProvider.SubtitlesSearchManager.GetSubtitlesAsync(sourceUrl);
        }

        private List<SubtitlesSourcesDto> BuildSubtitlesSourcesDto(IEnumerable<string> frenchSubtitlesSourcesDto, IEnumerable<string> englishSubtitlesSourcesDto)
        {

            var subtitles = new List<SubtitlesSourcesDto>();

            if (frenchSubtitlesSourcesDto != null && frenchSubtitlesSourcesDto.Any())
                subtitles.Add(new SubtitlesSourcesDto()
                {
                    Language = "French",
                    SubtitlesSourceUrls = frenchSubtitlesSourcesDto.ToArray()
                });

            if (englishSubtitlesSourcesDto != null && englishSubtitlesSourcesDto.Any())
                subtitles.Add(new SubtitlesSourcesDto()
                {
                    Language = "English",
                    SubtitlesSourceUrls = englishSubtitlesSourcesDto.ToArray()
                });

            return subtitles;
        }

    }
}
