
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        [HttpGet("platform")]
        public IActionResult GetPlatform()
        {
            return Ok(new
            {
                isDesktopApplication = AppConfiguration.IsDesktopApplication
            }); 
        }
    }
}