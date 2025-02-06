using HtmlAgilityPack;
using MoviesAPI.Extensions;
using System.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class YtsVfTorrentScrapper : TorrentWebScrapper
    {
        public override string Url => "https://yts.com.mx";

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
    }
}
