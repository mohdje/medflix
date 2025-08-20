using Microsoft.Extensions.DependencyInjection;
using WebHostStreaming.Providers;
using WebHostStreaming.Torrent;

namespace WebHostStreaming.StartupExtensions
{
    public static class StartupExtensions
    {
        public static void AddProviders(this IServiceCollection services)
        {
            services.AddSingleton<ISearchersProvider, SearchersProvider>();
            services.AddSingleton<IVideoInfoProvider, VideoInfoProvider>();
            services.AddSingleton<ITorrentContentProvider, TorrentContentProvider>();
            services.AddSingleton<IBookmarkedMoviesProvider, BookmarkedMoviesProvider>();
            services.AddSingleton<IBookmarkedSeriesProvider, BookmarkedSeriesProvider>();
            services.AddSingleton<IWatchedMoviesProvider, WatchedMoviesProvider>();
            services.AddSingleton<IWatchedSeriesProvider, WatchedSeriesProvider>();
            services.AddSingleton<IRecommandationsProvider, RecommandationsProvider>();
            services.AddSingleton<ITorrentAutoDownloader, TorrentAutoDownloader>();
        }
    }
}
