using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public static class AppFolders
    {
        public static string SubtitlesFolder => Path.Combine(AppContext.BaseDirectory, "subtitles");
        public static string TorrentsFolder => Path.Combine(AppContext.BaseDirectory, "torrents/");
        public static string ViewFolder => Path.Combine(AppContext.BaseDirectory, "view");
        public static string CurrentFolder => AppContext.BaseDirectory;

        public static void CreateTorrentsFolder()
        {
            if (Directory.Exists(TorrentsFolder))
                Directory.Delete(TorrentsFolder, true);

            Directory.CreateDirectory(TorrentsFolder);
        }

        public static void CreateViewFolders()
        {
            var viewZipFile = Path.Combine(CurrentFolder, "view.zip");

            using (var fs = new FileStream(viewZipFile, FileMode.Create))
                fs.Write(Properties.Resources.View);

            if (Directory.Exists(ViewFolder))
                Directory.Delete(ViewFolder, true);

            System.IO.Compression.ZipFile.ExtractToDirectory(viewZipFile, ViewFolder);
        }
    }
}
