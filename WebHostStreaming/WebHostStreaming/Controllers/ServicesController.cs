using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services;
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
        public ServicesController(IVOMovieSearcherProvider movieServiceProvider)
        {
            this.voMovieSearcherProvider = movieServiceProvider;
        }
        [HttpGet]
        public IEnumerable<object> GetAvailableMovieServices()
        {
            var activeServiceTypeName = voMovieSearcherProvider.GetActiveServiceTypeName();

            return voMovieSearcherProvider.GetAvailableVOMovieSearchers()
                                        .Select(s => new
                                        {
                                            Name = s,
                                            Selected = s == activeServiceTypeName
                                        });
        }

        [HttpGet("active")]
        public string GetActiveServiceName()
        {
           return voMovieSearcherProvider.GetActiveServiceTypeName();
        }

        [HttpPost]
        public IActionResult ChangeMovieService([FromForm] string serviceName)
        {
            try
            {
                voMovieSearcherProvider.UpdateActiveVOMovieSearcher((MovieServiceType)Enum.Parse(typeof(MovieServiceType), serviceName));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return StatusCode(200);
        }
    }
}
