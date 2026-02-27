
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoviesAPI.Services.Torrent.Dtos;
using WebHostStreaming.Models;
using WebHostStreaming.Providers;

namespace WebHostStreaming.Torrent
{
    public class TorrentMovieDownloader : TorrentMediaDownloader<MovieLiteInfo>
    {
        readonly IBookmarkedMoviesProvider bookmarkedMoviesProvider;

        protected override string MediaType => "movies";

        public TorrentMovieDownloader(
            ITorrentContentProvider torrentContentProvider,
            IVideoInfoProvider videoInfoProvider,
            ISearchersProvider searchersProvider,
            IBookmarkedMoviesProvider bookmarkedMoviesProvider
         )
            : base(torrentContentProvider, videoInfoProvider, searchersProvider)
        {
            this.bookmarkedMoviesProvider = bookmarkedMoviesProvider;
            this.bookmarkedMoviesProvider.MovieBookmarkAdded += async (s, movieBookmark) =>
            {
                TryAddMediaToDownload(new MovieLiteInfo
                {
                    Id = movieBookmark.Id,
                    Title = movieBookmark.Title,
                    Year = movieBookmark.Year
                });
                OnMediaAddedToDownloadList();
            };
            this.bookmarkedMoviesProvider.MovieBookmarkDeleted += (s, movieId) =>
            {
                voDownloadList.TryRemove(movieId, out _);
                vfDownloadList.TryRemove(movieId, out _);
            };
        }

        protected override Task<IEnumerable<MovieLiteInfo>> GetMediasToDownloadAsync()
        {
            return Task.FromResult(bookmarkedMoviesProvider.GetBookmarkedMovies().Select(m => new MovieLiteInfo { Id = m.Id, Title = m.Title, Year = m.Year }));
        }

        protected override bool VideoExists(MovieLiteInfo media, LanguageVersion languageVersion)
        {
            return videoInfoProvider.GetMovieVideoInfo(media.Id, languageVersion) != null;
        }

        protected override string GetMediaId(MovieLiteInfo media)
        {
            return media.Id;
        }

        protected override string GetMediaInfo(MovieLiteInfo media)
        {
            return $"{media.Title} ({media.Year})";
        }

        protected override async Task<IEnumerable<MediaTorrent>> SearchVoTorrentsAsync(MovieLiteInfo movie)
        {
            return await searchersProvider.TorrentSearchManager.SearchVoTorrentsMovieAsync(movie.Title, movie.Year);
        }

        protected override async Task<IEnumerable<MediaTorrent>> SearchVfTorrentsAsync(MovieLiteInfo media)
        {
            var frenchTitle = await searchersProvider.MovieSearcher.GetMovieFrenchTitleAsync(media.Id);
            if (string.IsNullOrEmpty(frenchTitle))
                frenchTitle = media.Title;

            return await searchersProvider.TorrentSearchManager.SearchVfTorrentsMovieAsync(frenchTitle, media.Year);
        }

        protected override IEnumerable<TorrentRequest> BuildTorrentRequests(string clientId, MovieLiteInfo media, IEnumerable<MediaTorrent> torrents, LanguageVersion languageVersion)
        {
            foreach (var torrent in torrents)
            {
                yield return new TorrentRequest(clientId, torrent.DownloadUrl, media.Id, torrent.Quality, languageVersion);
            }
        }
    }
}