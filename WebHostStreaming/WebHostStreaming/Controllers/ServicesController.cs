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
        ISearchersProvider searchersProvider;

        public ServicesController(ISearchersProvider searchersProvider)
        {
            this.searchersProvider = searchersProvider;
        }
        [HttpGet("vo")]
        public async Task<IEnumerable<ServiceInfo>> GetVOMovieServices()
        {
            var services = await searchersProvider.GetVOMoviesServicesInfo();
            return services.OrderBy(s => s.Id);
        }

        [HttpGet("vo/selected")]
        public async Task<ServiceInfo> GetActiveVOMovieServices()
        {
            return await searchersProvider.GetSelectedVOMoviesServiceInfoAsync(false);
        }

        [HttpPost("vo")]
        public IActionResult SelectVOMovieService([FromForm] int selectedServiceId)
        {
            try
            {
                searchersProvider.UpdateSelectedVOMovieSearcher(selectedServiceId);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }

        [HttpGet("vf")]
        public async Task<IEnumerable<ServiceInfo>> GetVFMovieServices()
        {
            var services = await searchersProvider.GetVFMoviesServicesInfo();
            return services.OrderBy(s => s.Id);
        }

        [HttpGet("subtitles")]
        public async Task<IEnumerable<ServiceInfo>> GetSubtitlesServices()
        {
            var services = await searchersProvider.GetSubtitlesServicesInfo();
            return services.OrderBy(s => s.Id);
        }

        [HttpPost("save")]
        public IActionResult SaveSelectedServices([FromForm] int? selectedVOServiceId, [FromForm] int? selectedVFServiceId, [FromForm] int? selectedSubtitlesServiceId)
        {
            try
            {
                if(selectedSubtitlesServiceId.HasValue)
                    searchersProvider.UpdateSelectedSubtitlesSearcher(selectedSubtitlesServiceId.Value);

                if (selectedVFServiceId.HasValue)
                    searchersProvider.UpdateSelectedVFMovieSearcher(selectedVFServiceId.Value);

                if (selectedVOServiceId.HasValue)
                    searchersProvider.UpdateSelectedVOMovieSearcher(selectedVOServiceId.Value);

                searchersProvider.SaveSources();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }
    }
}
