using Microsoft.Extensions.Configuration;

namespace WebHostStreaming.Helpers
{
    public static class AppConfiguration
    {
        const string PLATFORM = "Platform";
        const string WEB = "web";
        const string WINDOWS = "windows";
        const string MACOS = "macos";
        const string VERSION_NAME = "VersionName";

        private static string platformValue;
        private static string versionName;

        public static bool IsWebVersion => platformValue.Equals(WEB, System.StringComparison.OrdinalIgnoreCase);
        public static bool IsWindowsVersion => platformValue.Equals(WINDOWS, System.StringComparison.OrdinalIgnoreCase);
        public static bool IsMacosVersion => platformValue.Equals(MACOS, System.StringComparison.OrdinalIgnoreCase);
        public static string VersionName => versionName;

        public static void ReadPlatformConfiguration(IConfiguration configuration)
        {
            platformValue = configuration.GetValue<string>(PLATFORM);
            versionName = configuration.GetValue<string>(VERSION_NAME);
        }
    }
}
