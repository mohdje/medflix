using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Torrent;
using System;
using System.Collections.Generic;
using MoviesAPI.Services.Content;
using MoviesAPI.Services.Torrent.Searchers;
using MoviesAPI.Services.Torrent.Searchers.WebScrappers;
using System.IO;
using MoviesAPI.Services.Subtitles.Searchers;

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

        public IMovieSearcher CreateMovieSearcher(string apiKey)
        {
            return new TmdbMovieClient(apiKey);
        }

        public ISeriesSearcher CreateSeriesSearcher(string apiKey)
        {
            return new TmdbSeriesClient(apiKey);
        }

        public TorrentSearchManager CreateTorrentSearchManager()
        {
            var torrent9Scrapper = new Torrent9WebScrapper();
            var zoneTorrentScrapper = new ZoneTorrentScrapper();
            var oxTorrentScrapper = new OxTorrentScrapper();
            var ytsDoWebScrapper = new YtsDoWebScrapper();
            var ytsRsWebScrapper = new YtsRsWebScrapper();
            var ytsApiSearcher = new YtsApiSearcher();
            var limeTorrentsScrapper = new LimeTorrentsScrapper();
            var eztvApiSearcher = new EztvApiSearcher();

            IEnumerable<ITorrentSearcher> vfTorrentSearchers = [oxTorrentScrapper, zoneTorrentScrapper, torrent9Scrapper];

            return new TorrentSearchManager(
                vfTorrentSearchers,
                [ytsApiSearcher, ytsDoWebScrapper, ytsRsWebScrapper],
                vfTorrentSearchers,
                [eztvApiSearcher, limeTorrentsScrapper]);
        }

        public SubtitlesSearchManager CreateSubstitlesSearchManager(string subtitlesFolder)
        {
            if (string.IsNullOrEmpty(subtitlesFolder) || !Directory.Exists(subtitlesFolder))
                throw new Exception("subtitlesFolder null or does not exist");

            var subtitlesDownloader = new SubtitlesDownloader(subtitlesFolder);
            var ytsSubsSearcher = new YtsSubsSearcher(subtitlesDownloader);
            var openSubtitlesSearcher = new OpenSubtitlesSearcher(subtitlesDownloader);
            var subSourceApi = new SubSourceApi(subtitlesDownloader);

            //return new SubtitlesSearchManager([subSourceApi, ytsSubsSearcher], [subSourceApi]);
            return new SubtitlesSearchManager([openSubtitlesSearcher], [openSubtitlesSearcher]);
        }

        public AIRecommendationEngine CreateAIRecommandationEngine(string token, IMovieSearcher moviesSearcher, ISeriesSearcher seriesSearcher)
        {
            return new AIRecommendationEngine(token, moviesSearcher, seriesSearcher);
        }
    }
}
