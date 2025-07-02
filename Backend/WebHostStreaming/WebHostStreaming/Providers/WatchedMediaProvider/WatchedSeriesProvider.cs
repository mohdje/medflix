using System.Collections.Generic;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using System.Linq;

namespace WebHostStreaming.Providers
{
    public class WatchedSeriesProvider : DataStoreProvider<WatchedMediaDto>, IWatchedSeriesProvider
    {
        protected override int MaxLimit => 1000;

        protected override string FilePath => AppFiles.WatchedSeries;

        public WatchedMediaDto GetWatchedEpisode(int serieId, int seasonNumber, int episodeNumber)
        {
            return Data.FirstOrDefault(wm => wm.Media.Id == serieId.ToString() && wm.SeasonNumber == seasonNumber && wm.EpisodeNumber == episodeNumber);       
        }

        public IEnumerable<WatchedMediaDto> GetWatchedEpisodes(int serieId, int seasonNumber)
        {
            return Data.Where(wm => wm.Media.Id == serieId.ToString() && wm.SeasonNumber == seasonNumber);
        }

        public IEnumerable<WatchedMediaDto> GetWatchedSeries()
        {
            return Data.Reverse().DistinctBy(watchedMedia => watchedMedia.Media.Id).Take(30);
        }

        public void SaveWatchedEpisode(WatchedMediaDto watchedEpisode)
        {
            if(Data.Any(wm => wm.Media.Id == watchedEpisode.Media.Id && wm.SeasonNumber == watchedEpisode.SeasonNumber && wm.EpisodeNumber == watchedEpisode.EpisodeNumber))
                UpdateData(watchedEpisode);
            else
                AddData(watchedEpisode);
        }
    }
}
