using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Services.OpenSubtitlesHtml;
using MoviesAPI.Services.OpenSubtitlesHtml.DTOs;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        [HttpGet("services")]
        public  IEnumerable<object> GetAvailableMovieServices()
        {
            return MovieServiceProvider.AvailableMovieServices
                                        .Select(s => new
                                        {
                                            Name = s,
                                            Selected = s == MovieServiceProvider.ActiveServiceTypeName
                                        }); 
        }

        [HttpPost("services")]
        public IActionResult ChangeMovieService([FromForm] string serviceName)
        {           
            try
            {
                MovieServiceProvider.UpdateActiveMovieService((MovieServiceType)Enum.Parse(typeof(MovieServiceType), serviceName));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }

        [HttpGet("genres")]
        public IEnumerable<string> GetMoviesGenres()
        {
            return MovieServiceProvider.MovieService.GetMovieGenres();
        }

        [HttpGet("suggested")]
        public async Task<IEnumerable<MovieDto>> GetSuggestedMovies()
        {
            return await MovieServiceProvider.MovieService.GetSuggestedMoviesAsync(5);
        }


        [HttpGet("genre/{genre}")]
        public async Task<IEnumerable<MovieDto>> GetLastMoviesByGenre(string genre)
        {
            return await MovieServiceProvider.MovieService.GetLastMoviesByGenreAsync(15, genre);
        }

        [HttpGet("genre/{genre}/{page}")]
        public async Task<IEnumerable<MovieDto>> GetLastMoviesByGenre(string genre, int page)
        {
            return await MovieServiceProvider.MovieService.GetMoviesByGenreAsync(genre, page);
        }

        [HttpGet("search/{text}")]
        public async Task<IEnumerable<MovieDto>> SearchMovies(string text)
        {
            return await MovieServiceProvider.MovieService.GetMoviesByNameAsync(text);
        }

        [HttpGet("details/{id}")]
        public async Task<MovieDto> GetMovieDetails(string id)
        {
            return await MovieServiceProvider.MovieService.GetMoviesDetailsAsync(id);
        }

        [HttpGet("stream")]
        public async Task<FileStreamResult> GetStream([FromQuery(Name = "url")] string url)
        {
            var rangeHeaderValue = HttpContext.Request.Headers.SingleOrDefault(h => h.Key == "Range").Value.FirstOrDefault();

            long offset = 0;
            long.TryParse(rangeHeaderValue.Replace("bytes=", string.Empty).Split("-")[0], out offset);

            return File(await MovieStreamProvider.Instance.GetMovieStreamAsync(url, offset), "video/mp4", true);         
        }

        [HttpGet("subtitles/available/{imdbCode}")]
        public async Task<IEnumerable<OpenSubtitlesDto>> GetAvailableSubtitles(string imdbCode)
        {
            var subtitlesService = MovieServiceProvider.MovieSubtitlesService;

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
                        if(openSubtitleDto?.SubtitlesIds != null && openSubtitleDto.SubtitlesIds.Any())
                            subtitles.Add(openSubtitleDto);
                    }
                }                    
            });

            return subtitles;
        }

        [HttpGet("subtitles/{subtitlesId}")]
        public IEnumerable<SubtitlesDto> GetSubtitles(string subtitlesId)
        {           
            return MovieServiceProvider.MovieSubtitlesService.GetSubtitles(subtitlesId, Helpers.AppFolders.SubtitlesFolder);      
        }
    }
}
