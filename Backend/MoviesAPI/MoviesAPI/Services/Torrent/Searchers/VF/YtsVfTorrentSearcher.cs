using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    internal class YtsVfTorrentSearcher : TorrentVFSearcher
    {
        public override string Url => "https://www.y-t-s.co";

        protected override string SearchResultListIdentifier => "//div[@class='browse-movie-bottom']";

        protected override string MagnetButtonIdentifier => "//div[contains(@class, 'torrent-modal-download')]/a";

        protected override string TorrentButtonIdentifier => string.Empty;
    }
}
