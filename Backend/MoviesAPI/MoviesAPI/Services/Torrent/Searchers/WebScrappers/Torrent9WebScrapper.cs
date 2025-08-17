using HtmlAgilityPack;
using System.Linq;


namespace MoviesAPI.Services.Torrent.Searchers.WebScrappers
{
    internal class Torrent9WebScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www.torrent9.diy";

        protected override string SearchResultListIdentifier => "//table[@class='table table-striped table-bordered cust-table -table']//td[a]";

        protected override string TorrentLinkPageIdentifier => "//a";

        protected override string TorrentLinkButtonsIdentifier => "//div[@class='download-btn']/a[starts-with(@href, 'magnet')]";

        protected override string MediaQualityIdentifier => "//div[@class='movie-section']//h1";

        protected override bool FrenchVersion => true;

        protected override bool CheckQuality => true;

        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/recherche/{mediaSearchId}").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument torrentResultNode)
        {
            return torrentResultNode.DocumentNode.InnerText.Trim();
        }

        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            return true;
        }
    }
}
