using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public async Task<IEnumerable<SubtitlesDto>> GetSubtitles([FromQuery(Name = "sourceUrl")] string sourceUrl)
        {
            return await searchersProvider.SubtitlesSearchManager.GetSubtitlesAsync(sourceUrl);
        }
    }
}
