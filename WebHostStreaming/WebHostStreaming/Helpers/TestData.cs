using MoviesAPI.Services.CommonDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public static class TestData
    {     
        public static string ParadiseCanyonTorrentUrl => "https://archive.org/download/ParadiseCanyon/ParadiseCanyon_archive.torrent";
        public static string JungleBookTorrentUrl => "https://archive.org/download/JungleBook/JungleBook_archive.torrent";
        public static System.IO.Stream SunriseMexicoVideoStream => new System.IO.FileStream(@"C:\Users\mohamed\Desktop\Projects\Streaming\data\Sunrise In Empty Plaza del Zócalo In Mexico City.mp4", System.IO.FileMode.Open);
    
        public static IEnumerable<SubtitlesDto> GetSubtitlesDto()
        {
            var subtitles = new List<SubtitlesDto>();
            subtitles.Add(new SubtitlesDto()
            {
                StartTime = 0.5,
                EndTime = 2.5,
                Text = "On voit le feu vert"
            });
            subtitles.Add(new SubtitlesDto()
            {
                StartTime = 2,
                EndTime = 4,
                Text = "On le voit plus"
            });

            subtitles.Add(new SubtitlesDto()
            {
                StartTime = 14,
                EndTime = 16,
                Text = "Un mec graille à droite"
            });
            subtitles.Add(new SubtitlesDto()
            {
                StartTime = 16,
                EndTime = 18,
                Text = "On le voit plus"
            });

            return subtitles;
        }
    }
}
