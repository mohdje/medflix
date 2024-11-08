using MonoTorrent;
using MonoTorrent.Client;
using System.Collections.Generic;

namespace WebHostStreaming.Torrent
{
    public interface ITorrentFileSelector
    {
        ITorrentManagerFile SelectTorrentFileInfo(IList<ITorrentManagerFile> torrentFileInfos);
    }
}
