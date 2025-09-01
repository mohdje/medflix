using MoviesAPI.Extensions;
using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services.Torrent
{
    internal class EztvApiSearcher : ITorrentSearcher
    {
        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(TorrentRequest torrentRequest)
        {
            var url = "https://eztv.tf/api/get-torrents";

            var parameters = new NameValueCollection
            {
                { "imdb_id", torrentRequest.ImdbId.Replace("tt", string.Empty) },
                { "limit", "100" }
            };

            var keepSearching = true;
            var page = 0;
            var torrents = Enumerable.Empty<EztvTorrentDto>();
            while (keepSearching)
            {
                page++;
                parameters.Set("page", page.ToString());
                var requestResult = await HttpRequester.GetAsync<EztvResultDto>(url, parameters);

                if (requestResult?.Torrents == null || !requestResult.Torrents.Any())
                    break;

                torrents = requestResult.Torrents.Where(t =>
                                t.SeasonInt == torrentRequest.SeasonNumber &&
                                (t.EpisodeInt == torrentRequest.EpisodeNumber || t.EpisodeInt == 0) &&
                                (t.Peers > 0 || t.Seeds > 0) &&
                                (!string.IsNullOrEmpty(t.TorrentUrl) || !string.IsNullOrEmpty(t.MagnetUrl)));

                keepSearching = !torrents.Any();
            }
           
            return torrents.Select(t => new MediaTorrent() 
            { 
                DownloadUrl = !string.IsNullOrEmpty(t.TorrentUrl) ? t.TorrentUrl : t.MagnetUrl,
                Quality = t.Title.GetVideoQuality()
            });
        }
    }
}
