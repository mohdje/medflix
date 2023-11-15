using Medflix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Tools
{
    internal static class TestData
    {
        public static VideoPlayerOptions GetVideoPlayerOptions()
        {
            //"http://localhost:5000/torrent/stream/movies?base64Url=aaaa"
            var options = new VideoPlayerOptions();
            options.Sources = new VideoOption[]
            {
            new VideoOption{ Quality = "1080p", Url = @"D:\Projets\Streaming\R&D\data\myvideo.mkv", Selected = true },
            new VideoOption{ Quality = "720p", Url = @"https://download.samplelib.com/mp4/sample-15s.mp4" },
            new VideoOption{ Quality = "1080p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" },
            new VideoOption{ Quality = "720p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" },
            new VideoOption{ Quality = "1080p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" }
            };
            options.ResumeToTime = 0;
            options.Subtitles = new SubtitleOption[]
            {
            new SubtitleOption(){ Language = "English", SubtitlesSourceUrls = new string[] { "0", "0", "0", "0", "0", "0" } },
            new SubtitleOption(){ Language = "French", SubtitlesSourceUrls = new string[] { "2", "3" } },
            };
            options.WatchedMedia = new WebHostStreaming.Models.WatchedMediaDto();
            options.WatchedMedia.SeasonNumber = 1;
            options.WatchedMedia.EpisodeNumber = 1;
            options.WatchedMedia.CoverImageUrl = "cover";
            options.WatchedMedia.Id = "252525";
            options.WatchedMedia.Rating = 7.5f;
            options.WatchedMedia.Synopsis = "this is a movie...";
            options.WatchedMedia.Title = "Breaking Bad";
            options.WatchedMedia.TotalDuration = 3600;
            options.WatchedMedia.Year = 2022;
            options.MediaType = "series";

            return options;
        }
    }
}
