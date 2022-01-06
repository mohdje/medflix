using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public class MovieToSeeBookmarkProvider : BookmarkProvider, IMovieToSeeBookmarkProvider
    {
        protected override string GetFilePath()
        {
            return AppFiles.BookmarkedMovies;
        }
    }
}
