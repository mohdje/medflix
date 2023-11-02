using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Tools
{
    public static class AppFolders
    {
        public static string ExtractUpdateProgramFolder => Path.Combine(AppContext.BaseDirectory, "extract-update");
        public static string ExtractUpdateProgramTempFolder => $"{ExtractUpdateProgramFolder}-temp";

        private static void DeleteUpdateTempItems()
        {
            if (Directory.Exists(ExtractUpdateProgramTempFolder))
                Directory.Delete(ExtractUpdateProgramTempFolder, true);

            if (File.Exists(AppFiles.NewReleasePackage))
                File.Delete(AppFiles.NewReleasePackage);
        }
    }
}
