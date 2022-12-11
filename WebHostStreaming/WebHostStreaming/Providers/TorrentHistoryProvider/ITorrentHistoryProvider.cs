using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface ITorrentHistoryProvider
    {
        Task<IEnumerable<TorrentInfoDto>> GetTorrentFilesHistoryAsync();

        Task SaveTorrentFileHistoryAsync(TorrentInfoDto torrentInfoDto);

    }
}
