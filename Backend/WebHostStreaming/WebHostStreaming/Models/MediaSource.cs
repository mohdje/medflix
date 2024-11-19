using System.IO;

namespace WebHostStreaming.Models
{
    public class MediaSource
    {
        public string Quality { get; set; }
        public string FilePath { get; set; }
        public string TorrentUrl { get; set; }
    }
}
