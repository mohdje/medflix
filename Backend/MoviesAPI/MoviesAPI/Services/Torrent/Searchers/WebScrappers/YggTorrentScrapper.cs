using HtmlAgilityPack;
using System.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class YggTorrentScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www.zone-torrent1.com";
        protected override string SearchResultListIdentifier => "//td//div[@class='maxi']";
        protected override string TorrentLinkButtonsIdentifier => "//table[@class='table']//a[starts-with(@href, '/get_torrents') or starts-with(@href, 'magnet')]"; 
        protected override string MediaQualityIdentifier => "//div[@id='torrentsdesc']//div[@class='maximum']";
        protected override string TorrentLinkPageIdentifier => "//a";
        protected override bool FrenchVersion => true;
        protected override bool CheckQuality => true;

        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaId => $"{Url}/recherche/{mediaId}").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            var titleNode = htmlNode.DocumentNode.SelectSingleNode("//a");
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
