
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        IHostApplicationLifetime applicationLifetime;

        private const string WINDOWS_VLC_PATH = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
        private const string MACOS_VLC_PATH = @"/Applications/VLC.app/Contents/MacOS/VLC";

        public ApplicationController(IHostApplicationLifetime applicationLifetime)
        {
            this.applicationLifetime = applicationLifetime;
        }


        [HttpGet("stop")]
        public IActionResult StopApp()
        {
            if (PlatformConfiguration.PlatformIsWeb)
                return BadRequest();

            try
            {
                applicationLifetime.StopApplication();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("startvlc")]
        public IActionResult StartVLC([FromQuery] string url)
        {
            if (PlatformConfiguration.PlatformIsWindows)
            {
                if (!System.IO.File.Exists(WINDOWS_VLC_PATH)) return NotFound();
                System.Diagnostics.Process.Start(WINDOWS_VLC_PATH, url);
            }
            else if (PlatformConfiguration.PlatformIsMacos)
            {
                if (!System.IO.File.Exists(MACOS_VLC_PATH)) return NotFound();
                System.Diagnostics.Process.Start(MACOS_VLC_PATH, url);

            }

            return Ok();
        }

        [HttpGet("platform")]
        public IActionResult GetPlatform()
        {
            return Ok(new
            {
                isDesktopApplication = !PlatformConfiguration.PlatformIsWeb
            }); ;
        }

    }
}