using MoviesAPI.Services;
using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.VFMovies;
using MoviesAPI.Services.VFMovies.VFMoviesSearchers;
using MoviesAPI.Services.VOMovies;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class SearchersProvider : ISearchersProvider
    {
        public VOMovieSearcher ActiveVOMovieSearcher => VOMovieSearcherFactory.GetService(selectedVOMovieServiceType);
        public VFMoviesSearcher ActiveVFMovieSearcher => VFMovieSearcherFactory.GetService(selectedVFMovieServiceType);
        public SubtitlesSearcher ActiveSubtitlesSearcher => SubtitlesSearcherFactory.GetService(selectedSubtitlesServiceType);

        private VOMovieService selectedVOMovieServiceType = VOMovieService.YtsHtmlOne;
        private VOMovieSearcherFactory VOMovieSearcherFactory = new VOMovieSearcherFactory();

        private VFMoviesService selectedVFMovieServiceType = VFMoviesService.ZeTorrents;
        private VFMovieSearcherFactory VFMovieSearcherFactory = new VFMovieSearcherFactory();

        private SubtitlesService selectedSubtitlesServiceType = SubtitlesService.YtsSubs;
        private SubtitlesSearcherFactory SubtitlesSearcherFactory = new SubtitlesSearcherFactory();

        public SearchersProvider()
        {
            LoadSourcesSettings();

            SubtitlesSearcher.SetExtractionFolderLocation(AppFolders.SubtitlesFolder);
        }

        private void LoadSourcesSettings()
        {
            if (!System.IO.File.Exists(AppFiles.SourcesSettings))
                return;

            var sourcesSettings = JsonHelper.DeserializeFromFile<SourcesSettings>(AppFiles.SourcesSettings);

            if (sourcesSettings != null)
            {
                selectedSubtitlesServiceType = sourcesSettings.SubtitlesService;
                selectedVFMovieServiceType = sourcesSettings.VFMoviesService;
                selectedVOMovieServiceType = sourcesSettings.VOMovieService;
            }
        }

        
        public async Task<ServiceInfo> GetSelectedVOMoviesServiceInfoAsync(bool includeAvailabiltyState)
        {
            return await VOMovieSearcherFactory.GetServiceInfoAsync(selectedVOMovieServiceType, includeAvailabiltyState);
        }

        public async Task<IEnumerable<ServiceInfo>> GetSubtitlesServicesInfo()
        {
            var servicesInfo = await SubtitlesSearcherFactory.GetServicesInfoAsync(true);
            servicesInfo.SingleOrDefault(s => s.Id == (int)selectedSubtitlesServiceType).Selected = true;
            return servicesInfo;
        }

        public async Task<IEnumerable<ServiceInfo>> GetVFMoviesServicesInfo()
        {
            var servicesInfo = await VFMovieSearcherFactory.GetServicesInfoAsync(true);
            servicesInfo.SingleOrDefault(s => s.Id == (int)selectedVFMovieServiceType).Selected = true;
            return servicesInfo;
        }

        public async Task<IEnumerable<ServiceInfo>> GetVOMoviesServicesInfo()
        {
            var servicesInfo = await VOMovieSearcherFactory.GetServicesInfoAsync(true);
            servicesInfo.SingleOrDefault(s => s.Id == (int)selectedVOMovieServiceType).Selected = true;
            return servicesInfo;
        }

        public void UpdateSelectedSubtitlesSearcher(int selectedSubtitleServiceId)
        {
            selectedSubtitlesServiceType = (SubtitlesService)selectedSubtitleServiceId;
        }

        public void UpdateSelectedVFMovieSearcher(int selectedMovieServiceId)
        {
            selectedVFMovieServiceType = (VFMoviesService)selectedMovieServiceId;
        }

        public void UpdateSelectedVOMovieSearcher(int selectedMovieServiceId)
        {
            selectedVOMovieServiceType = (VOMovieService)selectedMovieServiceId;
        }

        public void SaveSources()
        {
            JsonHelper.SerializeToFileAsync(AppFiles.SourcesSettings, new SourcesSettings()
            {
                SubtitlesService = selectedSubtitlesServiceType,
                VFMoviesService = selectedVFMovieServiceType,
                VOMovieService = selectedVOMovieServiceType
            });
        }
    }
}
