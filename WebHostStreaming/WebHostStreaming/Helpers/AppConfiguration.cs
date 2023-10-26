using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;

namespace WebHostStreaming.Helpers
{
    public static class AppConfiguration
    {
        const string VERSION_NAME = "VersionName";

        private static string versionName;

        public static bool IsDesktopApplication { get; private set; }
        public static string VersionName => versionName;


        public static void Init(IConfiguration configuration, bool isDesktopApplication)
        {
            IsDesktopApplication = isDesktopApplication;
            versionName = configuration.GetValue<string>(VERSION_NAME);
        }
    }
}
