using MonoTorrent;
using MoviesAPI.Extensions;
using WebHostStreaming.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers.AvailableVideosListProvider
{
    public class AvailableVideosListProvider : IAvailableVideosListProvider
    {
        private string[] FrenchTags = { "FRENCH", "TRUEFRENCH", "Vf", "Multi" };

        SemaphoreSlim Locker = new SemaphoreSlim(1, 1);
        List<string> videoSourcesList;
        public string[] VideosSourcesList 
        { 
            get
            {
                if (videoSourcesList == null)
                    videoSourcesList = LoadList().ToList();

                return videoSourcesList.ToArray();
            } 
        }
        public async Task<bool> AddMediaSource(string videoFilePath)
        {
            if (VideosSourcesList.Contains(videoFilePath))
                return false;

            if (!Directory.Exists(AppFolders.DataFolder))
                Directory.CreateDirectory(AppFolders.DataFolder);

            try
            {
                await Locker.WaitAsync();
                File.AppendAllLines(AppFiles.AvailableMediaSources, new string[] { videoFilePath });
              
                videoSourcesList.Add(videoFilePath);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                Locker.Release();
            }
        }

        public string GetVoMovieSource(string movieName, int year)
        {
            return GetMovieSource(movieName, string.Empty, year, false);
        }

        public string GetVfMovieSource(string originalMovieName, string frenchMovieName, int year)
        {
            return GetMovieSource(originalMovieName, frenchMovieName, year, true);
        }

        private string GetMovieSource(string originalMovieName, string frenchMovieName, int year, bool frenchVersion)
        {
            if (!VideosSourcesList.Any())
                return null;

            var mediaSource = VideosSourcesList.FirstOrDefault
            (videoPath =>
                {
                    var fileName = Path.GetFileName(videoPath);

                    bool isCorrectVersion = false;

                    if (frenchVersion)
                        isCorrectVersion = FrenchTags.Any(tag => fileName.Contains(tag, StringComparison.OrdinalIgnoreCase));
                    else
                        isCorrectVersion = !FrenchTags.Any(tag => fileName.Contains(tag, StringComparison.OrdinalIgnoreCase));

                    return
                    (fileName.CustomContains(originalMovieName) || (frenchVersion && fileName.CustomContains(frenchMovieName)))
                     && isCorrectVersion
                     && fileName.Contains(year.ToString());
                }
            );

            return mediaSource;
        }
        public string GetVoSerieSource(string serieName, int seasonNumber, int episodeNumber)
        {
            return GetSerieSource(serieName, string.Empty, seasonNumber, episodeNumber, false);
        }
        public string GetVfSerieSource(string originalSerieName, string frenchSerieName, int seasonNumber, int episodeNumber)
        {
            return GetSerieSource(originalSerieName, frenchSerieName,seasonNumber, episodeNumber, true);
        }

        private string GetSerieSource(string originalSerieName, string frenchSerieName, int seasonNumber, int episodeNumber, bool frenchVersion)
        {
            if (!VideosSourcesList.Any())
                return null;

            var seasonId = $"S{seasonNumber.ToString("D2")}";
            var episodeId = $"E{episodeNumber.ToString("D2")}";

            var mediaSource = VideosSourcesList.FirstOrDefault
            (videoPath =>
                {
                    var fileName = Path.GetFileName(videoPath);

                    bool isCorrectVersion = false;

                    if (frenchVersion)
                        isCorrectVersion = FrenchTags.Any(tag => fileName.Contains(tag, StringComparison.OrdinalIgnoreCase));
                    else
                        isCorrectVersion = !FrenchTags.Any(tag => fileName.Contains(tag, StringComparison.OrdinalIgnoreCase));

                    var isCorrectEpisode = fileName.Contains(seasonId, StringComparison.OrdinalIgnoreCase) && fileName.Contains(episodeId, StringComparison.OrdinalIgnoreCase);

                    return 
                    (fileName.CustomContains(originalSerieName) || (frenchVersion && fileName.CustomContains(frenchSerieName)))
                    && isCorrectVersion
                    && isCorrectEpisode;
                }
            );

            return mediaSource;
        }

        private string[] LoadList()
        {
            Locker.Wait();

            try
            {
                var lines = File.ReadAllLines(AppFiles.AvailableMediaSources);
                return lines;
            }
            catch (Exception ex)
            {
                AppLogger.LogInfo($"Error trying to read {AppFiles.AvailableMediaSources}");
            }
            finally
            {
                Locker.Release();
            }
            return Array.Empty<string>();
        }

        public bool RemoveMediaSource(string videoFilePath)
        {
            AppLogger.LogInfo($"RemoveMediaSource called for : {videoFilePath}");

            if (!File.Exists(videoFilePath) && !VideosSourcesList.Contains(videoFilePath))
            {
                AppLogger.LogInfo($"{videoFilePath} does not exist on disk and is not in the list");
                return false;
            }

            try
            {
                if (System.IO.File.Exists(videoFilePath))
                {
                    AppLogger.LogInfo($"RemoveMediaSource try to delete {videoFilePath}");
                    File.Delete(videoFilePath);
                    AppLogger.LogInfo($"RemoveMediaSource successfully deleted {videoFilePath}");
                }

                if (VideosSourcesList.Contains(videoFilePath))
                {
                    AppLogger.LogInfo($"Remove from list {videoFilePath}");
                    videoSourcesList.Remove(videoFilePath);
                }

                Locker.Wait();

                File.WriteAllLines(AppFiles.AvailableMediaSources, videoSourcesList);

                AppLogger.LogInfo($"RemoveMediaSource success for {videoFilePath}");

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.LogInfo($"RemoveMediaSource Error: {ex.Message}");
                return false;
            }
            finally
            {
                Locker.Release();
            }
        }
    }
}
