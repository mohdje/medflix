using Microsoft.Extensions.Configuration;

namespace WebHostStreaming.Helpers
{
    public static class PlatformConfiguration
    {
        const string PLATFORM = "Platform";
        const string WEB = "web";
        const string WINDOWS = "windows";
        const string MACOS = "macos";
        private static string platformValue;

        public static bool PlatformIsWeb => platformValue.Equals(WEB, System.StringComparison.OrdinalIgnoreCase);
        public static bool PlatformIsWindows => platformValue.Equals(WINDOWS, System.StringComparison.OrdinalIgnoreCase);
        public static bool PlatformIsMacos => platformValue.Equals(MACOS, System.StringComparison.OrdinalIgnoreCase);

        public static void ReadPlatformConfiguration(IConfiguration configuration)
        {
            platformValue = configuration.GetValue<string>(PLATFORM);
        }
    }
}
