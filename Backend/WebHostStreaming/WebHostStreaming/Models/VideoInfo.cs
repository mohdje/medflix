using System.IO;
using System.Text.Json.Serialization;

namespace WebHostStreaming.Models
{
    public class VideoInfo
    {
        private string filePath;
        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                bytesLength = null;
                filePath = value;
            }
        }
        public string MediaId { get; set; }
        public string Quality { get; set; }
        public LanguageVersion Language { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        private long? bytesLength;
        public long? BytesLength
        {
            get
            {
                if (!bytesLength.HasValue && !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
                    bytesLength = new FileInfo(FilePath).Length;

                return bytesLength;
            }
        }
        public string Id => $"{MediaId}_{Language}_{SeasonNumber}_{EpisodeNumber}_{Quality}";
        public bool IsSerie => SeasonNumber > 0 && EpisodeNumber > 0;
    }
}