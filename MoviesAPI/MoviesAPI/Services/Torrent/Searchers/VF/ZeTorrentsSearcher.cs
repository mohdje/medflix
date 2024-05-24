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

namespace MoviesAPI.Services.Torrent
{
    internal class ZeTorrentsSearcher : TorrentVFSearcher
    {
        public override string Url => "https://www.zetorrents.pw";
        protected override string SearchResultListIdentifier => "//div[@class='content-list-torrent']//div[@class='maxi']";
        protected override string MagnetButtonIdentifier => "//div[@class='btn-magnet']/a";
        protected override string TorrentButtonIdentifier => "//div[@class='btn-download']/a";
    }
}
