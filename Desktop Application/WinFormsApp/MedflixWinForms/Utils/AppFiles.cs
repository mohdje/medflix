namespace MedflixWinForms.Utils
{
    public static class AppFiles
    {
        public static string WindowsDesktopApp => Path.Combine(AppContext.BaseDirectory, "Medflix.exe");
        public static string NewReleasePackage => Path.Combine(AppContext.BaseDirectory, "medflix_release.zip");

        //use temp folder because 'extract-update' folder will be replaced during package updating 
        public static string WindowsExtractUpdateProgram => Path.Combine(AppFolders.ExtractUpdateProgramTempFolder, "extract_package.exe");
    }
}
