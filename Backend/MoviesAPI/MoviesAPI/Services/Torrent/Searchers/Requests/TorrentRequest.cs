namespace MoviesAPI.Services.Torrent
{
    internal class TorrentRequest
    {
        public string MediaName { get; set; }
        public int Year { get; set; }
        public string ImdbId { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
    }
}
