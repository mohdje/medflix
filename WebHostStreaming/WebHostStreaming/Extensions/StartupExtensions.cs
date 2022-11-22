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
            services.AddSingleton<ITorrentVideoStreamProvider, TorrentVideoStreamProvider>();
            services.AddSingleton<IBookmarkedMoviesProvider, BookmarkedMoviesProvider>();
            services.AddSingleton<IWatchedMoviesProvider, WatchedMoviesProvider>();
        }

        public static void SetupUI(this IApplicationBuilder applicationBuilder, IHostApplicationLifetime appLifetime)
        {
            if (PlatformConfiguration.PlatformIsWeb)
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
            else if (PlatformConfiguration.PlatformIsWindows)
            {
                if (!File.Exists(AppFiles.WindowsDesktopAppView))
                    throw new FileNotFoundException(AppFiles.WindowsDesktopAppView);

                appLifetime.ApplicationStarted.Register(() =>
                {
                    System.Diagnostics.Process.Start(AppFiles.WindowsDesktopAppView);
                });
            }
            else if (PlatformConfiguration.PlatformIsMacos)
            {
                if (!File.Exists(AppFiles.MacosDesktopAppView))
                    throw new FileNotFoundException(AppFiles.MacosDesktopAppView);

                appLifetime.ApplicationStarted.Register(() =>
                {
                    System.Diagnostics.Process.Start(AppFiles.MacosDesktopAppView);
                });
            }
           
        }
    }
}
