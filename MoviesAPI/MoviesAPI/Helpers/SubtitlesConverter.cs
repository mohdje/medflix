using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MoviesAPI.Helpers
{
   
    internal class SubtitlesConverter
    {
        private static readonly Regex _rgxCueID = new Regex(@"^\d+$");
        private static readonly Regex _rgxTimeFrame = new Regex(@"(\d\d:\d\d:\d\d(?:[,.]\d\d\d)?) --> (\d\d:\d\d:\d\d(?:[,.]\d\d\d)?)");

        public static IEnumerable<SubtitlesDto> GetSubtitles(string filePath)
        {
            using (var srtReader = new StreamReader(filePath, Encoding.GetEncoding("iso-8859-1")))
            {
                var subtitlesDtos = new List<SubtitlesDto>();

                string srtLine;
                while ((srtLine = srtReader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(srtLine) ||_rgxCueID.IsMatch(srtLine)) // Ignore cue ID number lines                
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
                            subtitlesDtos[subtitlesDtos.Count - 1].Text = srtLine;
                    }
                }

                return subtitlesDtos;
            }
        }
    }
}
