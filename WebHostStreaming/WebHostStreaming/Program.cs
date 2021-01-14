using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using WebHostStreaming.Helpers;

namespace WebHostStreaming
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupBeforeRun();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void SetupBeforeRun()
        {
            AppFolders.CreateViewFolders();
            AppFolders.CreateTorrentsFolder();
        }
    }
}
