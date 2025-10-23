using HtmlAgilityPack;
using System.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class ZeTorrentsScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www.zetorrents2.com";
        protected override string SearchResultListIdentifier => "//div[@class='content-list-torrent']//div[@class='maxi']";
        protected override string TorrentLinkButtonsIdentifier => "//div[@class='btn-download']/a";
        protected override bool FrenchVersion => true;
        protected override bool CheckQuality => true;
        protected override string MediaQualityIdentifier => "//div[@id='torrentsdesc']//div[@class='maximum']";
        protected override string TorrentLinkPageIdentifier => "//a";

        protected override string[] GetSearchUrls(TorrentWebScapperRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/recherche/{mediaSearchId}").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument torrentHtmlPage)
        {
            var titleNode = torrentHtmlPage.DocumentNode.SelectSingleNode("//a");
            return titleNode?.InnerText.Trim();
        }

        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            var seedersNode = torrentHtmlPage.DocumentNode.SelectSingleNode("//font[@id='retourSeeds']");

            if (int.TryParse(seedersNode?.InnerText.Trim(), out var nbSeeders))
                return nbSeeders > 0;
            else
                return false;
        }
    }
}
