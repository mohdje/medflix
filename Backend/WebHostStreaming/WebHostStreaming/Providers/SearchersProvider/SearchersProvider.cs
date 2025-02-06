using MoviesAPI.Services;
using MoviesAPI.Services.Content;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Torrent;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Providers
{
    public class SearchersProvider : ISearchersProvider
    {
        private IMovieSearcher _movieSearcher;
        private ISeriesSearcher _seriesSearcher;
        public IMovieSearcher MovieSearcher => _movieSearcher;
        public ISeriesSearcher SeriesSearcher => _seriesSearcher;

        public TorrentSearchManager _torrentSearchManager;
        public TorrentSearchManager TorrentSearchManager => _torrentSearchManager;

        public SubtitlesSearchManager _subtitlesSearchManager;
        public SubtitlesSearchManager SubtitlesSearchManager => _subtitlesSearchManager;

        public SearchersProvider()
        {
           MoviesAPIFactory.Instance.SetSubtitlesFolder(AppFolders.SubtitlesFolder);

            _subtitlesSearchManager = MoviesAPIFactory.Instance.CreateSubstitlesSearchManager();
            _torrentSearchManager = MoviesAPIFactory.Instance.CreateTorrentSearchManager();

            _movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(Tokens.TmdbApiKey);
            _seriesSearcher = MoviesAPIFactory.Instance.CreateSeriesSearcher(Tokens.TmdbApiKey);
        }     
    }
}
