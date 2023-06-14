using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public interface IAppUpdater
    {
        Task<bool> IsNewReleaseAvailableAsync();

        Task<bool> DownloadNewReleaseAsync(string filePath);
    }
}
