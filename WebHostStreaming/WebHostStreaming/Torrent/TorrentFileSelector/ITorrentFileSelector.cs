using MonoTorrent.Client;
using System.Collections.Generic;

namespace WebHostStreaming.Torrent
{
    public interface ITorrentFileSelector
    {
        ITorrentFileInfo SelectTorrentFileInfo(IList<ITorrentFileInfo> torrentFileInfos);
    }
}
