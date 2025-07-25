using System;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public interface ITorrentContentProvider
    {
        Task<TorrentStream> GetTorrentStreamAsync(TorrentRequest torrentRequest);
        Task<bool> DownloadTorrentMediaAsync(TorrentRequest torrentRequest, CancellationToken cancellationToken);
        Task<DownloadingState> GetDownloadingStateAsync(string clientAppIdentifier, string torrentUrl);
        event EventHandler OnNoActiveTorrentClient;
    }            
}
