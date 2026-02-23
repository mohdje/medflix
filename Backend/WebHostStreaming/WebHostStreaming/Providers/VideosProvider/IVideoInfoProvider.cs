using System.Collections.Generic;
using WebHostStreaming.Models;
using static WebHostStreaming.Models.VideoInfo;

namespace WebHostStreaming.Providers
{
    public interface IVideoInfoProvider
    {
        void AddVideoInfo(VideoInfo videoInfo);
        bool RemoveVideoInfo(string videoInfoId);
        VideoInfo GetMovieVideoInfo(string mediaId, LanguageVersion language);
        VideoInfo GetEpisodeVideoInfo(string mediaId, LanguageVersion language, int seasonNumber, int episodeNumber);

        public IEnumerable<VideoInfo> AllVideosInfos { get; }
    }
}
