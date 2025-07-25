
namespace WebHostStreaming.Models
{
    public partial class VideoInfo
    {
        public string FilePath { get; set; }
        public string MediaId { get; set; }
        public string Quality { get; set; }
        public LanguageVersion Language { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string Id => $"{MediaId}_{Language}_{SeasonNumber}_{EpisodeNumber}_{Quality}";
    }
}
