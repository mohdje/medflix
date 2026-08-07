using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebHostStreaming.Models;

namespace WebHostStreaming.Converters
{
    public class SubtitlesConverter : ISubtitlesConverter
    {
        public async Task<IEnumerable<SubtitlesDto>> ToSubtitlesDtoAsync(string subtitlesFilePath)
        {
            var extension = System.IO.Path.GetExtension(subtitlesFilePath).ToLowerInvariant();
            if (extension == ".srt")
            {
                return await ParseSrtFile(subtitlesFilePath);
            }
            if (extension == ".vtt")
            {
                return await ParseVttFile(subtitlesFilePath);
            }
            if (extension == ".sub")
            {
                return await ParseSubFile(subtitlesFilePath);
            }
            if (extension == ".ass")
            {
                return await ParseAssFile(subtitlesFilePath);
            }

            return [];
        }

        private async Task<IEnumerable<SubtitlesDto>> ParseSrtFile(string subtitlesFilePath)
        {
            var subtitlesDtos = new List<SubtitlesDto>();
            var lines = await System.IO.File.ReadAllLinesAsync(subtitlesFilePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                // Skip the subtitle index line
                i++;

                var timecodeLine = lines[i];
                var timecodes = timecodeLine.Split(" --> ");
                if (timecodes.Length != 2)
                    throw new FormatException($"Invalid timecode format in line: {timecodeLine}");

                var subtitlesDto = new SubtitlesDto();
                subtitlesDto.StartTime = TimeSpan.Parse(timecodes[0]).TotalSeconds;
                subtitlesDto.EndTime = TimeSpan.Parse(timecodes[1]).TotalSeconds;

                i++;
                var subtitleText = string.Empty;
                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    subtitleText += lines[i] + Environment.NewLine;
                    i++;
                }

                subtitlesDto.Text = subtitleText.Trim();
                subtitlesDtos.Add(subtitlesDto);
            }

            return subtitlesDtos;
        }

        private async Task<IEnumerable<SubtitlesDto>> ParseVttFile(string subtitlesFilePath)
        {
            var subtitlesDtos = new List<SubtitlesDto>();
            var lines = await System.IO.File.ReadAllLinesAsync(subtitlesFilePath);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.Equals("WEBVTT", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (line.StartsWith("NOTE", StringComparison.OrdinalIgnoreCase))
                {
                    while (i + 1 < lines.Length && !string.IsNullOrWhiteSpace(lines[i + 1]))
                    {
                        i++;
                    }

                    continue;
                }

                if (!line.Contains("-->", StringComparison.Ordinal))
                    continue;

                var timecodeLine = line;
                var timecodes = timecodeLine.Split(new[] { "-->" }, StringSplitOptions.None);
                if (timecodes.Length != 2)
                    throw new FormatException($"Invalid timecode format in line: {timecodeLine}");

                var subtitlesDto = new SubtitlesDto();
                var startTime = timecodes[0].Trim();
                var endTime = timecodes[1].Trim();
                var endTimeSeparator = endTime.IndexOf(' ');
                if (endTimeSeparator >= 0)
                {
                    endTime = endTime.Substring(0, endTimeSeparator);
                }

                subtitlesDto.StartTime = ParseTimeToSeconds(startTime);
                subtitlesDto.EndTime = ParseTimeToSeconds(endTime);

                var subtitleTextLines = new List<string>();
                i++;
                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    subtitleTextLines.Add(lines[i].TrimEnd());
                    i++;
                }

                subtitlesDto.Text = string.Join(Environment.NewLine, subtitleTextLines).Trim();
                subtitlesDtos.Add(subtitlesDto);
            }

            return subtitlesDtos;
        }

        private async Task<IEnumerable<SubtitlesDto>> ParseSubFile(string subtitlesFilePath)
        {
            var subtitlesDtos = new List<SubtitlesDto>();
            var lines = await System.IO.File.ReadAllLinesAsync(subtitlesFilePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var subtitle = ParseMicrodvdCue(line);
                if (subtitle != null)
                {
                    subtitlesDtos.Add(subtitle);
                }
            }

            return subtitlesDtos;
        }

        private static SubtitlesDto ParseMicrodvdCue(string line)
        {
            var match = Regex.Match(line.Trim(), @"^\{(?<start>-?\d+(?:\.\d+)?)\}\{(?<end>-?\d+(?:\.\d+)?)\}(?<text>.*)$");
            if (!match.Success)
            {
                return null;
            }

            if (!double.TryParse(match.Groups["start"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var startFrames) ||
                !double.TryParse(match.Groups["end"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var endFrames))
            {
                return null;
            }

            return new SubtitlesDto
            {
                StartTime = startFrames / 25.0,
                EndTime = endFrames / 25.0,
                Text = match.Groups["text"].Value.Trim().Replace("|", Environment.NewLine)
            };
        }

        private async Task<IEnumerable<SubtitlesDto>> ParseAssFile(string subtitlesFilePath)
        {
            var subtitlesDtos = new List<SubtitlesDto>();
            var lines = await System.IO.File.ReadAllLinesAsync(subtitlesFilePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("Dialogue:", StringComparison.OrdinalIgnoreCase))
                    continue;

                var subtitle = ParseAssDialogue(line);
                if (subtitle != null)
                {
                    subtitlesDtos.Add(subtitle);
                }
            }

            return subtitlesDtos;
        }

        private static SubtitlesDto ParseAssDialogue(string line)
        {
            var match = Regex.Match(
                line,
                @"^Dialogue:\s*(?<layer>[^,]+),(?<start>[^,]+),(?<end>[^,]+),(?<style>[^,]*),(?<name>[^,]*),(?<marginL>[^,]*),(?<marginR>[^,]*),(?<marginV>[^,]*),(?<effect>[^,]*),(?<text>.*)$");

            if (!match.Success)
            {
                return null;
            }

            if (!TryParseAssTime(match.Groups["start"].Value, out var startTime) ||
                !TryParseAssTime(match.Groups["end"].Value, out var endTime))
            {
                return null;
            }

            var text = match.Groups["text"].Value;
            text = Regex.Replace(text, @"\{[^{}]*\}", string.Empty);
            text = text.Replace("\\N", Environment.NewLine);
            text = text.Replace("\\n", Environment.NewLine);
            text = text.Replace("\\h", " ");
            text = text.Trim();

            return new SubtitlesDto
            {
                StartTime = startTime,
                EndTime = endTime,
                Text = text
            };
        }

        private static bool TryParseAssTime(string value, out double seconds)
        {
            seconds = 0;
            var normalizedValue = value.Trim();

            if (TimeSpan.TryParse(normalizedValue, out var parsedTime))
            {
                seconds = parsedTime.TotalSeconds;
                return true;
            }

            var parts = normalizedValue.Split(':');
            if (parts.Length != 3)
            {
                return false;
            }

            var secondsParts = parts[2].Split('.');
            if (secondsParts.Length > 2)
            {
                return false;
            }

            if (!int.TryParse(parts[0], out var hours) ||
                !int.TryParse(parts[1], out var minutes) ||
                !int.TryParse(secondsParts[0], out var secondsValue))
            {
                return false;
            }

            var milliseconds = secondsParts.Length > 1 ? int.Parse(secondsParts[1].PadRight(3, '0')) : 0;
            seconds = new TimeSpan(0, hours, minutes, secondsValue, milliseconds).TotalSeconds;
            return true;
        }

        private static double ParseTimeToSeconds(string value)
        {
            var normalizedValue = value.Trim().Replace(',', '.');

            if (TimeSpan.TryParse(normalizedValue, out var parsedTime))
            {
                return parsedTime.TotalSeconds;
            }

            var parts = normalizedValue.Split(':');
            if (parts.Length != 3)
            {
                throw new FormatException($"Invalid timestamp format: {value}");
            }

            var secondsParts = parts[2].Split('.');
            if (secondsParts.Length > 2)
            {
                throw new FormatException($"Invalid timestamp format: {value}");
            }

            var hours = int.Parse(parts[0]);
            var minutes = int.Parse(parts[1]);
            var seconds = int.Parse(secondsParts[0]);
            var milliseconds = secondsParts.Length > 1 ? int.Parse(secondsParts[1].PadRight(3, '0')) : 0;

            return new TimeSpan(0, hours, minutes, seconds, milliseconds).TotalSeconds;
        }
    }
}
