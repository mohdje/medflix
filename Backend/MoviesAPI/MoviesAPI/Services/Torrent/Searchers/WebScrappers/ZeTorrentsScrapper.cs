using HtmlAgilityPack;
using MoviesAPI.Extensions;

namespace MoviesAPI.Services.Torrent
{
    internal class ZeTorrentsScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www.zetorrents.my";
        protected override string SearchResultListIdentifier => "//div[@class='content-list-torrent']//div[@class='maxi']";
        protected override string TorrentLinkButtonsIdentifier => "//div[@class='btn-download']/a";
        protected override bool FrenchVersion => true;
        protected override bool CheckQuality => true;
        protected override string MediaQualityIdentifier => "//div[@id='torrentsdesc']//div[@class='maximum']";
        protected override string TorrentLinkPageIdentifier => "//a";

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
