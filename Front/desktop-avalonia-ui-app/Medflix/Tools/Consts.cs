
namespace Medflix.Tools
{
    public static class Consts
    {
        public static string AppVersionName => $"Medflix {AppVersion}";
        public const string AppVersion = "2.2.1";
        public const string LatestReleaseUrl = "https://api.github.com/repos/mohdje/medflix/releases/latest";

        //use react project for debug http://localhost:3000/index.html
        //prod url http://localhost:5000/home/index.html
        public const string MainAppViewUrl = "http://localhost:5000/home/index.html";
    }
}
