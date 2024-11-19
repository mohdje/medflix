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
        public async Task AddMediaSource(string videoFilePath)
        {
            if (videoSourcesList == null)
                await LoadListAsync();

            if (videoSourcesList != null && videoSourcesList.Contains(videoFilePath))
                return;

            if (!Directory.Exists(AppFolders.DataFolder))
                Directory.CreateDirectory(AppFolders.DataFolder);

            try
            {
                await Locker.WaitAsync();
                File.AppendAllLines(AppFiles.AvailableMediaSources, new string[] { videoFilePath });
              
                videoSourcesList.Add(videoFilePath);
            }
            catch
            {

            }
            finally
            {
                Locker.Release();
            }
        }

        public async Task<string> GetVoMovieSource(string movieName, int year)
        {
            return await GetMovieSource(movieName, string.Empty, year, false);
        }

        public async Task<string> GetVfMovieSource(string originalMovieName, string frenchMovieName, int year)
        {
            return await GetMovieSource(originalMovieName, frenchMovieName, year, true);
        }

        private async Task<string> GetMovieSource(string originalMovieName, string frenchMovieName, int year, bool frenchVersion)
        {
            if (videoSourcesList == null)
                await LoadListAsync();

            if (videoSourcesList == null || !videoSourcesList.Any())
                return null;

            var mediaSource = videoSourcesList.FirstOrDefault
            (videoPath =>
                {
                    var fileName = Path.GetFileName(videoPath);

                    bool isCorrectVersion = false;

                    if (frenchVersion)
                        isCorrectVersion = FrenchTags.Any(tag => fileName.Contains(tag, StringComparison.OrdinalIgnoreCase));
                    else
                        isCorrectVersion = !FrenchTags.Any(tag => fileName.Contains(tag, StringComparison.OrdinalIgnoreCase));

                    return
                    (fileName.CustomStartsWith(originalMovieName) || (frenchVersion && fileName.CustomStartsWith(frenchMovieName)))
                     && isCorrectVersion
                     && fileName.Contains(year.ToString());
                }
            );

            return mediaSource;
        }
        public async Task<string> GetVoSerieSource(string serieName, int seasonNumber, int episodeNumber)
        {
            return await GetSerieSource(serieName, string.Empty, seasonNumber, episodeNumber, false);
        }
        public async Task<string> GetVfSerieSource(string originalSerieName, string frenchSerieName, int seasonNumber, int episodeNumber)
        {
            return await GetSerieSource(originalSerieName, frenchSerieName,seasonNumber, episodeNumber, true);
        }

        private async Task<string> GetSerieSource(string originalSerieName, string frenchSerieName, int seasonNumber, int episodeNumber, bool frenchVersion)
        {
            if (videoSourcesList == null)
                await LoadListAsync();

            if (videoSourcesList == null || !videoSourcesList.Any())
                return null;

            var seasonId = $"S{seasonNumber.ToString("D2")}";
            var episodeId = $"E{episodeNumber.ToString("D2")}";

            var mediaSource = videoSourcesList.FirstOrDefault
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
                    (fileName.CustomStartsWith(originalSerieName) || (frenchVersion && fileName.CustomStartsWith(frenchSerieName)))
                    && isCorrectVersion
                    && isCorrectEpisode;
                }
            );

            return mediaSource;
        }

        private async Task LoadListAsync()
        {
            await Locker.WaitAsync();

            try
            {
                var lines = File.ReadAllLines(AppFiles.AvailableMediaSources);
                videoSourcesList = lines.ToList();
            }
            catch (Exception ex)
            {
                AppLogger.LogInfo($"Error trying to read {AppFiles.AvailableMediaSources}");
            }
            finally
            {
                videoSourcesList = videoSourcesList ?? new List<string>();

                Locker.Release();
            }
        }
    }
}
