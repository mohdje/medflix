using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services;
using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        IVOMovieSearcherProvider voMovieSearcherProvider;
        IVFMovieSearcherProvider vfMovieSearcherProvider;
        ISubtitlesSearcherProvider subtitlesSearcherProvider;
        public ServicesController(
            IVOMovieSearcherProvider voMovieSearcherProvider, 
            IVFMovieSearcherProvider vfMovieSearcherProvider,
            ISubtitlesSearcherProvider subtitlesSearcherProvider
            )
        {
            this.voMovieSearcherProvider = voMovieSearcherProvider;
            this.vfMovieSearcherProvider = vfMovieSearcherProvider;
            this.subtitlesSearcherProvider = subtitlesSearcherProvider;
        }
        [HttpGet("vo")]
        public IEnumerable<ServiceInfo> GetVOMovieServices()
        {
            return voMovieSearcherProvider.GetVOMoviesServicesInfo();
        }

        [HttpPost("vo")]
        public IActionResult SelectVOMovieService([FromForm] int selectedVOMovieServiceId)
        {
            try
            {
                voMovieSearcherProvider.UpdateSelectedVOMovieSearcher(selectedVOMovieServiceId);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }

        [HttpGet("vf")]
        public IEnumerable<ServiceInfo> GetVFMovieServices()
        {
            return vfMovieSearcherProvider.GetVFMoviesServicesInfo();
        }

        [HttpPost("vf")]
        public IActionResult SelectVFMovieService([FromForm] int selectedVFMovieServiceId)
        {
            try
            {
                vfMovieSearcherProvider.UpdateSelectedVFMovieSearcher(selectedVFMovieServiceId);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }

        [HttpGet("subtitles")]
        public IEnumerable<ServiceInfo> GetSubtitlesServices()
        {
            return subtitlesSearcherProvider.GetSubtitlesServicesInfo();
        }

        [HttpPost("subtitles")]
        public IActionResult SelectSubtitlesService([FromForm] int selectedSubtitlesServiceId)
        {
            try
            {
                subtitlesSearcherProvider.UpdateSelectedSubtitlesSearcher(selectedSubtitlesServiceId);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }
    }
}
