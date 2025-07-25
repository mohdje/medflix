using System.Collections.Generic;
using WebHostStreaming.Models;
using static WebHostStreaming.Models.VideoInfo;

namespace WebHostStreaming.Providers
{
    public interface IVideoInfoProvider
    {
        void AddVideoInfo(VideoInfo videoInfo);
        bool RemoveVideoInfo(string videoInfoId);
        VideoInfo GetVideoInfo(string mediaId, LanguageVersion language, int seasonNumber = 0, int episodeNumber = 0);
        public IEnumerable<VideoInfo> AllVideosInfos { get; }
    }
}
