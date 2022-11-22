using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public interface ITorrentVideoStreamProvider
    {
        Task<StreamDto> GetStreamAsync(string torrentUri, int offset, string videoExtension);
        DownloadingState GetStreamDownloadingState(string torrentUri);
    }
}
