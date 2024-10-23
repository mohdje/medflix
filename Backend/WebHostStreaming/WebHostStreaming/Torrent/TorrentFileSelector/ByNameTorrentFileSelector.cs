using MonoTorrent.Client;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebHostStreaming.Torrent
{
    public class ByNameTorrentFileSelector : ITorrentFileSelector
    {
        readonly string fileNameToSelect;
        public ByNameTorrentFileSelector(string fileNameToSelect)
        {
            this.fileNameToSelect = fileNameToSelect;
        }
        public ITorrentFileInfo SelectTorrentFileInfo(IList<ITorrentFileInfo> torrentFileInfos)
        {
            return torrentFileInfos.SingleOrDefault(f => Path.GetFileName(f.FullPath) == fileNameToSelect);
        }
    }
}
