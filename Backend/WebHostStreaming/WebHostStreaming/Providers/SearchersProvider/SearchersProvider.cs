using MedflixAPI.Services;
using MedflixAPI.Services.Content;
using MedflixAPI.Services.Subtitles;
using MedflixAPI.Services.Torrent;
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

            SubtitlesSearchManager = MedflixAPIFactory.CreateSubstitlesSearchManager(AppFolders.SubtitlesFolder);
            TorrentSearchManager = MedflixAPIFactory.CreateTorrentSearchManager();

            MovieSearcher = MedflixAPIFactory.CreateMovieSearcher(Tokens.TmdbApiKey);
            SeriesSearcher = MedflixAPIFactory.CreateSeriesSearcher(Tokens.TmdbApiKey);
        }
    }
}
