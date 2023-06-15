
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
        IHostApplicationLifetime applicationLifetime;
        IAppUpdater updateChecker;

        private const string WINDOWS_VLC_PATH = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
        private const string MACOS_VLC_PATH = @"/Applications/VLC.app/Contents/MacOS/VLC";

        public ApplicationController(IHostApplicationLifetime applicationLifetime, IAppUpdater updateChecker)
        {
            this.applicationLifetime = applicationLifetime;
            this.updateChecker = updateChecker;
        }


        [HttpGet("stop")]
        public IActionResult StopApp()
        {
            if (AppConfiguration.IsWebVersion)
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
        public IActionResult StartVLC([FromQuery] string data)
        {
            var parameters = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            try
            {
                if (AppConfiguration.IsWindowsVersion)
                {
                    if (!System.IO.File.Exists(WINDOWS_VLC_PATH)) return NotFound();
                    System.Diagnostics.Process.Start(WINDOWS_VLC_PATH, parameters);
                }
                else if (AppConfiguration.IsMacosVersion)
                {
                    if (!System.IO.File.Exists(MACOS_VLC_PATH)) return NotFound();
                    System.Diagnostics.Process.Start(MACOS_VLC_PATH, parameters.Replace(" ", "%20"));
                }

                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(500);
            }
        }

        [HttpGet("platform")]
        public IActionResult GetPlatform()
        {
            return Ok(new
            {
                isDesktopApplication = !AppConfiguration.IsWebVersion
            }); 
        }

        [HttpGet("checkUpdate")]
        public async Task<IActionResult> CheckUpdateAsync()
        {
            if (AppConfiguration.IsWebVersion)
                return BadRequest();

            try
            {
                var updateAvailable = await updateChecker.IsNewReleaseAvailableAsync();
                return Ok(new
                {
                    updateAvailable = updateAvailable
                });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("downloadNewVersion")]
        public async Task<IActionResult> DownloadNewVersionAsync()
        {
            try
            {
                var downloadWithSuccess = await updateChecker.DownloadNewReleaseAsync(AppFiles.NewReleasePackage);

                return downloadWithSuccess ? Ok() : StatusCode(500);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("startUpdate")]
        public IActionResult StartUpdate()
        {
            if (!AppConfiguration.IsWebVersion && Directory.Exists(AppFolders.ExtractUpdateProgramFolder))
            {
                //rename folder so it can be replaced during package extraction
                Directory.Move(AppFolders.ExtractUpdateProgramFolder, AppFolders.ExtractUpdateProgramTempFolder);

                var arguments = new List<string>();
                arguments.Add(AppFiles.NewReleasePackage);
                arguments.Add(AppFolders.CurrentFolder);

                if (AppConfiguration.IsWindowsVersion)
                {
                    arguments.Add(AppFiles.WindowsDesktopApp);
                    System.Diagnostics.Process.Start(AppFiles.WindowsExtractUpdateProgram, arguments);
                }
                else if (AppConfiguration.IsMacosVersion)
                {
                    System.Diagnostics.Process.Start(AppFiles.MacosExtractUpdateProgram, arguments);
                }

                applicationLifetime.StopApplication();
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }
        }

    }
}