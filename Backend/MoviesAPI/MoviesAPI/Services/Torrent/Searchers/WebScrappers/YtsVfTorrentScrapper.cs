using HtmlAgilityPack;
using System.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class YtsVfTorrentScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www-yts.com";

        protected override string SearchResultListIdentifier => "//div[@class='browse-movie-bottom']";

        protected override string TorrentLinkButtonsIdentifier => "//div[contains(@class, 'torrent-modal-download')]/a";

        protected override bool FrenchVersion => true;

        protected override bool CheckQuality => true;

        protected override string MediaQualityIdentifier => "//div[@id='movie-info']//h1";

        protected override string TorrentLinkPageIdentifier => "//a";

        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/recherche/{mediaSearchId}").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            var titleNode = htmlNode.DocumentNode.SelectSingleNode("//a");
            return titleNode?.InnerText.Trim();
        }

        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            var specsNode = torrentHtmlPage.DocumentNode.SelectNodes("//div[contains(@class, 'tech-spec-element')]");
            var textIdentifier = "Seeders:";
            var seedersNode = specsNode.FirstOrDefault(node => node.InnerText.Contains(textIdentifier));
            if (seedersNode != null && int.TryParse(seedersNode?.InnerText.Replace(textIdentifier, string.Empty).Trim(), out var nbSeeders))
                return nbSeeders > 0;
            else 
                return false;
        }
    }
}
