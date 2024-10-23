using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    internal class SubtitlesFileProvider : ISubtitlesFileProvider
    {
        private readonly string ExtractionFolder;
        private string SubtitlesZipFile => Path.Combine(ExtractionFolder, "subtitles.zip");
        internal SubtitlesFileProvider(string extractionFolder) : base()
        {
            if (string.IsNullOrEmpty(extractionFolder))
                throw new Exception("You have to provide an extraction folder");

            ExtractionFolder = extractionFolder;
        }
        public async Task<string> GetSubtitlesFileAsync(string subtitlesSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders)
        {
            if (File.Exists(SubtitlesZipFile))
                File.Delete(SubtitlesZipFile);

            var result = await HttpRequester.DownloadAsync(new Uri(subtitlesSourceUrl), httpRequestHeaders);

            File.WriteAllBytes(SubtitlesZipFile, result);

            if (!File.Exists(SubtitlesZipFile))
                throw new FileNotFoundException($"{SubtitlesZipFile} not found");

            var extractedFile = Path.Combine(ExtractionFolder, "subtitles.srt");
            if (File.Exists(extractedFile))
                File.Delete(extractedFile);

            using (ZipArchive archive = ZipFile.OpenRead(SubtitlesZipFile))
            {
                var subtitleFileEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".srt", StringComparison.OrdinalIgnoreCase));

                if (subtitleFileEntry != null)
                {
                    // Gets the full path to ensure that relative segments are removed.
                    string destinationPath = Path.GetFullPath(extractedFile);

                    // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                    // are case-insensitive.
                    var destinationFolder = Path.GetDirectoryName(extractedFile);
                    if (destinationPath.StartsWith(destinationFolder, StringComparison.Ordinal))
                    {
                        subtitleFileEntry.ExtractToFile(destinationPath);
                        return extractedFile;
                    }
                }
            }

            return null;
        }
    }
}
