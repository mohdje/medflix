using WebHostStreaming.Helpers;
using WebHostStreaming.Models;

namespace WebHostStreaming.Torrent
{
    public class TorrentRequest
    {
        public ITorrentFileSelector TorrentFileSelector { get; }
        public VideoInfo VideoInfo { get; }
        public string TorrentUrl { get; }
        public string ClientAppId { get; }

        public TorrentRequest(string clientAppId, string torrentUrl, string mediaId, string quality, LanguageVersion languageVersion, int seasonNumber = 0, int episodeNumber = 0)
        {
            this.TorrentUrl = torrentUrl;   
            this.ClientAppId = clientAppId;

            this.VideoInfo = new VideoInfo
            {
                MediaId = mediaId,
                Quality = quality,
                Language = languageVersion,
                SeasonNumber = seasonNumber,
                EpisodeNumber = episodeNumber
            };

            if (seasonNumber > 0 && episodeNumber > 0)
                TorrentFileSelector = new SerieEpisodeTorrentFileSelector(seasonNumber, episodeNumber);
            else
                TorrentFileSelector = new VideoTorrentFileSelector();
        }
    }
}
