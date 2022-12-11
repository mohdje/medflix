using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class TorrentHistoryProvider : DataProvider, ITorrentHistoryProvider
    {
        protected override string FilePath()
        {
            return AppFiles.TorrentHistory; 
        }

        protected override int MaxLimit()
        {
            return 30;
        }
        public async Task<IEnumerable<TorrentInfoDto>> GetTorrentFilesHistoryAsync()
        {
            return await GetDataAsync<TorrentInfoDto>();
        }

        public async Task SaveTorrentFileHistoryAsync(TorrentInfoDto torrentInfoDto)
        {
            await SaveDataAsync(torrentInfoDto, null);
        }

        
    }
}
