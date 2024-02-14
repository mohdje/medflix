using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Helpers;

namespace WebHostStreaming
{
    public static class AppStart
    {
        public static void Start(string[] args)
        {
            System.Console.WriteLine("---- Setup folders ----");
            AppFolders.SetupFolders();

            CreateHostBuilder(args, args.Contains("-d")).Build().Run();
        }

        public static IHost CreateHost(string[] args, bool isDesktopApplication)
        {
            AppFolders.SetupFolders();

            return CreateHostBuilder(args, isDesktopApplication).Build();
        }
        private static IHostBuilder CreateHostBuilder(string[] args, bool isDeskopApplication)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            AppConfiguration.Init(isDeskopApplication);

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(configuration);
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
