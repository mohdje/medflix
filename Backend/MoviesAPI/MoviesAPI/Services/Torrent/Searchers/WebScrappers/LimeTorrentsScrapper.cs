using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
