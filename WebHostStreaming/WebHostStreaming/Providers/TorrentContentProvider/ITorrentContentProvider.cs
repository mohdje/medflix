using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface ITorrentContentProvider
    {
        Task<StreamDto> GetStreamAsync(string torrentUri, int offset, Func<string, bool> torrentFileSelector);
        DownloadingState GetStreamDownloadingState(string torrentUri);
        Task<IEnumerable<string>> GetTorrentFilesAsync(string torrentUri);
    }
}
