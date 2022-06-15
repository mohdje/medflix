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
        public static string TorrentsFolder => Path.Combine(AppContext.BaseDirectory, "torrents");
        public static string ViewFolder => Path.Combine(AppContext.BaseDirectory, "view");
        public static string CurrentFolder => AppContext.BaseDirectory;

        public static void SetupTorrentsFolder()
        {
            if (!Directory.Exists(TorrentsFolder))
                Directory.CreateDirectory(TorrentsFolder);
            else
            {
                var directories = Directory.GetDirectories(TorrentsFolder);
                foreach (var folder in directories)
                {
                    var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

                    if (files.Length > 0)
                    {
                        var oldestUsedFileDateTime = files.Select(f => File.GetLastWriteTime(f))
                                .OrderBy(f => f.Ticks)
                                .Reverse()
                                .First();

                        if (DateTime.Now - oldestUsedFileDateTime >= TimeSpan.FromDays(2))
                            Directory.Delete(folder, true);
                    }
                    else
                        Directory.Delete(folder, true);
                }
            }
        }

        public static void SetupSubtitlesFolder()
        {
            if (!Directory.Exists(SubtitlesFolder))
                Directory.CreateDirectory(SubtitlesFolder);

        }
    }
}
