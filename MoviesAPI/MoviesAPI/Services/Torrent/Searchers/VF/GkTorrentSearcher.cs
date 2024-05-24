using HtmlAgilityPack;
using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Extensions;
using MoviesAPI.Services.Torrent.Dtos;
using System.Web;

namespace MoviesAPI.Services.Torrent
{
    internal class GkTorrentSearcher : TorrentVFSearcher
    {
        public override string Url => "https://www.gktorrent.pm";
        protected override string SearchResultListIdentifier => "//table[@class='table table-hover']//td[@class='liste-accueil-nom']";
        protected override string MagnetButtonIdentifier => "//div[@class='btn-magnet']/a";
        protected override string TorrentButtonIdentifier => "//div[@class='btn-download']/a";
    }
}
