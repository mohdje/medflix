
namespace WebHostStreaming.Models
{
    public class MediaVideosDto(string title, int? year, string coverImageUrl, bool isSerie, VideoInfo[] videos)
    {
        public string Title { get; set; } = title;
        public int? Year { get; set; } = year;
        public string CoverImageUrl { get; set; } = coverImageUrl;
        public bool IsSerie { get; set; } = isSerie;
        public bool IsMovie { get; set; } = !isSerie;
        public VideoInfo[] Videos { get; set; } = videos;
    }
}