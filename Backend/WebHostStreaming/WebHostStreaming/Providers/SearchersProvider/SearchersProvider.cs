using MoviesAPI.Services;
using MoviesAPI.Services.Content;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Torrent;
using System.IO;
using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public class SearchersProvider : ISearchersProvider
    {
        public IMovieSearcher MovieSearcher { get; }
        public ISeriesSearcher SeriesSearcher { get; }
        public TorrentSearchManager TorrentSearchManager { get; }
        public SubtitlesSearchManager SubtitlesSearchManager { get; }

        public SearchersProvider()
        {
            if (!Directory.Exists(AppFolders.SubtitlesFolder))
                Directory.CreateDirectory(AppFolders.SubtitlesFolder);

            SubtitlesSearchManager = MoviesAPIFactory.Instance.CreateSubstitlesSearchManager(AppFolders.SubtitlesFolder);
            TorrentSearchManager = MoviesAPIFactory.Instance.CreateTorrentSearchManager();

            MovieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(Tokens.TmdbApiKey);
            SeriesSearcher = MoviesAPIFactory.Instance.CreateSeriesSearcher(Tokens.TmdbApiKey);
        }     
    }
}
