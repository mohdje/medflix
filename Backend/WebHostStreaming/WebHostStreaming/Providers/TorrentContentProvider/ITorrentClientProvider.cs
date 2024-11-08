using Microsoft.AspNetCore.Http;
using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.Providers
{
    public interface ITorrentClientProvider
    {
        Task<Stream> GetTorrentStreamAsync(string torrentUri, ITorrentFileSelector torrentFileSelector);
        DownloadingState GetStreamDownloadingState(string torrentUri);
        string GetContentType(string torrentUri);
        Task<IEnumerable<string>> GetTorrentFilesAsync(string torrentUri);
    }
}
