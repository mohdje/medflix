using MoviesAPI.Helpers;
using MoviesAPI.Services.Tmdb;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Subtitles.Searchers;
using MoviesAPI.Services.Torrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Services.Content;

namespace MoviesAPI.Services
{
    public class MoviesAPIFactory
    {
        private static MoviesAPIFactory _instance;

        public static MoviesAPIFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MoviesAPIFactory();

                return _instance;
            }
        }

        private string subtitlesFolder;

        private MoviesAPIFactory()
        {

        }

       
        public IMovieSearcher CreateMovieSearcher(string apiKey)
        {
            return new TmdbMovieClient(apiKey);
        }

        public ISeriesSearcher CreateSeriesSearcher(string apiKey)
        {
            return new TmdbSeriesClient(apiKey);
        }

        #region Torrent
        public async Task<TorrentSearchManager> CreateTorrentSearchManagerAsync()
        {
            var availableVfTorrentMovieSearchers = await GetAvailableServicesAsync(GetVfTorrentMovieSearchers());
            var availableVoTorrentMovieSearchers = await GetAvailableServicesAsync(GetVoTorrentMovieSearchers());
            var availableVoTorrentSerieSearchers = await GetAvailableServicesAsync(GetVoTorrentSerieSearchers());

            return new TorrentSearchManager(availableVfTorrentMovieSearchers, availableVoTorrentMovieSearchers, null, availableVoTorrentSerieSearchers);
        }

        private IEnumerable<ITorrentMovieSearcher> GetVfTorrentMovieSearchers()
        {
            return new List<ITorrentMovieSearcher>()
            {
                new YggTorrentSearcher(),
                new GkTorrentSearcher(),
                new ZeTorrentsSearcher()
            };
        }

        private IEnumerable<ITorrentMovieSearcher> GetVoTorrentMovieSearchers()
        {
            return new List<ITorrentMovieSearcher>()
            {
                new YtsHtmlV2Searcher(new YtsHtmlRsUrlProvider()),
                new YtsHtmlSearcher(new YtsHtmlOneUrlProvider()),
                new YtsApiSearcher(new YtsApiUrlMxProvider()),
            };
        }

        private IEnumerable<ITorrentSerieSearcher> GetVoTorrentSerieSearchers()
        {
            return new List<ITorrentSerieSearcher>()
            {
                new OneomSearcher()
            };
        }

        #endregion

        #region Subtitles
        public void SetSubtitlesFolder(string subtitlesFolder)
        {
            this.subtitlesFolder = subtitlesFolder;
        }
        public async Task<SubtitlesSearchManager> CreateSubstitlesSearchManagerAsync()
        {
            var availableMovieSubtitlesSearchers = await GetAvailableServicesAsync(GetMovieSubtitlesSearchers());
            var availableSerieSubtitlesSearchers = await GetAvailableServicesAsync(GetSerieSubtitlesSearchers());

            return new SubtitlesSearchManager(availableMovieSubtitlesSearchers, availableSerieSubtitlesSearchers);
        }

        private IEnumerable<ISubtitlesMovieSearcher> GetMovieSubtitlesSearchers()
        {
            if(string.IsNullOrEmpty(this.subtitlesFolder))
                throw new Exception("You have to call SetSubtitlesFolder method first");

            return new List<ISubtitlesMovieSearcher>()
            {
                new YtsSubsSearcher(new SubtitlesFileProvider(subtitlesFolder)),
                new OpenSubtitlesSearcher(new SubtitlesFileProvider(subtitlesFolder))
            };
        }

        private IEnumerable<ISubtitlesSerieSearcher> GetSerieSubtitlesSearchers()
        {
            if (string.IsNullOrEmpty(this.subtitlesFolder))
                throw new Exception("You have to call SetSubtitlesFolder method first");

            return new List<ISubtitlesSerieSearcher>()
            {
                new OpenSubtitlesSearcher(new SubtitlesFileProvider(subtitlesFolder))
            };
        }
        #endregion

        private async Task<IEnumerable<T>> GetAvailableServicesAsync<T>(IEnumerable<T> searcherServices) where T : ISearcherService
        {
            var availableServices = new List<T>();
            var tasks = new List<Task>();

            foreach (var searcher in searcherServices)
            {
                tasks.Add(IsAvailable(searcher).ContinueWith(t =>
                {
                    if (t.Result)
                        availableServices.Add(searcher);
                }));
            }

            await Task.WhenAll(tasks); 

            return availableServices;
        }

        private async Task<bool> IsAvailable(ISearcherService searcherService)
        {
            try
            {
                var result = await HttpRequester.GetAsync(new Uri(searcherService.GetPingUrl()));
                return !string.IsNullOrEmpty(result);
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
