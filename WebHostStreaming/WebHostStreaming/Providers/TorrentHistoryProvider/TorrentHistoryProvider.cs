using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class TorrentHistoryProvider : DataProvider, ITorrentHistoryProvider
    {
        private readonly string TorrentHistoryFile = AppFiles.TorrentHistory;

        protected override int MaxLimit()
        {
            return 30;
        }
        public async Task<IEnumerable<TorrentInfoDto>> GetTorrentFilesHistoryAsync()
        {
            return await GetDataAsync<TorrentInfoDto>(TorrentHistoryFile);
        }

        public async Task SaveTorrentFileHistoryAsync(TorrentInfoDto torrentInfoDto)
        {
            await SaveDataAsync(TorrentHistoryFile, torrentInfoDto, null);
        }

        
    }
}
