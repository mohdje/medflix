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
        public override string Url => "https://www.torrent911.in/";
        protected override string SearchResultListIdentifier => "//td";
        protected override string MagnetButtonIdentifier => "//div[@class='btn-magnet']/a";
        protected override string TorrentButtonIdentifier => "//div[@class='btn-download']/a";
    }
}
