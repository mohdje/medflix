
using MedflixAPI.Services.Content.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Extensions;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Controllers
{
    [Route("videos")]
    [ApiController]
    public class VideoManagementController : ControllerBase
    {
        IVideoInfoProvider videoInfoProvider;
        ICacheProvider<ContentDto> mediaInfoCacheProvider;
        ISearchersProvider searchersProvider;

        public VideoManagementController(
            IVideoInfoProvider videoInfoProvider,
            ICacheProvider<ContentDto> mediaInfoCacheProvider,
            ISearchersProvider searchersProvider)
        {
            this.videoInfoProvider = videoInfoProvider;
            this.mediaInfoCacheProvider = mediaInfoCacheProvider;
            this.searchersProvider = searchersProvider;
        }

        [HttpGet("stream")]
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

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var multipartReader = HttpContext.Request.GetMultipartReader();

                VideoInfo videoInfo = await multipartReader.GetVideoInfoAsync();

                var fileSection = await multipartReader.ReadNextSectionAsync();

                if (fileSection?.GetContentDispositionHeader()?.Name.Value != "file")
                    throw new ArgumentException("File section is missing or has an invalid name");

                var filePath = await SaveFileAsync(fileSection.AsFileSection());

                videoInfo.FilePath = filePath;
                videoInfoProvider.AddVideoInfo(videoInfo);

                return CreatedAtAction(nameof(UploadFile), new { FilePath = filePath });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error while uploading video");
            }
        }

        [HttpGet("size")]
        public IActionResult GetFileSize(string fileName)
        {
            var filePath = Path.Combine(AppFolders.UploadFoler, fileName);

            if (System.IO.File.Exists(filePath))
                return Ok(new FileInfo(filePath).Length);
            else
                return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetVideos()
        {
            var tasks = videoInfoProvider.AllVideosInfos.DistinctBy(videoInfo => videoInfo.MediaId).Select(videoInfo => BuildMediaVideosDto(videoInfo.MediaId, videoInfo.IsSerie));
            var results = await Task.WhenAll(tasks);
            return Ok(results);
        }

        private async Task<MediaVideosDto> BuildMediaVideosDto(string mediaId, bool isSerie)
        {
            ContentDto mediaInfo = mediaInfoCacheProvider.GetFromCache(mediaId);
            if (mediaInfo == null)
            {
                if (isSerie)
                    mediaInfo = await searchersProvider.SeriesSearcher.GetSerieDetailsAsync(mediaId);
                else
                    mediaInfo = await searchersProvider.MovieSearcher.GetMovieDetailsAsync(mediaId);

                if (mediaInfo != null)
                    mediaInfoCacheProvider.AddToCache(mediaId, mediaInfo);
            }

            return new MediaVideosDto
            (
                mediaInfo?.Title,
                mediaInfo?.Year,
                mediaInfo?.CoverImageUrl,
                isSerie,
                [.. videoInfoProvider.AllVideosInfos.Where(v => v.MediaId == mediaId)]
            );
        }

        [HttpDelete]
        public IActionResult DeleteAvailableVideos([FromQuery] string[] videosIds)
        {
            if (videosIds == null || videosIds.Length == 0)
                return BadRequest();

            var counter = 0;

            foreach (var videoId in videosIds)
            {
                var videoInfo = videoInfoProvider.AllVideosInfos.FirstOrDefault(v => v.Id == videoId);
                if (videoInfo == null)
                    continue;

                var filePath = videoInfo.FilePath;
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                if (videoInfo != null)
                    mediaInfoCacheProvider.RemoveFromCache(videoInfo.MediaId);

                if (videoInfoProvider.RemoveVideoInfo(videoId))
                    counter++;
            }

            return Ok(new
            {
                Total = videosIds.Length,
                Success = counter
            });
        }

        private async Task<string> SaveFileAsync(FileMultipartSection fileSection)
        {
            if (fileSection == null || fileSection.FileStream == null)
                throw new ArgumentException($"File section is invalid");

            if (!Directory.Exists(AppFolders.UploadFoler))
                Directory.CreateDirectory(AppFolders.UploadFoler);

            var filePath = Path.Combine(AppFolders.UploadFoler, fileSection.FileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await fileSection.FileStream.CopyToAsync(fileStream);
            }

            if (!System.IO.File.Exists(filePath))
                throw new Exception($"Saving {fileSection?.FileName} failed");

            return filePath;
        }
    }
}