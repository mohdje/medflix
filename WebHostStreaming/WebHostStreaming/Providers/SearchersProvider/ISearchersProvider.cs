
using MoviesAPI.Services.Content;
using MoviesAPI.Services.Subtitles;
using MoviesAPI.Services.Torrent;

namespace WebHostStreaming.Providers
{
    public interface ISearchersProvider
    {
        IMovieSearcher MovieSearcher { get; }
        ISeriesSearcher SeriesSearcher { get; }
        TorrentSearchManager TorrentSearchManager { get; }
        SubtitlesSearchManager SubtitlesSearchManager { get; }
    }
}
