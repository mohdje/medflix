namespace MoviesAPI.Services.Torrent
{
    internal abstract class TorrentWebScapperRequest
    {
        public string MediaName { get; }
        public abstract string[] MediaSearchIdentifiers { get; }

        protected bool FrenchVersion { get; }

        public abstract bool MatchWithTorrentTitle(string title);

        protected TorrentWebScapperRequest(string mediaName, bool searchFrenchVersion)
        {
            MediaName = mediaName;
            FrenchVersion = searchFrenchVersion;
        }
    }
}
