using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers.AvailableVideosListProvider
{
    public interface IAvailableVideosListProvider
    {
        string[] VideosSourcesList { get; }
        Task<bool?> AddMediaSource(string videoFilePath);
        bool RemoveMediaSource(string videoFilePath);
        string GetVoMovieSource(string movieName, int year);
        string GetVoSerieSource(string serieName, int seasonNumber, int episodeNumber);
        string GetVfMovieSource(string orignalMovieName, string frenchMovieName, int year);
        string GetVfSerieSource(string originalSerieName, string frenchSerieName, int seasonNumber, int episodeNumber);
    }
}
