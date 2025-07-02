using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface IWatchedSeriesProvider
    {
        void SaveWatchedEpisode(WatchedMediaDto movieToSave);
        IEnumerable<WatchedMediaDto> GetWatchedSeries();
        WatchedMediaDto GetWatchedEpisode(int serieId, int seasonNumber, int episodeNumber);
        IEnumerable<WatchedMediaDto> GetWatchedEpisodes(int serieId, int seasonNumber);

    }
}
