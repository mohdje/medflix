﻿using HtmlAgilityPack;
using System.Linq;


namespace MoviesAPI.Services.Torrent.Searchers.WebScrappers
{
    internal class OxTorrentScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www.oxtorrent.co";
        protected override string SearchResultListIdentifier => "//table[@class='table table-hover']//td[a]";
        protected override string TorrentLinkButtonsIdentifier => "//div[@class='btn-magnet']//a";
        protected override string MediaQualityIdentifier => "//div[@class='block-detail']//a";
        protected override string TorrentLinkPageIdentifier => "//a";
        protected override bool FrenchVersion => true;
        protected override bool CheckQuality => true;
        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaId => $"{Url}/recherche/{mediaId}").ToArray();
        }
        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            return htmlNode.DocumentNode.InnerText.Trim();
        }
        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            var specsNodes = torrentHtmlPage.DocumentNode.SelectNodes("//table[@class='table']//tr");
            var textIdentifier = "Seeders:";
            var seedersNode = specsNodes.FirstOrDefault(node => node.InnerText.Contains(textIdentifier));
            if (seedersNode != null && int.TryParse(seedersNode?.InnerText.Replace(textIdentifier, string.Empty).Trim(), out var nbSeeders))
                return nbSeeders > 0;
            else
                return false;
        }
    }
}
