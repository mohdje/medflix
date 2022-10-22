using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.Subtitles;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebHostStreaming.Providers
{
    public interface ISubtitlesSearcherProvider
    {
        SubtitlesSearcher ActiveSubtitlesSearcher { get; }
        Task<IEnumerable<ServiceInfo>> GetSubtitlesServicesInfo();
        void UpdateSelectedSubtitlesSearcher(int selectedSubtitleServiceId);
    }
}
