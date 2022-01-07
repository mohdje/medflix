using MoviesAPI.Services.CommonDtos;
using MoviesAPI.Services.Subtitles.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostStreaming.Helpers
{
    public static class TestData
    {     
        public static string ParadiseCanyonTorrentUrl => "https://archive.org/download/ParadiseCanyon/ParadiseCanyon_archive.torrent";
        public static string ParadiseCanyonMagnetLink => "magnet:?xt=urn:btih:UU4TU7WI7HGPACAR3JUA3FZXRCTGG2ZM&dn=ParadiseCanyon&tr=http%3A%2F%2Fbt1.archive.org%3A6969%2Fannounce";
        public static string JungleBookTorrentUrl => "https://archive.org/download/JungleBook/JungleBook_archive.torrent";
        public static System.IO.Stream SunriseMexicoVideoStream => new System.IO.FileStream(@"D:\Projets\Streaming\test\data\Sunrise In Empty Plaza del Zócalo In Mexico City.mp4", System.IO.FileMode.Open);
    
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
