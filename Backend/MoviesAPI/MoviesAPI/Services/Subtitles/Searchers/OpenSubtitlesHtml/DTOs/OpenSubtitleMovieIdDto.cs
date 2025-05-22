
using System.Text.Json.Serialization;

namespace MoviesAPI.Services.Subtitles.OpenSubtitlesHtml.DTOs
{
    public class OpenSubtitleMovieIdDto
    {
        [JsonPropertyName("id")]
        public int OpenSubtitleMovieId { get; set; }
    }
}
