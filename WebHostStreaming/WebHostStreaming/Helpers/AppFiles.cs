using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public static class AppFiles
    {
        public static string WatchedMovies => Path.Combine(AppFolders.DataFolder, "watchedmovies.json");
        public static string WatchedSeries => Path.Combine(AppFolders.DataFolder, "watchedseries.json");
        public static string BookmarkedMovies => Path.Combine(AppFolders.DataFolder, "bookmarkedmovies.json");
        public static string BookmarkedSeries => Path.Combine(AppFolders.DataFolder, "bookmarkedseries.json");
        public static string TorrentHistory => Path.Combine(AppFolders.DataFolder, "torrenthistory.json");
        public static string WindowsDesktopApp => Path.Combine(AppFolders.CurrentFolder, "Medflix.exe");
        public static string WindowsDesktopAppView => Path.Combine(AppFolders.CurrentFolder, "windows-app", "medflix.exe");
        public static string MacosDesktopAppView => Path.Combine(AppFolders.CurrentFolder, "Medflix.app", "Contents", "MacOS", "Medflix");
        public static string NewReleasePackage => Path.Combine(AppFolders.CurrentFolder, "medflix_release.zip");

        //use temp folder because 'extract-update' folder will be replaced during package updating 
        public static string WindowsExtractUpdateProgram => Path.Combine(AppFolders.ExtractUpdateProgramTempFolder, "extract_package.exe");
        public static string MacosExtractUpdateProgram => Path.Combine(AppFolders.ExtractUpdateProgramTempFolder, "Extract Medflix Package.app","Contents","MacOS","extract_package");
    }
}
