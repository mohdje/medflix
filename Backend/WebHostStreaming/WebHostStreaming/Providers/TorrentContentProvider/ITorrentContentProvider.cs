using System;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public interface ITorrentContentProvider
    {
        Task<TorrentStream> GetTorrentStreamAsync(string torrentUri, string clientAppIdentifier, ITorrentFileSelector torrentFileSelector);
        Task<bool> DownloadTorrentMediaAsync(string torrentUri, string clientAppIdentifier, ITorrentFileSelector torrentFileSelector, CancellationToken cancellationToken);
        Task<DownloadingState> GetDownloadingStateAsync(string clientAppIdentifier, string torrentUrl);
        event EventHandler OnNoActiveTorrentClient;
    }            
}
