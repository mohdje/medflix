using MoviesAPI.Services.Content.Dtos;
using System.Threading.Tasks;

namespace WebHostStreaming.Torrent
{
    public interface ITorrentAutoDownloader
    {
        void AddToDownloadList(LiteContentDto movieToDownload);
        void RemoveFromDownloadList(LiteContentDto movieToDownload);
        void StopDownload();
    }
}
