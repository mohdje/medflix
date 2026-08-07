using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MoviesAPI.Services.Subtitles
{
    internal class SubtitlesDownloader
    {
        private readonly string ExtractionFolder;
        internal SubtitlesDownloader(string extractionFolder) : base()
        {
            if (string.IsNullOrEmpty(extractionFolder))
                throw new Exception("You have to provide an extraction folder");

            ExtractionFolder = extractionFolder;
        }

        public async Task<string> DownloadSubtitlesFileAsync(string subtitlesSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders = null)
        {
            return await GetSubtitlesFileAsync(subtitlesSourceUrl, httpRequestHeaders);
        }

        private async Task<string> GetSubtitlesFileAsync(string subtitlesSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders, string[] supportedExtensions = null)
        {
            var subtitlesZipFile = Path.Combine(ExtractionFolder, $"subtitles_{DateTime.Now.Ticks}.zip");
            supportedExtensions ??= [".srt", ".vtt", ".ass", ".ssa", ".sub", ".txt"];

            try
            {
                var result = await HttpRequester.DownloadAsync(new Uri(subtitlesSourceUrl), httpRequestHeaders);

                if (result != null && result.Any())
                    File.WriteAllBytes(subtitlesZipFile, result);

                if (!File.Exists(subtitlesZipFile))
                    throw new FileNotFoundException($"{subtitlesZipFile} not found");

                using (ZipArchive archive = ZipFile.OpenRead(subtitlesZipFile))
                {
                    var subtitleFileEntry = archive.Entries.FirstOrDefault(e => supportedExtensions.Contains(Path.GetExtension(e.FullName)));
                    if (subtitleFileEntry != null)
                    {
                        string destinationPath = Path.Combine(ExtractionFolder, $"{DateTime.Now.Ticks}_${subtitleFileEntry.Name}");
                        subtitleFileEntry.ExtractToFile(destinationPath);
                        return destinationPath;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                var errMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                Console.WriteLine($"Error downloading subtitles from {subtitlesSourceUrl}: {errMessage}");
                return null;
            }
            finally
            {
                if (File.Exists(subtitlesZipFile))
                    File.Delete(subtitlesZipFile);
            }
        }
    }
}
