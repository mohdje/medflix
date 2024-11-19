using Medflix.Models.Media;
using Medflix.Utils;
using System.Text.Json.Serialization;

namespace Medflix.Models.Media
{
    public class WatchMediaInfo
    {
        public MediaDetails Media { get; set; }
        public float TotalDuration { get; set; }
        public float CurrentTime { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string VideoSource { get; set; }

        [JsonIgnore]
        public double Progress => (double)CurrentTime / TotalDuration;
        [JsonIgnore]
        public string Type => Media.SeasonsCount > 0 ? Consts.Series : Consts.Movies;
    }
}
