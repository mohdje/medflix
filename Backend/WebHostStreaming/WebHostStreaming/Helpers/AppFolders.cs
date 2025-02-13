using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebHostStreaming.Models;

namespace WebHostStreaming.Helpers
{
    public static class AppFolders
    {
        public static string CurrentFolder => AppContext.BaseDirectory;
        public static string StorageFolder => Path.Combine(CurrentFolder, "storage");
        public static string DataFolder => Path.Combine(StorageFolder, "data");
        public static string SubtitlesFolder => Path.Combine(CurrentFolder, "subtitles");
        public static string TorrentsFolder => Path.Combine(StorageFolder, "torrents");
        public static string UploadFoler => Path.Combine(StorageFolder, "upload");
        public static string ViewFolder => Path.Combine(CurrentFolder, "view");
        public static string ManageViewFolder => Path.Combine(ViewFolder, "manage");
        

        private static void CleanUpTorrentsFolder()
        {
            //if (!Directory.Exists(TorrentsFolder))
            //    return;

            //var watchedMedias = GetWatchedMedias();
            //var directories = Directory.GetDirectories(TorrentsFolder);
            //foreach (var folder in directories)
            //{
            //    var watchedMedia = watchedMedias.SingleOrDefault(watchedMedia => watchedMedia.TorrentUrl?.ToMD5Hash() == Path.GetFileName(folder));
            //    if (watchedMedia == null)
            //    {
            //        Directory.Delete(folder, true);
            //    }
            //    else
            //    {
            //        var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

            //        if (files.Any())
            //        {
            //            var oldestUsedFileDateTime = files.Select(f => File.GetLastAccessTime(f))
            //                    .OrderBy(f => f.Ticks)
            //                    .Reverse()
            //                    .First();

            //            if ((watchedMedia.CurrentTime / watchedMedia.TotalDuration) >= 0.95
            //                && DateTime.Now - oldestUsedFileDateTime >= TimeSpan.FromDays(3))
            //                Directory.Delete(folder, true);
            //            else if (DateTime.Now - oldestUsedFileDateTime >= TimeSpan.FromDays(10))
            //                Directory.Delete(folder, true);
            //        }
            //        else
            //            Directory.Delete(folder, true);
            //    }
            //}
        }

        private static IEnumerable<WatchedMediaDto> GetWatchedMedias()
        {
            var watchedMediaFiles = new string[] { AppFiles.WatchedMovies, AppFiles.WatchedSeries };

            var watchedMedias = new List<WatchedMediaDto>();

            foreach (var file in watchedMediaFiles)
            {
                if (File.Exists(file))
                {
                    var medias = JsonHelper.DeserializeFromFile<WatchedMediaDto[]>(file);
                    if (medias != null && medias.Any())
                        watchedMedias.AddRange(medias);
                }
            }

            return watchedMedias;
        }
    }
}
