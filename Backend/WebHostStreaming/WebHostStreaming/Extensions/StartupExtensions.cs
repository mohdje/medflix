using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using WebHostStreaming.Helpers;
using WebHostStreaming.Properties;
using WebHostStreaming.Providers;
using WebHostStreaming.Providers.AvailableVideosListProvider;
using WebHostStreaming.Providers.TorrentContentProvider;

namespace WebHostStreaming.StartupExtensions
{
    public static class StartupExtensions
    {
        public static void AddProviders(this IServiceCollection services)
        {
            services.AddSingleton<ISearchersProvider, SearchersProvider>();
            services.AddSingleton<IAvailableVideosListProvider, AvailableVideosListProvider>();
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

            if (Directory.Exists(AppFolders.ManageViewFolder))
                Directory.Delete(AppFolders.ManageViewFolder, true);

            Directory.CreateDirectory(AppFolders.ManageViewFolder);

            File.WriteAllText(AppFiles.UploadHtmlPage, Resources.ManageVideosPage);

            applicationBuilder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(AppFolders.ManageViewFolder),
                RequestPath = "/manage"
            });

            System.Console.WriteLine("Web application accessible here : http://*:5000/home/index.html");
            System.Console.WriteLine("Media resources management page accessible here : http://*:5000/manage/index.html");
        }
    }
}
