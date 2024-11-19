
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
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Medflix Pong");
        }

        [HttpGet("video")]
        public IActionResult GetVideoStream(string base64VideoPath)
        {
            var videoPath = base64VideoPath.DecodeBase64();

            if (System.IO.File.Exists(videoPath))
            {
                var stream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(stream, videoPath.GetContentType(), true);
            }
            else return NoContent();
        }
    }
}