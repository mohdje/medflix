using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebHostStreaming.Models;
using MediaTypeHeaderValue = Microsoft.Net.Http.Headers.MediaTypeHeaderValue;

namespace WebHostStreaming.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string MEDFLIX_CLIENT_IDENTIFIER_PREFIX = "MEDFLIX_CLIENT";
        public static string GetClientAppIdentifier(this HttpRequest request)
        {
            if (request.Headers.TryGetValue("User-Agent", out var userAgent))
            {
                var userAgentClientAppIdientifier = userAgent.ToString().Split(" ").FirstOrDefault();

                if (!string.IsNullOrEmpty(userAgentClientAppIdientifier) && userAgentClientAppIdientifier.StartsWith(MEDFLIX_CLIENT_IDENTIFIER_PREFIX))
                    return userAgentClientAppIdientifier.Replace(MEDFLIX_CLIENT_IDENTIFIER_PREFIX, string.Empty);
            }

            return null;
        }

        public static MultipartReader GetMultipartReader(this HttpRequest request)
        {
            if (!request.HasFormContentType || !request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("No multipart/form-data header");
            var boundary = HeaderUtilities.RemoveQuotes(MediaTypeHeaderValue.Parse(request.ContentType).Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
                throw new ArgumentException("Missing content-type boundary");

            return new MultipartReader(boundary, request.Body);
        }

        public static async Task<VideoInfo> GetVideoInfoAsync(this MultipartReader multipartReader)
        {
            VideoInfo videoInfo = new VideoInfo();
            var counter = 0;
            for (int i = 1; i <= 5; i++)
            {
                var section = await multipartReader.ReadNextSectionAsync();

                if (section == null)
                    throw new ArgumentException("A section in multipart/form-data is missing");

                var sectionName = section.GetContentDispositionHeader()?.Name.Value;

                if (sectionName == "mediaId")
                {
                    var data = await section.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data))
                        throw new ArgumentException("mediaId section is empty");

                    videoInfo.MediaId = data;
                    counter++;
                }
                else if (sectionName == "languageVersion")
                {
                    var data = await section.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data) || !int.TryParse(data, out int languageVersion))
                        throw new ArgumentException("languageVersion section is empty or invalid");
                    videoInfo.Language = (LanguageVersion)languageVersion;
                    counter++;
                }
                else if (sectionName == "mediaQuality")
                {
                    var data = await section.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data))
                        throw new ArgumentException("mediaQuality section is empty");
                    videoInfo.Quality = data;
                    counter++;
                }
                else if (sectionName == "seasonNumber")
                {
                    var data = await section.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data) || !int.TryParse(data, out int seasonNumber))
                        throw new ArgumentException("seasonNumber section is empty or invalid");
                    videoInfo.SeasonNumber = seasonNumber;
                    counter++;
                }
                else if (sectionName == "episodeNumber")
                {
                    var data = await section.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(data) || !int.TryParse(data, out int episodeNumber))
                        throw new ArgumentException("episodeNumber section is empty or invalid");
                    videoInfo.EpisodeNumber = episodeNumber;
                    counter++;
                }
            }

            if (counter < 5)
                throw new ArgumentException("Not all required sections are present in the request");

            return videoInfo;
        }
    }
}
