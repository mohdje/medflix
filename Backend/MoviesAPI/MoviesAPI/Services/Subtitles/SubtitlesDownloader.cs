using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MoviesAPI.Services.Subtitles.DTOs;

namespace MoviesAPI.Services.Subtitles
{
    internal class SubtitlesDownloader
    {
        private readonly string ExtractionFolder;
        private static readonly Regex _rgxCueID = new Regex(@"^\d+$");
        private static readonly Regex _rgxTimeFrame = new Regex(@"(\d\d:\d\d:\d\d(?:[,.]\d\d\d)?) --> (\d\d:\d\d:\d\d(?:[,.]\d\d\d)?)");

        internal SubtitlesDownloader(string extractionFolder) : base()
        {
            if (string.IsNullOrEmpty(extractionFolder))
                throw new Exception("You have to provide an extraction folder");

            ExtractionFolder = extractionFolder;
        }

        public async Task<IEnumerable<SubtitlesDto>> DownloadSubtitlesAsync(string subtitlesSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders = null)
        {
            var subtitlesFile = await GetSubtitlesFileAsync(subtitlesSourceUrl, httpRequestHeaders);

            if (!string.IsNullOrEmpty(subtitlesFile))
            {
                var subtitles = GetSubtitles(subtitlesFile);

                if (File.Exists(subtitlesFile))
                    File.Delete(subtitlesFile);

                return subtitles;
            }
            else 
                return Array.Empty<SubtitlesDto>();
        }

        private async Task<string> GetSubtitlesFileAsync(string subtitlesSourceUrl, IEnumerable<KeyValuePair<string, string>> httpRequestHeaders)
        {
            var subtitlesZipFile = Path.Combine(ExtractionFolder, $"subtitles_{DateTime.Now.Ticks}.zip");
            var extractedFile = Path.Combine(ExtractionFolder, $"subtitles_{DateTime.Now.Ticks}.srt");

            try
            {
                var result = await HttpRequester.DownloadAsync(new Uri(subtitlesSourceUrl), httpRequestHeaders);

                if(result != null && result.Any())
                    File.WriteAllBytes(subtitlesZipFile, result);

                if (!File.Exists(subtitlesZipFile))
                    throw new FileNotFoundException($"{subtitlesZipFile} not found");

                using (ZipArchive archive = ZipFile.OpenRead(subtitlesZipFile))
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
            catch(Exception ex)
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

        private IEnumerable<SubtitlesDto> GetSubtitles(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return Array.Empty<SubtitlesDto>();

            using (var srtReader = new StreamReader(filePath, Encoding.UTF8))
            {
                var subtitlesDtos = new List<SubtitlesDto>();

                string srtLine;
                while ((srtLine = srtReader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(srtLine) || _rgxCueID.IsMatch(srtLine)) // Ignore cue ID number lines                
                        continue;

                    Match match = _rgxTimeFrame.Match(srtLine);
                    if (match.Success) //If line with start time and end time
                    {
                        var subtitlesDto = new SubtitlesDto();

                        var startTime = TimeSpan.Parse(match.Groups[1].Value.Replace(',', '.'));
                        var endTime = TimeSpan.Parse(match.Groups[2].Value.Replace(',', '.'));
                        subtitlesDto.StartTime = startTime.TotalSeconds;
                        subtitlesDto.EndTime = endTime.TotalSeconds;

                        subtitlesDtos.Add(subtitlesDto);
                    }
                    else
                    {
                        if (subtitlesDtos.Count > 0)
                        {
                            subtitlesDtos[subtitlesDtos.Count - 1].Text =
                                string.IsNullOrEmpty(subtitlesDtos[subtitlesDtos.Count - 1].Text) ?
                                srtLine : subtitlesDtos[subtitlesDtos.Count - 1].Text + " " + srtLine;
                        }

                    }
                }

                return subtitlesDtos;
            }
        }
    }
}
