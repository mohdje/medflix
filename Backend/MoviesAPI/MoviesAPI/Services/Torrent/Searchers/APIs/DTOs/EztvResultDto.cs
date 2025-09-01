using System.Collections.Generic;

namespace MoviesAPI.Services.Torrent
{
    internal class EztvResultDto
    {
        public List<EztvTorrentDto> Torrents { get; set; }
    }

    internal class EztvTorrentDto
    {
        public string Title { get; set; }
        public string MagnetUrl { get; set; }
        public string TorrentUrl { get; set; }
        public string Season { get; set; }
        public int SeasonInt => int.TryParse(Season, out var season) ? season : -1;
        public string Episode { get; set; }
        public int EpisodeInt => int.TryParse(Episode, out var episode) ? episode : -1;
        public int Seeds { get; set; }
        public int Peers { get; set; }
    }
}
