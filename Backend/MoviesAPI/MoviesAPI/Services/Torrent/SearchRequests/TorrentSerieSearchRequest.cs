using MoviesAPI.Extensions;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentSerieSearchRequest : TorrentSearchRequest
    {
        public int EpisodeNumber { get; }
        public int SeasonNumber { get; }

        public TorrentSerieSearchRequest(string serieName, int episodeNumber, int seasonNumber, bool searchFrenchVersion) : base(serieName, searchFrenchVersion)
        {
            EpisodeNumber = episodeNumber;
            SeasonNumber = seasonNumber;
        }
        public override bool MatchWithTorrentTitle(string title)
        {
            var seasonId = $"S{SeasonNumber.ToString("D2")}";
            var episodeId = $"E{EpisodeNumber.ToString("D2")}";

            return (title.CustomStartsWith($"{MediaName} Saison {SeasonNumber}") || title.CustomStartsWith($"{MediaName} {seasonId}{episodeId}"))
                   && (!FrenchVersion || title.Contains("FRENCH") || title.Contains("TRUEFRENCH"))
                   && !title.Contains("VOSTFR");
        }
    }
}
