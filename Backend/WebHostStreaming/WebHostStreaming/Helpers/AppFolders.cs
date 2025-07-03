using System;
using System.IO;

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
        public static string HomeViewFolder => Path.Combine(ViewFolder, "home");
        public static string ManageViewFolder => Path.Combine(ViewFolder, "manage");
        
    }
}
