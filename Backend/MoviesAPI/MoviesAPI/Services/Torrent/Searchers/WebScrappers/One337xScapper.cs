using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent.Searchers.WebScrappers
{
    internal class One337xScapper : TorrentWebScrapper
    {
        public override string Url => "https://1337x.to";

        protected override string SearchResultListIdentifier => "//td[contains(@class, 'coll-1 name')]";

        protected override string TorrentLinkPageIdentifier => "//a[contains(@href, 'torrent')]";

        protected override string TorrentLinkButtonsIdentifier => "//a[contains(@href, 'magnet')]";

        protected override string MediaQualityIdentifier => "//div[@class='box-info-heading clearfix']//h1";

        protected override bool FrenchVersion => false;

        protected override bool CheckQuality => true;

        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaSearchId => $"{Url}/category-search/{mediaSearchId}/TV/1/").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            var nodes = htmlNode.DocumentNode.SelectNodes("//a");
            var titleNode = nodes.FirstOrDefault(node => node.Attributes["href"]?.Value.Contains("torrent") ?? false);
            return titleNode?.InnerText.Trim();
        }
    }
}
