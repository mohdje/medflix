using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Models
{
    public class StreamDto
    {
        public Stream Stream { get; }
        public string ContentType { get; set; }
        public StreamDto(Stream stream, string videoFormat)
        {
            Stream = stream;
            ContentType = GetContentType(videoFormat);
        }

        private string GetContentType(string videoFormat)
        {
            if (videoFormat == ".mp4")
                return "video/mp4";
            else if (videoFormat == ".avi")
                return "video/x-msvideo";
            else if (videoFormat == ".mkv")
                return "video/x-matroska";
            else
                return null;
        }
    }
}
