using HtmlAgilityPack;
using MoviesAPI.Extensions;
using System.Linq;
using System.Web;

namespace MoviesAPI.Services.Torrent
{
    internal abstract class TorrentSearchRequest
    {
        public string MediaName { get; }
        public abstract string[] MediaSearchIdentifiers { get; }

        protected bool FrenchVersion { get; }

        public abstract bool MatchWithTorrentTitle(string title);

        protected TorrentSearchRequest(string mediaName, bool searchFrenchVersion)
        {
            MediaName = mediaName;
            FrenchVersion = searchFrenchVersion;
        }
    }
}
