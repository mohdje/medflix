using HtmlAgilityPack;
using System.Linq;

namespace MoviesAPI.Services.Torrent.Searchers.WebScrappers
{
    internal class LimeTorrentsScrapper : TorrentWebScrapper
    {
        public override string Url => "http://www.limetorrents.lol";

        protected override string SearchResultListIdentifier => "//table[@class='table2']//div[@class='tt-name']";

        protected override string TorrentLinkPageIdentifier => "//a[not(contains(@rel, 'nofollow'))]";

        protected override string TorrentLinkButtonsIdentifier => "//div[@class='dltorrent']/a";

        protected override string MediaQualityIdentifier => "//div[@id='content']/h1";

        protected override bool FrenchVersion => false;

        protected override bool CheckQuality => true;

        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/search/tv/{mediaSearchId}").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            return htmlNode?.DocumentNode.InnerText.Trim();
        }

        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            var specsNode = torrentHtmlPage.DocumentNode.SelectNodes("//span[@class='greenish']");
            var textIdentifier = "Seeders :";
            var seedersNode = specsNode.FirstOrDefault(node => node.InnerText.Contains(textIdentifier));
            if (seedersNode != null && int.TryParse(seedersNode?.InnerText.Replace(textIdentifier, string.Empty).Trim(), out var nbSeeders))
                return nbSeeders > 0;
            else
                return false;
        }
    }
}
