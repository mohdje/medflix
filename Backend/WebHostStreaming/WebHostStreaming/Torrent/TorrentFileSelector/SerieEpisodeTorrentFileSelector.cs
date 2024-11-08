using MonoTorrent;
using MonoTorrent.Client;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebHostStreaming.Torrent
{
    public class SerieEpisodeTorrentFileSelector : ITorrentFileSelector
    {
        protected readonly string episodeId;
        public SerieEpisodeTorrentFileSelector(int seasonNumber, int episodeNumber)
        {
            episodeId = BuildEpisodeId(seasonNumber, episodeNumber);
        }
        public virtual ITorrentManagerFile SelectTorrentFileInfo(IList<ITorrentManagerFile> torrentFileInfos)
        {
            return torrentFileInfos.FirstOrDefault(f => Path.GetFileName(f.FullPath).Contains(episodeId, System.StringComparison.OrdinalIgnoreCase));
        }

        protected string BuildEpisodeId(int seasonNumber, int episodeNumber)
        {
            var season = seasonNumber < 10 ? "0" + seasonNumber.ToString() : seasonNumber.ToString();
            var episode = episodeNumber < 10 ? "0" + episodeNumber.ToString() : episodeNumber.ToString();

            return $"S{season}E{episode}";
        }
    }
}
