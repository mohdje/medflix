using HtmlAgilityPack;
using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace MoviesAPI.Services.Torrent
{
    internal class YggTorrentSearcher : TorrentVFSearcher
    {
        public override string Url => "https://www.yggtorrent.pm";
        protected override string SearchResultListIdentifier => "//td";
        protected override string MagnetButtonIdentifier => "//a[@class='bott']";
        protected override string TorrentButtonIdentifier => "//a[@class='butt']";
    }
}
