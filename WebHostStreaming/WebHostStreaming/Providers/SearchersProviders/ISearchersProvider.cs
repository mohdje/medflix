namespace WebHostStreaming.Providers
{
    public interface ISearchersProvider : ISubtitlesSearcherProvider, IVFMovieSearcherProvider, IVOMovieSearcherProvider
    {
        void SaveSources();
    }
}
