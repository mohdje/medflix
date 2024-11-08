using MonoTorrent;
using MonoTorrent.Client;
using System.Collections.Generic;
using System.Linq;

namespace WebHostStreaming.Torrent
{
    public class SerieEpisodeMp4TorrentFileSelector : SerieEpisodeTorrentFileSelector
    {
        public SerieEpisodeMp4TorrentFileSelector(int seasonNumber, int episodeNumber) : base(seasonNumber, episodeNumber)
        {
        }

        public override ITorrentManagerFile SelectTorrentFileInfo(IList<ITorrentManagerFile> torrentFileInfos)
        {
            return base.SelectTorrentFileInfo(torrentFileInfos.Where(f => f.FullPath.EndsWith(".mp4")).ToList());
        }
    }
}
