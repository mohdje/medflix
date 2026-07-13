using HtmlAgilityPack;
using System;
using System.Linq;

namespace MoviesAPI.Services.Torrent.Searchers.WebScrappers
{
    internal class BitSearchScrapper : TorrentWebScrapper
    {
        public override string Url => "https://bitsearch.eu";

        protected override string SearchResultListIdentifier => "//div[@data-impression-ids]/div";

        protected override string TorrentLinkPageIdentifier => "//a[contains(@href, '/torrent/')]";

        protected override string TorrentLinkButtonsIdentifier => "//a[starts-with(@href, 'magnet:') or starts-with(@href, '/download/torrent/')]";

        protected override string MediaQualityIdentifier => "//h1[contains(@class, 'text-lg')]";

        protected override bool FrenchVersion => false;

        protected override bool CheckQuality => true;

        protected override string[] GetSearchUrls(TorrentWebScapperRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/search?q={mediaSearchId}&sortBy=seeders&page=1").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            var titleNode = htmlNode.DocumentNode.SelectSingleNode("//h3");
            return titleNode?.InnerText.Trim();
        }

        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            var specsNodes = torrentHtmlPage.DocumentNode.SelectNodes("//div[@class='bg-white rounded-xl shadow-lg border border-gray-200 overflow-hidden']//div[@class='grid grid-cols-2 sm:grid-cols-4 gap-2 sm:gap-3']//div[@class='text-center p-2 sm:p-3 bg-white bg-opacity-20 rounded-lg backdrop-blur-sm']");
            var textIdentifier = "Seeders";
            var seedersNode = specsNodes.FirstOrDefault(node => node.InnerText.Contains(textIdentifier));

            if (seedersNode != null && int.TryParse(seedersNode?.InnerText.Replace(textIdentifier, string.Empty).Trim(), out var nbSeeders))
                return nbSeeders > 0;
            else
                return false;
        }
    }
}
