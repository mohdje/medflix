using MoviesAPI.Extensions;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentMovieSearchRequest : TorrentSearchRequest
    {
        public int Year { get; }

        bool checkQuality;

        public TorrentMovieSearchRequest(string movieName, int year, bool checkQuality, bool searchFrenchVersion = false) : base(movieName, searchFrenchVersion)
        {
            Year = year;
            this.checkQuality = checkQuality;
        }

        public override bool MatchWithTorrentTitle(string torrentTitle)
        {
            return torrentTitle.CustomStartsWith(MediaName)
                    && (!FrenchVersion || torrentTitle.Contains("FRENCH") || torrentTitle.Contains("TRUEFRENCH") || torrentTitle.Contains("MULTI"))
                    && torrentTitle.EndsWith(Year.ToString())
                    && !torrentTitle.Contains("MD")
                    && !torrentTitle.Contains("VOSTFR")
                    && (!checkQuality || torrentTitle.Contains("720p") || torrentTitle.Contains("1080p") || torrentTitle.Contains("DVDRIP") || torrentTitle.Contains("WEBRIP"));
        }
    }
}
