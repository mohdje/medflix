using HtmlAgilityPack;
using MoviesAPI.Extensions;

namespace MoviesAPI.Services.Torrent
{
    internal class YggTorrentScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www-torrent911.com";
        protected override string SearchResultListIdentifier => "//td";
        protected override string TorrentLinkButtonsIdentifier => "//div[@class='btn-download' or @class='btn-magnet']/a";
        protected override string MediaQualityIdentifier => "//div[@id='torrentsdesc']//div[@class='maximum']";
        protected override string TorrentLinkPageIdentifier => "//a";
        protected override bool FrenchVersion => true;
        protected override bool CheckQuality => true;

        protected override string BuildSearchUrl(TorrentSearchRequest torrentSearchRequest)
        {
            return $"{Url}/recherche/{torrentSearchRequest.MediaName.RemoveSpecialCharacters()}";
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            var titleNode = htmlNode.DocumentNode.SelectSingleNode("//a");
            return titleNode?.InnerText.Trim();
        }
    }
}
