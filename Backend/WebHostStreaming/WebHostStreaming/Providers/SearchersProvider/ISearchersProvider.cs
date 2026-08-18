
using MedflixAPI.Services.Content;
using MedflixAPI.Services.Subtitles;
using MedflixAPI.Services.Torrent;

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
