using MoviesAPI.Services.Content.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebHostStreaming.Torrent
{
    public interface ITorrentAutoDownloader
    {
        void AddToDownloadList(LiteContentDto movieToDownload);
        void AddToDownloadList(IEnumerable<LiteContentDto> movieToDownload);
        void RemoveFromDownloadList(LiteContentDto movieToDownload);
        void StopDownload();
    }
}
