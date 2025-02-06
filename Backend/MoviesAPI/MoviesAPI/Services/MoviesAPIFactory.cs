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
using System.Threading;
using System.Runtime.CompilerServices;
using MoviesAPI.Services.Torrent.Searchers;
using MoviesAPI.Services.Torrent.Searchers.WebScrappers;

namespace MoviesAPI.Services
{
    public class MoviesAPIFactory
    {
        private static MoviesAPIFactory _instance;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

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

        private Dictionary<string, bool> serviceAvailibilityCache;

        private MoviesAPIFactory()
        {
            serviceAvailibilityCache = new Dictionary<string, bool>();
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
        public TorrentSearchManager CreateTorrentSearchManager()
        {
            var availableVoTorrentMovieSearchers = GetVoTorrentMovieSearchers();
            var availableVoTorrentSerieSearchers = GetVoTorrentSerieSearchers();
            var availableVfTorrentMovieSearchers = GetVfTorrentMovieSearchers();
            var availableVfTorrentSerieSearchers = GetVfTorrentSerieSearchers();

            return new TorrentSearchManager(availableVfTorrentMovieSearchers, availableVoTorrentMovieSearchers, availableVfTorrentSerieSearchers, availableVoTorrentSerieSearchers);
        }

        private IEnumerable<ITorrentSearcher> GetVfTorrentMovieSearchers()
        {
            return new List<ITorrentSearcher>()
            {
                new YggTorrentScrapper(),
                new YtsVfTorrentScrapper(),
                new ZeTorrentsScrapper()
                //new GkTorrentSearcher(),
            };
        }

        private IEnumerable<ITorrentSearcher> GetVoTorrentMovieSearchers()
        {
            return new List<ITorrentSearcher>()
            {
               //new YtsMxWebScrapper(),
               new YtsDoWebScrapper(),
               new YtsRsWebScrapper(),
               new YtsApiSearcher(),
            };
        }

        private IEnumerable<ITorrentSearcher> GetVoTorrentSerieSearchers()
        {
            return new List<ITorrentSearcher>()
            {
                new LimeTorrentsScrapper(),
               // new One337xScapper()
            };
        }

        private IEnumerable<ITorrentSearcher> GetVfTorrentSerieSearchers()
        {
            return new List<ITorrentSearcher>()
            {
                new YggTorrentScrapper(),
                new YtsVfTorrentScrapper(),
                new ZeTorrentsScrapper(),
                //new GkTorrentSearcher(),
            };
        }

        #endregion

        #region Subtitles
        public void SetSubtitlesFolder(string subtitlesFolder)
        {
            this.subtitlesFolder = subtitlesFolder;
        }
        public SubtitlesSearchManager CreateSubstitlesSearchManager()
        {
            var availableMovieSubtitlesSearchers = GetMovieSubtitlesSearchers();
            var availableSerieSubtitlesSearchers = GetSerieSubtitlesSearchers();

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
    }
}
