
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Providers.AvailableVideosListProvider;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private IAvailableVideosListProvider availableVideosListProvider;

        public ApplicationController(IAvailableVideosListProvider availableVideosListProvider)
        {
            this.availableVideosListProvider = availableVideosListProvider;
        }
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

        [HttpPost("upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadFile()
        {
            if (!HttpContext.Request.HasFormContentType
         || !HttpContext.Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                return BadRequest("no multipart/form-data header");

            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(HttpContext.Request.ContentType).Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
                return BadRequest("Missing content-type boundary.");

            var multipartReader = new MultipartReader(boundary, HttpContext.Request.Body);
            var section = await multipartReader.ReadNextSectionAsync();

            if (section == null)
                return BadRequest("No section found");

            var fileSection = section.AsFileSection();
            if (fileSection == null)
                return BadRequest("No File section found");

            var filePath = await SaveFileAsync(fileSection);
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return StatusCode(500);

            var success = await availableVideosListProvider.AddMediaSource(filePath);

            if(success)
            {
                return CreatedAtAction(nameof(UploadFile), new
                {
                    FilePath = filePath
                });
            }
            else
                return StatusCode(500);
        }

        [HttpGet("upload")]
        public IActionResult GetUploadFileSize(string fileName)
        {
            var filePath = Path.Combine(AppFolders.UploadFoler, fileName);

            if (System.IO.File.Exists(filePath))
                return Ok(new FileInfo(filePath).Length);
            else
                return NoContent();
        }

        [HttpGet("availablevideos")]
        public string[] GetAvailableVideos()
        {
            return availableVideosListProvider.VideosSourcesList;
        }

        [HttpDelete("availablevideos")]
        public async Task<IActionResult> DeleteAvailableVideos()
        {
            string body = null;
            using (StreamReader sr = new StreamReader(HttpContext.Request.Body))
            {
                body = await sr.ReadToEndAsync();
            }

            if (string.IsNullOrEmpty(body))
                return BadRequest();

            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(HttpContext.Request.ContentType).Boundary).Value;

            body = body.Replace(boundary, string.Empty);
            body = body.Replace("-", string.Empty);

            var parameterName = "\"videoFilePaths\"";
            var index = body.IndexOf(parameterName);

            if (index == -1)
                return BadRequest();

            var list = body.Substring(index + parameterName.Length).Trim();

            var videoFilePaths = list.Split("?");

            var counter = 0;

            foreach(var videoFilePath in videoFilePaths)
            {
                if (availableVideosListProvider.RemoveMediaSource(videoFilePath))
                    counter++;
            }

            return Ok(new
            {
                Total = videoFilePaths.Length,
                Success = counter
            });
        }


        private async Task<string> SaveFileAsync(FileMultipartSection fileSection)
        {
            if (!Directory.Exists(AppFolders.UploadFoler))
                Directory.CreateDirectory(AppFolders.UploadFoler);

            var filePath = Path.Combine(AppFolders.UploadFoler, fileSection?.FileName);

            if (availableVideosListProvider.VideosSourcesList.Contains(filePath))
                return null;

            try
            {
                await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024);

                await fileSection.FileStream?.CopyToAsync(stream);

                return filePath;
            }
            catch
            {
                return null;
            }
        }
    }
}