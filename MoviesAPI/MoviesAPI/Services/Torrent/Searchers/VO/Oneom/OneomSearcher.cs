using MoviesAPI.Helpers;
using MoviesAPI.Services.Torrent.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoviesAPI.Extensions;

namespace MoviesAPI.Services.Torrent
{
    internal class OneomSearcher : ITorrentSerieSearcher
    {
        private List<KeyValuePair<string, string>> httpRequestHeaders;
        public OneomSearcher()
        {
            httpRequestHeaders = new List<KeyValuePair<string, string>>();
            httpRequestHeaders.Add(new KeyValuePair<string, string>("Accept", "application/json"));
        }
        public string GetPingUrl()
        {
            return "https://oneom.is";
        }


        public async Task<IEnumerable<MediaTorrent>> GetTorrentLinksAsync(string serieName, string imdbId, int seasonNumber, int episodeNumber)
        {
            var oneomId = await GetOneomId(serieName, imdbId);

            if(string.IsNullOrEmpty(oneomId))
                return new MediaTorrent[0];

            var torrents = await GetOneomTorrentsEpisode(oneomId, seasonNumber, episodeNumber);

            if (torrents != null && torrents.Any())
            {
                return torrents.DistinctBy(torrent => torrent.Value)
                                .Select(torrent => new MediaTorrent()
                                {
                                    DownloadUrl = torrent.Value,
                                    Quality = GetQuality(torrent.Title)
                                });
            }
            else
                return new MediaTorrent[0];
        }

        private async Task<string> GetOneomId(string serieName, string imdbId)
        {
            var result = await HttpRequester.GetAsync<OneomSearchResultDto>(BuildSearchSerieUrl(serieName), httpRequestHeaders);

            string id = null;
            if(result != null)
                id = result.Data.Series.FirstOrDefault(serie => serie.ImdbId == imdbId)?.Id;

            return id;
        }

        private async Task<OneomTorrentDto[]> GetOneomTorrentsEpisode(string oneomId, int seasonNumber, int episodeNumber)
        {
            var result = await HttpRequester.GetAsync<OneomSearchResultDto>(BuildSearchTorrentUrl(oneomId), httpRequestHeaders);

            if (result != null)
                return result.Data.Serie.Episodes?
                          .FirstOrDefault(episode => episode.Season.toInt().Value == seasonNumber && episode.Episode.toInt().Value == episodeNumber)
                          ?.Torrent;
            else
                return new OneomTorrentDto[0];
        }

        private string BuildSearchSerieUrl(string serieName)
        {
            return $"https://oneom.is/search/serial?title={serieName.RemoveSpecialCharacters()}&limit=500";
        }

        private string BuildSearchTorrentUrl(string oneomId)
        {
            return $"https://oneom.is/serial/{oneomId}";
        }

        private string GetQuality(string torrentTitle)
        {
            var qualities = new[] {"HDTV", "480p", "720p", "1080p" };

            foreach (var quality in qualities)
            {
                if (torrentTitle.Contains(quality, StringComparison.OrdinalIgnoreCase))
                    return quality;
            }

            return "Unknown";
        }

    }
}
