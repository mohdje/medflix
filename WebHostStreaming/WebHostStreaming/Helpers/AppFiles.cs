using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public static class AppFiles
    {
        public static string LastSeenMovies => Path.Combine(AppFolders.CurrentFolder, "lastseenmovies.json");
        public static string BookmarkedMovies => Path.Combine(AppFolders.CurrentFolder, "bookmarkedmovies.json");
        public static string SourcesSettings => Path.Combine(AppFolders.CurrentFolder, "sourcessettings.json");
        public static string WindowsDesktopAppView => Path.Combine(AppFolders.CurrentFolder, "windows-app", "medflix.exe");
        public static string MacosDesktopAppView => Path.Combine(AppFolders.CurrentFolder, "Medflix.app/Contents/MacOS/Medflix");
    }
}
