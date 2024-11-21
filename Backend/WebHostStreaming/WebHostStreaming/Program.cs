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
                    webBuilder.UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = null;
                    });
                    webBuilder.UseConfiguration(configuration);
                    webBuilder.UseUrls("http://0.0.0.0:5000");
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
