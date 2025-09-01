using MoviesAPI.Extensions;
using System.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentSerieWebScrapperRequest : TorrentWebScapperRequest
    {
        public int EpisodeNumber { get; }
        public int SeasonNumber { get; }

        public string EpisodeIdentifier => $"S{SeasonNumber.ToString("D2")}E{EpisodeNumber.ToString("D2")}";
        public string SeasonIdentifier => $"{(FrenchVersion ? "Saison" : "Season")} {SeasonNumber}";
        public override string[] MediaSearchIdentifiers => [$"{MediaName} {EpisodeIdentifier}", $"{MediaName} {SeasonIdentifier}"];

        public TorrentSerieWebScrapperRequest(string serieName, int episodeNumber, int seasonNumber, bool searchFrenchVersion) : base(serieName, searchFrenchVersion)
        {
            EpisodeNumber = episodeNumber;
            SeasonNumber = seasonNumber;
        }
        public override bool MatchWithTorrentTitle(string title)
        {
            return title.StartsWithIgnoreDiactrics(this.MediaName)
                   && (title.Contains(EpisodeIdentifier) || title.Contains(SeasonIdentifier))
                   && (!FrenchVersion || title.Contains("FRENCH") || title.Contains("TRUEFRENCH"))
                   && !title.Contains("VOSTFR");
        }
    }
}
