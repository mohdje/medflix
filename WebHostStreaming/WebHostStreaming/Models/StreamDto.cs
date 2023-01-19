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
        public string ContentType => GetContentType();
        public string FilePath { get; }
        public StreamDto(Stream stream, string filePath)
        {
            Stream = stream;
            FilePath = filePath;
        }

        private string GetContentType()
        {
            var videoFormat = Path.GetExtension(FilePath);

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
