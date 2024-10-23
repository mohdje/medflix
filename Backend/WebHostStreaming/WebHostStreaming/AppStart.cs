using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
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

            PlanCleanUpFoldersOperation();

            CreateHostBuilder(args).Build().Run();
        }

       
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false)
                                .Build();

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(configuration);
                    webBuilder.UseUrls("http://0.0.0.0:5000");
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static void PlanCleanUpFoldersOperation()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var operationDateTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 9, 0, 0);
            var timeSpan = operationDateTime - DateTime.Now;
            var delay = (int)timeSpan.TotalMilliseconds;

            Timer timer = new Timer((e) =>
            {
                AppFolders.SetupFolders();
            }, null, delay, 24*60*60*1000);// every 24 hours
        }

    }
}
