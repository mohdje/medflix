using MonoTorrent;
using MonoTorrent.Client;
using System.Collections.Generic;
using System.Linq;

namespace WebHostStreaming.Torrent
{
    public class VideoTorrentFileSelector : ITorrentFileSelector
    {
        public ITorrentManagerFile SelectTorrentFileInfo(IList<ITorrentManagerFile> torrentFileInfos)
        {
            return torrentFileInfos.FirstOrDefault(f => f.FullPath.EndsWith(".mp4") || f.FullPath.EndsWith(".avi") || f.FullPath.EndsWith(".mkv"));
        }
    }
}
