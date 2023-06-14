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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(configuration);
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static void SetupBeforeRun()
        {
            System.Console.WriteLine("---- Setup folders ----");
            AppFolders.SetupFolders();
        }
    }
}
