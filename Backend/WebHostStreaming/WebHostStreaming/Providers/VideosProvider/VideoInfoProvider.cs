using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class VideoInfoProvider : DataStoreProvider<VideoInfo>, IVideoInfoProvider
    {
        protected override int MaxLimit => int.MaxValue;

        protected override string FilePath => AppFiles.VideosInfos;

        public IEnumerable<VideoInfo> AllVideosInfos => Data;

        public void AddVideoInfo(VideoInfo videoInfo)
        {
            lock (Data)
            {
                if (Data.Any(v => v.Id == videoInfo.Id))
                    RemoveVideoInfo(videoInfo.Id);

                AddData(videoInfo);
            }
        }

        public bool RemoveVideoInfo(string videoInfoId)
        {
            var videoInfoToRemove = Data.FirstOrDefault(v => v.Id == videoInfoId);

            if (videoInfoToRemove != null)
            {
                RemoveData(videoInfoToRemove);
                return true;
            }
            else
            {
                AppLogger.LogInfo($"Video {videoInfoId} not found in the list.");
                return false;
            }
        }

        public VideoInfo GetVideoInfo(string mediaId, LanguageVersion language, int seasonNumber = 0, int episodeNumber = 0)
        {
            return Data.FirstOrDefault(v =>
                v.MediaId == mediaId &&
                v.Language == language &&
                v.SeasonNumber == seasonNumber &&
                v.EpisodeNumber == episodeNumber);
        }
    }
}
