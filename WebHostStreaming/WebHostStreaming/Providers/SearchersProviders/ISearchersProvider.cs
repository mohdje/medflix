using MoviesAPI.Services.Movies;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Torrent;

namespace WebHostStreaming.Providers
{
    public interface ISearchersProvider
    {
        IMovieSearcher MovieSearcher { get; }
        TorrentSearchManager TorrentSearchManager { get; }
        SubtitlesSearchManager SubtitlesSearchManager { get; }
    }
}
