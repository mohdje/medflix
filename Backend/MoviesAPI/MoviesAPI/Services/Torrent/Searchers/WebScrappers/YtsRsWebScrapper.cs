


using HtmlAgilityPack;
using System.Linq;

namespace MoviesAPI.Services.Torrent.Searchers.WebScrappers
{
    internal class YtsRsWebScrapper : TorrentWebScrapper
    {
        public override string Url => "https://yts.rs";

        protected override string SearchResultListIdentifier => "//div[@class='card']";

        protected override string TorrentLinkButtonsIdentifier => "//div[contains(@class, 'torrent-qualities')]//a";

        protected override bool FrenchVersion => false;

        protected override bool CheckQuality => false;

        protected override string MediaQualityIdentifier => null;

        protected override string TorrentLinkPageIdentifier => "//a";

        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/browse-movies/{mediaSearchId}/all/all/0/latest").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            var titleNode = htmlNode.DocumentNode.SelectSingleNode("//a[contains(@class, 'image-container-link')]");
            return titleNode?.Attributes["title"]?.Value.Replace("(", string.Empty).Replace(")", string.Empty);
        }
    }
}
