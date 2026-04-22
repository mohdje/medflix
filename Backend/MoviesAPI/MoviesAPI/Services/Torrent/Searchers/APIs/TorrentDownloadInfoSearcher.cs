using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Linq;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using MoviesAPI.Extensions;
using System.Xml;
using System.Xml.Serialization;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentDownloadInfoSearcher : ITorrentSearcher
    {

        string Url => "https://www.torrentdownload.info/feed";

        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(TorrentRequest torrentRequest)
        {
            if (torrentRequest.SeasonNumber <= 0 || torrentRequest.EpisodeNumber <= 0)
                return [];

            var parameters = new NameValueCollection
            {
                { "q", $"{torrentRequest.MediaName} S{torrentRequest.SeasonNumber:00}E{torrentRequest.EpisodeNumber:00}" }
            };

            var requestResult = await HttpRequester.GetAsync(new Uri(Url), parameters);
            var xml = requestResult.Substring(requestResult.IndexOf("<rss"), requestResult.LastIndexOf("</rss>") - requestResult.IndexOf("<rss") + "</rss>".Length);

            var serializer = new XmlSerializer(typeof(TorrentDownloadRssDto));
            var rssDto = serializer.Deserialize(new XmlTextReader(new System.IO.StringReader(xml))) as TorrentDownloadRssDto;

            if (rssDto?.Channel?.Items == null || rssDto.Channel.Items.Count == 0)
                return [];

            var episodeItems = rssDto.Channel.Items.Where(i => i.Title.StartsWithIgnoreDiactrics(torrentRequest.MediaName) && i.Title.Contains($"S{torrentRequest.SeasonNumber:00}E{torrentRequest.EpisodeNumber:00}"))
            .OrderByDescending(i => GetSeeds(i) ?? 0)
            .Where(i => GetSeeds(i) > 0)
            .Take(10);

            return episodeItems.Select(i => new MediaTorrent() { DownloadUrl = GetDownloadUrl(i), Quality = i.Title.GetVideoQuality() });
        }

        private int? GetSeeds(TorrentRssItem item)
        {
            var seedsLabel = "Seeds:";
            var seedsInfoIndex = item.Description.IndexOf(seedsLabel);
            if (seedsInfoIndex != -1)
            {
                var seedsPart = item.Description.Substring(seedsInfoIndex + seedsLabel.Length).Trim().Split(' ')[0];
                if (int.TryParse(seedsPart, out int seeds))
                {
                    return seeds;
                }
            }
            return null;
        }

        private string GetDownloadUrl(TorrentRssItem item)
        {
            var hashLabel = "Hash:";
            var hashInfoIndex = item.Description.IndexOf(hashLabel);
            if (hashInfoIndex != -1)
            {
                var hash = item.Description.Substring(hashInfoIndex + hashLabel.Length).Trim();
                if (!string.IsNullOrEmpty(hash))
                {
                    return $"https://itorrents.net/torrent/{hash}.torrent?title={item.Title}";
                }
            }
            return null;
        }
    }
}