


using HtmlAgilityPack;
using System.Linq;

namespace MoviesAPI.Services.Torrent.Searchers.WebScrappers
{
    internal class YtsMxWebScrapper : TorrentWebScrapper
    {
        public override string Url => "https://yts.mx";

        protected override string SearchResultListIdentifier => "//div[@class='browse-content']//div[contains(@class, 'browse-movie-wrap')]";

        protected override string TorrentLinkButtonsIdentifier => "//div[@id='movie-info']//a[@rel='nofollow']";

        protected override bool FrenchVersion => false;

        protected override bool CheckQuality => false;

        protected override string MediaQualityIdentifier => null;

        protected override string TorrentLinkPageIdentifier => "//div[@class='browse-movie-bottom']//a";

        protected override string[] GetSearchUrls(TorrentWebScapperRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/browse-movies/{mediaSearchId}/all/all/0/latest/0/all").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            var titleNode = htmlNode.DocumentNode.SelectSingleNode("//div[@class='browse-movie-bottom']");
            return titleNode?.InnerText.Trim();
        }

        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            return true;
        }
    }
}
