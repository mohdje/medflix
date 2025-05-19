
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;
using WebHostStreaming.Providers.AvailableVideosListProvider;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace WebHostStreaming.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        IAvailableVideosListProvider availableVideosListProvider;
        IWatchedMediaProvider watchedMediaProvider;

        public ApplicationController(IAvailableVideosListProvider availableVideosListProvider, IWatchedMediaProvider watchedMediaProvider)
        {
            this.availableVideosListProvider = availableVideosListProvider;
            this.watchedMediaProvider = watchedMediaProvider;
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
            else return NotFound();
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

            if (success.HasValue && success.Value)
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
        public IActionResult DeleteAvailableVideos([FromQuery] string videoFilePathsBase64)
        {
            if (string.IsNullOrEmpty(videoFilePathsBase64))
                return BadRequest();

            var videoFilePaths = HttpUtility.UrlDecode(videoFilePathsBase64.DecodeBase64()).Split("|");

            var counter = 0;

            foreach (var videoFilePath in videoFilePaths)
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


        [HttpDelete("clean")]
        public async Task<IActionResult> CleanUnusedResources()
        {
            var watchedMovies = await watchedMediaProvider.GetWatchedMoviesAsync();
            var watchedSeries = await watchedMediaProvider.GetWatchedSeriesAsync();
            var counter = 0;
            var videoExtensions = new string[] { ".mp4", ".avi", ".mkv" };

            foreach (var torrentFolder in Directory.GetDirectories(AppFolders.TorrentsFolder))
            {
                var shouldDeleteFolder = true;

                var videoFiles = Directory.GetFiles(torrentFolder, "*.*", SearchOption.AllDirectories).Where(f => videoExtensions.Contains(Path.GetExtension(f)));
             
                foreach (var videofilePath in videoFiles)
                {
                    var torrentUrlMd5Hash = Path.GetFileName(torrentFolder);

                    if (availableVideosListProvider.VideosSourcesList.Contains(videofilePath)
                        || (watchedMovies != null && watchedMovies.Any(m => m.VideoSource.ToMD5Hash() == torrentUrlMd5Hash))
                        || (watchedSeries != null && watchedSeries.Any(s => s.VideoSource.ToMD5Hash() == torrentUrlMd5Hash)))
                    {
                        shouldDeleteFolder = false;
                    }
                }

                if (shouldDeleteFolder)
                {
                    try
                    {
                        Directory.Delete(torrentFolder, true);
                        counter++;

                        AppLogger.LogInfo($"Deleted folder {torrentFolder}");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogInfo($"Error occured trying to delete folder {torrentFolder}: {ex.Message}");
                    }
                }
            }

            return Ok( new
            {
                foldersDeleted = counter
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