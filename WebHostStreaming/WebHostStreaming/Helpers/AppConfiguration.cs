using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;

namespace WebHostStreaming.Helpers
{
    public static class AppConfiguration
    {
        public static bool IsDesktopApplication { get; private set; }

        public static void Init(bool isDesktopApplication)
        {
            IsDesktopApplication = isDesktopApplication;
        }
    }
}
