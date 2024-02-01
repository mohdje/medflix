using Medflix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedflixWinForms.Utils
{
    internal static class TestData
    {
        public static VideoPlayerOptions GetVideoPlayerOptions()
        {
            //"http://localhost:5000/torrent/stream/movies?base64Url=aaaa"
            var options = new VideoPlayerOptions();
            options.Sources = new VideoOption[]
            {
            new VideoOption{ Quality = "1080p", Url = @"D:\Projets\Streaming\R&D\data\myvideo.mkv" },
            new VideoOption{ Quality = "720p", Url = @"D:\Projets\Streaming\R&D\data\paradise_canyon.mp4"  },
            new VideoOption{ Quality = "Test", Url = @"http://localhost:5000/torrent/stream/movies?base64Url=aHR0cHM6Ly9kdWNrZHVja2dvLmNvbS8/cT10b2Jhc2U2NCZhdGI9djM2Mi0xJmlhPXdlYg==", Selected = true  },
            new VideoOption{ Quality = "1080p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" },
            new VideoOption{ Quality = "720p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" },
            new VideoOption{ Quality = "1080p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" }
            };
            options.ResumeToTime = 0;
            options.Subtitles = new SubtitleOption[]
            {
            new SubtitleOption(){ Language = "English", SubtitlesSourceUrls = new string[] { "https://yts-subs.com/subtitles/dungeons-dragons-honor-among-thieves-2023-english-yify-494150", "https://dl.opensubtitles.org/en/download/sub/9532843", "0", "1", "2", "3" } },
            new SubtitleOption(){ Language = "French", SubtitlesSourceUrls = new string[] { "https://dl.opensubtitles.org/en/download/sub/9550679", "https://yts-subs.com/subtitles/dungeons-dragons-honor-among-thieves-2023-french-yify-500070" } },
            };
            options.WatchedMedia = new WebHostStreaming.Models.WatchedMediaDto();
            options.WatchedMedia.SeasonNumber = 1;
            options.WatchedMedia.EpisodeNumber = 1;
            options.WatchedMedia.CoverImageUrl = "https://image.tmdb.org/t/p/w300_and_h450_bestv2/bkpPTZUdq31UGDovmszsg2CchiI.jpg";
            options.WatchedMedia.Id = "252525";
            options.WatchedMedia.Rating = 7.5f;
            options.WatchedMedia.Synopsis = "When Branch's brother, Floyd, is kidnapped for his musical talents by a pair of nefarious pop-star villains, Branch and Poppy embark on a harrowing and emotional journey to reunite the other brothers and rescue Floyd from a fate even worse than pop-culture obscurity.";
            options.WatchedMedia.Title = "Trolls Band Together";
            options.WatchedMedia.TotalDuration = 3202.233f;
            options.WatchedMedia.Year = 2022;
            options.MediaType = "movies";//series

            return options;
        }
    }
}
