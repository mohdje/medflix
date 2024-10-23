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
            AppStart.Start(args);
        }
    }
}
