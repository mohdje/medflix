
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

        public ApplicationController(IHostApplicationLifetime applicationLifetime, IAppUpdater updateChecker)
        {
            this.applicationLifetime = applicationLifetime;
            this.updateChecker = updateChecker;
        }

        [HttpGet("platform")]
        public IActionResult GetPlatform()
        {
            return Ok(new
            {
                isDesktopApplication = AppConfiguration.IsDesktopApplication
            }); 
        }

        [HttpGet("checkUpdate")]
        public async Task<IActionResult> CheckUpdateAsync()
        {
            if (!AppConfiguration.IsDesktopApplication)
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
            if (AppConfiguration.IsDesktopApplication && Directory.Exists(AppFolders.ExtractUpdateProgramFolder))
            {
                //rename folder so it can be replaced during package extraction
                Directory.Move(AppFolders.ExtractUpdateProgramFolder, AppFolders.ExtractUpdateProgramTempFolder);

                var arguments = new List<string>();
                arguments.Add(AppFiles.NewReleasePackage);

                //if (AppConfiguration.IsWindowsVersion)
                //{
                //    arguments.Add(AppFolders.CurrentFolder);
                //    arguments.Add(AppFiles.WindowsDesktopApp);
                //    System.Diagnostics.Process.Start(AppFiles.WindowsExtractUpdateProgram, arguments);
                //}
                //else if (AppConfiguration.IsMacosVersion)
                //{
                //    arguments.Add("/Applications");
                //    System.Diagnostics.Process.Start(AppFiles.MacosExtractUpdateProgram, arguments);
                //}

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