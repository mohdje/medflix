using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using WebHostStreaming.Helpers;
using WebHostStreaming.Providers;

namespace WebHostStreaming.StartupExtensions
{
    public static class StartupExtensions
    {
        public static void AddProviders(this IServiceCollection services)
        {
            services.AddSingleton<ISearchersProvider, SearchersProvider>();
            services.AddSingleton<ITorrentContentProvider, TorrentContentProvider>();
            services.AddSingleton<ITorrentHistoryProvider, TorrentHistoryProvider>();
            services.AddSingleton<IBookmarkedMediaProvider, BookmarkedMediaProvider>();
            services.AddSingleton<IWatchedMediaProvider, WatchedMediaProvider>();
            services.AddSingleton<IRecommandationsProvider, RecommandationsProvider>();
        }

        public static void SetupUI(this IApplicationBuilder applicationBuilder)
        {
            if (!Directory.Exists(AppFolders.ViewFolder))
                throw new DirectoryNotFoundException(AppFolders.ViewFolder);

            applicationBuilder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(AppFolders.ViewFolder),
                RequestPath = "/home"
            });

            System.Console.WriteLine("Web application accessible here : http://localhost:5000/home/index.html");
        }
    }
}
