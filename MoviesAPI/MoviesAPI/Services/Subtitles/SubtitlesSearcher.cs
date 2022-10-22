using MoviesAPI.Helpers;
using MoviesAPI.Services.Subtitles.DTOs;
using MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Subtitles
{
    public abstract class SubtitlesSearcher : BaseService
    {
        private static string ExtractionFolder;
        private string SubtitlesZipFile => Path.Combine(ExtractionFolder, "subtitles.zip");


        internal SubtitlesSearcher() : base()
        {
            if (string.IsNullOrEmpty(ExtractionFolder))
                throw new Exception("You have to provide an extraction folder by calling SubtitlesSearcher.SetExtractionFolderLocation() before instantiation of an object.");

        }
       
        public abstract Task<SubtitlesSearchResultDto> GetAvailableSubtitlesAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage);

        public abstract Task<IEnumerable<SubtitlesDto>> GetSubtitlesAsync(string subtitlesSourceUrl);

        protected abstract string GetLanguageCode(SubtitlesLanguage subtitlesLanguage);

        protected abstract string GetLanguageLabel(SubtitlesLanguage subtitlesLanguage);

        protected async Task<string> GetSubtitlesFileAsync(string subtitleSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders)
        {
            if (File.Exists(SubtitlesZipFile))
                File.Delete(SubtitlesZipFile);

            var result = await HttpRequester.DownloadAsync(new Uri(subtitleSourceUrl), httpRequestHeaders, false);

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

        public static void SetExtractionFolderLocation(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException("This directory has been found :" + folderPath);

            ExtractionFolder = folderPath;
        }

    }
}
