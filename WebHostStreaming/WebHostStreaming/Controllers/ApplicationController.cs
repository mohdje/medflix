
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        IHostApplicationLifetime appliationLifeTime;
        public ApplicationController(IHostApplicationLifetime hostApplicationLifetime)
        {
            appliationLifeTime = hostApplicationLifetime;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

        [HttpGet("platform")]
        public IActionResult GetPlatform()
        {
            return Ok(new
            {
                isDesktopApplication = AppConfiguration.IsDesktopApplication
            });  
        }

        [HttpGet("stop")]
        public IActionResult StopApplication()
        {
            if (!AppConfiguration.IsDesktopApplication)
                return NotFound();

            try
            {
                appliationLifeTime.StopApplication();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode((int)HttpStatusCode.OK);
            }
        }
    }
}