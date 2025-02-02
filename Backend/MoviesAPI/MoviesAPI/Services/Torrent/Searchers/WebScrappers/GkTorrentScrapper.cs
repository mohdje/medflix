
using System;
using HtmlAgilityPack;
using MoviesAPI.Extensions;

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

        protected override string BuildSearchUrl(TorrentSearchRequest torrentSearchRequest)
        {
           return $"{Url}/recherche/{torrentSearchRequest.MediaName.RemoveSpecialCharacters()}";
        }

        protected override string GetTorrentTitle(HtmlDocument htmlNode)
        {
            throw new NotImplementedException();
        }
    }
}
