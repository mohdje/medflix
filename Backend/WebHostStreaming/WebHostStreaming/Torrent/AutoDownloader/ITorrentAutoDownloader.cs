using System.Threading.Tasks;

namespace WebHostStreaming.Torrent
{
    public interface ITorrentAutoDownloader
    {
        Task StartAsync();
        void RetryLater();
    }
}
