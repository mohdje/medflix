using MoviesAPI.Extensions;
using System.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentSerieSearchRequest : TorrentSearchRequest
    {
        public int EpisodeNumber { get; }
        public int SeasonNumber { get; }

        public override string[] MediaSearchIdentifiers => [$"{MediaName} S{SeasonNumber.ToString("D2")}E{EpisodeNumber.ToString("D2")}", $"{MediaName} {(FrenchVersion ? "Saison" : "Season")} {SeasonNumber}"];

        public TorrentSerieSearchRequest(string serieName, int episodeNumber, int seasonNumber, bool searchFrenchVersion) : base(serieName, searchFrenchVersion)
        {
            EpisodeNumber = episodeNumber;
            SeasonNumber = seasonNumber;
        }
        public override bool MatchWithTorrentTitle(string title)
        {
            return MediaSearchIdentifiers.Any(mediaId => title.CustomStartsWith(mediaId))
                   && (!FrenchVersion || title.Contains("FRENCH") || title.Contains("TRUEFRENCH"))
                   && !title.Contains("VOSTFR");
        }
    }
}
