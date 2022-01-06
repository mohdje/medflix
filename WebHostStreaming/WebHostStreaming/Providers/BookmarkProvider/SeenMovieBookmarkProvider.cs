using WebHostStreaming.Helpers;

namespace WebHostStreaming.Providers
{
    public class SeenMovieBookmarkProvider : BookmarkProvider, ISeenMovieBookmarkProvider
    {
        protected override string GetFilePath()
        {
            return AppFiles.LastSeenMovies;
        }
    }
}
