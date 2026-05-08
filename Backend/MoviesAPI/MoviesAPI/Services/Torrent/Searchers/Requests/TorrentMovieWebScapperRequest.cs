using MoviesAPI.Extensions;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentMovieWebScapperRequest : TorrentWebScapperRequest
    {
        public int Year { get; }

        public override string[] MediaSearchIdentifiers => [MediaName.RemoveDiacritics()];

        bool checkQuality;

        public TorrentMovieWebScapperRequest(string movieName, int year, bool checkQuality, bool searchFrenchVersion = false) : base(movieName, searchFrenchVersion)
        {
            Year = year;
            this.checkQuality = checkQuality;
        }

        public override bool MatchWithTorrentTitle(string torrentTitle)
        {
            return torrentTitle.StartsWithIgnoreDiactrics(MediaName)
                    && (!FrenchVersion || torrentTitle.Contains("FRENCH") || torrentTitle.Contains("TRUEFRENCH") || torrentTitle.Contains("MULTI") || torrentTitle.Contains("MULTi"))
                    && torrentTitle.Contains(Year.ToString())
                    && !torrentTitle.Contains("MD")
                    && !torrentTitle.Contains("VOSTFR")
                    && !torrentTitle.Contains("2160p")
                    && (!checkQuality || torrentTitle.Contains("720p") || torrentTitle.Contains("1080p") || torrentTitle.Contains("DVDRIP") || torrentTitle.Contains("WEBRIP") || torrentTitle.Contains("WEB"));
        }
    }
}
