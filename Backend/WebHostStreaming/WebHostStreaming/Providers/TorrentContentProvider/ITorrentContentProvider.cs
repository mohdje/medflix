using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public interface ITorrentContentProvider
    {
        Task<StreamDto> GetStreamAsync(string torrentUri, int offset, ITorrentFileSelector torrentFileSelector);
        DownloadingState GetStreamDownloadingState(string torrentUri);
        Task<IEnumerable<string>> GetTorrentFilesAsync(string torrentUri);
    }
}
