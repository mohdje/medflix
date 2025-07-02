using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class TorrentHistoryProvider : DataStoreProvider<TorrentInfoDto>, ITorrentHistoryProvider
    {
        protected override int MaxLimit => 30;

        protected override string FilePath => AppFiles.TorrentHistory;

        public IEnumerable<TorrentInfoDto> GetTorrentFilesHistory()
        {
            return Data.OrderByDescending(f => f.LastOpenedDateTime).Take(MaxLimit);
        }

        public void SaveTorrentFileHistory(TorrentInfoDto torrentInfoDto)
        {
            AddData(torrentInfoDto);
        }
    }
}
