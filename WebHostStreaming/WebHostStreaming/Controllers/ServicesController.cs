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
            return voMovieSearcherProvider.GetVOMoviesServicesInfo().OrderBy(s => s.Id);
        }

        [HttpGet("vo/selected")]
        public ServiceInfo GetActiveVOMovieServices()
        {
            return voMovieSearcherProvider.GetSelectedVOMoviesServiceInfo(false);
        }

        [HttpPost("vo")]
        public IActionResult SelectVOMovieService([FromForm] int selectedServiceId)
        {
            try
            {
                voMovieSearcherProvider.UpdateSelectedVOMovieSearcher(selectedServiceId);
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
            return vfMovieSearcherProvider.GetVFMoviesServicesInfo().OrderBy(s => s.Id);
        }

        [HttpPost("vf")]
        public IActionResult SelectVFMovieService([FromForm] int selectedServiceId)
        {
            try
            {
                vfMovieSearcherProvider.UpdateSelectedVFMovieSearcher(selectedServiceId);
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
            return subtitlesSearcherProvider.GetSubtitlesServicesInfo().OrderBy(s => s.Id);
        }

        [HttpPost("subtitles")]
        public IActionResult SelectSubtitlesService([FromForm] int selectedServiceId)
        {
            try
            {
                subtitlesSearcherProvider.UpdateSelectedSubtitlesSearcher(selectedServiceId);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }
    }
}
