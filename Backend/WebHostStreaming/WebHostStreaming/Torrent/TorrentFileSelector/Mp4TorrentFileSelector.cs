using MonoTorrent.Client;
using System.Collections.Generic;
using System.Linq;

namespace WebHostStreaming.Torrent
{
    public class Mp4TorrentFileSelector : ITorrentFileSelector
    {
        public ITorrentFileInfo SelectTorrentFileInfo(IList<ITorrentFileInfo> torrentFileInfos)
        {
            return torrentFileInfos.FirstOrDefault(f => f.FullPath.EndsWith(".mp4"));
        }
    }
}
