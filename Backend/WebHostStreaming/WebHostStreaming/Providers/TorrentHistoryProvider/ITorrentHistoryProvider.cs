using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface ITorrentHistoryProvider
    {
        IEnumerable<TorrentInfoDto> GetTorrentFilesHistory();

        void SaveTorrentFileHistory(TorrentInfoDto torrentInfoDto);

    }
}
