using MoviesAPI.Services;
using MoviesAPI.Services.Movies;
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
        public IMovieSearcher MovieSearcher => _movieSearcher;

        public TorrentSearchManager _torrentSearchManager;
        public TorrentSearchManager TorrentSearchManager => _torrentSearchManager;

        public SubtitlesSearchManager _subtitlesSearchManager;
        public SubtitlesSearchManager SubtitlesSearchManager => _subtitlesSearchManager;

        public SearchersProvider()
        {
           MoviesAPIFactory.Instance.SetSubtitlesFolder(AppFolders.SubtitlesFolder);

            Initialize().Wait();
        }

        private async Task Initialize()
        {
            _subtitlesSearchManager = await  MoviesAPIFactory.Instance.CreateSubstitlesSearchManagerAsync();
            _torrentSearchManager = await MoviesAPIFactory.Instance.CreateTorrentSearchManagerAsync();

            _movieSearcher = MoviesAPIFactory.Instance.CreateMovieSearcher(Tokens.TmdbApiKey);
        }

     
    }
}
