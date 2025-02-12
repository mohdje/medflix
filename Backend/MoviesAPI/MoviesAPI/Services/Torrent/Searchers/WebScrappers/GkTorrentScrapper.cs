
using System;
using System.Linq;
using HtmlAgilityPack;

namespace MoviesAPI.Services.Torrent
{
    internal class GkTorrentScrapper : TorrentWebScrapper
    {
        public override string Url => "https://www.gktorrent.gy";
        protected override string SearchResultListIdentifier => "//table[@class='table table-hover']//td[@class='liste-accueil-nom']";
        protected override string TorrentLinkButtonsIdentifier => "//div[@class='btn-download']/a";

        protected override bool FrenchVersion => true;

        protected override bool CheckQuality => true;

        protected override string MediaQualityIdentifier => throw new NotImplementedException();

        protected override string TorrentLinkPageIdentifier => throw new NotImplementedException();

        protected override string[] GetSearchUrls(TorrentSearchRequest torrentSearchRequest)
        {
            return torrentSearchRequest.MediaSearchIdentifiers.Select(mediaId => $"{Url}/recherche/{mediaId}").ToArray();
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            throw new NotImplementedException();
        }

        protected override bool TorrentHasSeeders(HtmlDocument torrentHtmlPage)
        {
            throw new NotImplementedException();
        }
    }
}
