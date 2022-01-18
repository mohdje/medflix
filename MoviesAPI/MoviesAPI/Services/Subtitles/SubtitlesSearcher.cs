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
        private string ExtractionFolder => Path.Combine(Environment.CurrentDirectory, "subtitles");

        private string SubtitlesZipFile => Path.Combine(ExtractionFolder, "subtitles.zip");


        internal SubtitlesSearcher() : base()
        {
            if (!Directory.Exists(ExtractionFolder))
                Directory.CreateDirectory(ExtractionFolder);
        }
       
        public abstract Task<SubtitlesSearchResultDto> GetAvailableSubtitlesAsync(string imdbCode, SubtitlesLanguage subtitlesLanguage);

        public abstract IEnumerable<SubtitlesDto> GetSubtitles(string subtitlesSourceUrl);

        protected abstract string GetLanguageCode(SubtitlesLanguage subtitlesLanguage);

        protected abstract string GetLanguageLabel(SubtitlesLanguage subtitlesLanguage);

        protected void DownloadSubtitlesZipFile(string subtitleSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders)
        {         
            if (File.Exists(SubtitlesZipFile))
                File.Delete(SubtitlesZipFile);

            var result = HttpRequester.DownloadAsync(new Uri(subtitleSourceUrl), httpRequestHeaders, false).Result;

            File.WriteAllBytes(SubtitlesZipFile, result);
        }

        protected string GetSubtitlesFile()
        {
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
