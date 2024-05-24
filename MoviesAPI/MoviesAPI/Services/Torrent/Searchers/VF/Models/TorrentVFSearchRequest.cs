using HtmlAgilityPack;
using MoviesAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoviesAPI.Services.Torrent
{
    internal abstract class TorrentVFSearchRequest
    {
        public abstract string MediaName { get; }
        protected abstract bool CheckTorrentTitle(string title);

        public bool Match(HtmlNode htmlNode, out string torrentPageRelativeUrl, out string mediaQuality)
        {
            var linkNode = htmlNode.Descendants()?.FirstOrDefault(n => n.Name == "a");
            var title = HttpUtility.HtmlDecode(linkNode?.InnerText.Trim());

            var isTitleMatching = title != null && CheckTorrentTitle(title);

            torrentPageRelativeUrl = isTitleMatching ? linkNode.Attributes["href"].Value : string.Empty;
            mediaQuality = isTitleMatching ? linkNode.InnerText.GetVideoQuality() : string.Empty;

            return isTitleMatching;
        }
    }
}
