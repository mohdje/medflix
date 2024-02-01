
namespace MedflixWinForms.Utils
{
    public static class Consts
    {
        public static string AppVersionName => $"Medflix {AppVersion}";
        public const string AppVersion = "2.2.2";
        public const string LatestReleaseUrl = "https://api.github.com/repos/mohdje/medflix/releases/latest";

        public const string MainAppViewUrlDebug = "http://localhost:3000/index.html";
        public const string MainAppViewUrl = "http://localhost:5000/home/index.html";
        public const string WebHostUrl = "http://localhost:5000";
    }
}
