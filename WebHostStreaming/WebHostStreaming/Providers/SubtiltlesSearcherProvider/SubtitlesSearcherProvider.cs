using MoviesAPI.Services;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.Subtitles;
using System.Collections.Generic;
using System.Linq;

namespace WebHostStreaming.Providers
{
    public class SubtitlesSearcherProvider : ISubtitlesSearcherProvider
    {
        private SubtitlesService selectedSubtitlesServiceType = SubtitlesService.YtsSubs;
        private SubtitlesSearcherFactory SubtitlesSearcherFactory = new SubtitlesSearcherFactory();
        public SubtitlesSearcher GetActiveSubtitlesSearcher()
        {
            return SubtitlesSearcherFactory.GetService(selectedSubtitlesServiceType);
        }

        public IEnumerable<ServiceInfo> GetSubtitlesServicesInfo()
        {
            var servicesInfo = SubtitlesSearcherFactory.GetServicesInfo(true);
            servicesInfo.SingleOrDefault(s => s.Id == (int)selectedSubtitlesServiceType).Selected = true;
            return servicesInfo;
        }

        public void UpdateSelectedSubtitlesSearcher(int selectedSubtitleServiceId)
        {
            selectedSubtitlesServiceType = (SubtitlesService)selectedSubtitleServiceId;
        }
    }
}
