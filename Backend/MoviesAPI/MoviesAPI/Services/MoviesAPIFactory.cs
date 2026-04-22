using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Torrent;
using System;
using System.Collections.Generic;
using MoviesAPI.Services.Content;
using MoviesAPI.Services.Torrent.Searchers.WebScrappers;
using System.IO;

namespace MoviesAPI.Services
{
    public static class MoviesAPIFactory
    {

        public static IMovieSearcher CreateMovieSearcher(string apiKey)
        {
            return new TmdbMovieClient(apiKey);
        }

        public static ISeriesSearcher CreateSeriesSearcher(string apiKey)
        {
            return new TmdbSeriesClient(apiKey);
        }

        public static TorrentSearchManager CreateTorrentSearchManager()
        {
            var torrent9Scrapper = new Torrent9WebScrapper();
            var zoneTorrentScrapper = new ZoneTorrentScrapper();
            var oxTorrentScrapper = new OxTorrentScrapper();
            var ytsDoWebScrapper = new YtsDoWebScrapper();
            var ytsRsWebScrapper = new YtsRsWebScrapper();
            var ytsApiSearcher = new YtsApiSearcher();
            var torrentDownloadInfoSearcher = new TorrentDownloadInfoSearcher();

            IEnumerable<ITorrentSearcher> vfTorrentSearchers = [oxTorrentScrapper, torrent9Scrapper, zoneTorrentScrapper];

            return new TorrentSearchManager(
                vfTorrentSearchers,
                [ytsApiSearcher, ytsDoWebScrapper],
                vfTorrentSearchers,
                [torrentDownloadInfoSearcher]);
        }

        public static SubtitlesSearchManager CreateSubstitlesSearchManager(string subtitlesFolder)
        {
            if (string.IsNullOrEmpty(subtitlesFolder) || !Directory.Exists(subtitlesFolder))
                throw new Exception("subtitlesFolder null or does not exist");

            var subtitlesDownloader = new SubtitlesDownloader(subtitlesFolder);
            var ytsSubsSearcher = new YtsSubsSearcher(subtitlesDownloader);
            var openSubtitlesSearcher = new OpenSubtitlesSearcher(subtitlesDownloader);

            return new SubtitlesSearchManager([openSubtitlesSearcher, ytsSubsSearcher], [openSubtitlesSearcher]);
        }
    }
}
