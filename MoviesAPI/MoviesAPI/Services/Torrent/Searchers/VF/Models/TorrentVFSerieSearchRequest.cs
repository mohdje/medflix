using HtmlAgilityPack;
using MoviesAPI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MoviesAPI.Services.Torrent
{
    internal class TorrentVFSerieSearchRequest : TorrentVFSearchRequest
    {
        public string SerieName { get;  }
        public int EpisodeNumber { get; }
        public int SeasonNumber { get; }
        public string ImdbId { get; }

        public override string MediaName => SerieName;

        public TorrentVFSerieSearchRequest(string serieName, int episodeNumber, int seasonNumber, string imdbId = null)
        {
            SerieName = serieName;
            EpisodeNumber = episodeNumber;
            SeasonNumber = seasonNumber;
            ImdbId = imdbId;
        }
        protected override bool CheckTorrentTitle(string title)
        {
            var seasonId = $"S{(SeasonNumber < 10 ? "0" : "")}{SeasonNumber}";
            var episodeId = $"E{(EpisodeNumber < 10 ? "0" : "")}{EpisodeNumber}";

            return (title.CustomStartsWith($"{SerieName} Saison {SeasonNumber}") || title.CustomStartsWith($"{SerieName} {seasonId}{episodeId}"))
                   && (title.Contains("FRENCH") || title.Contains("TRUEFRENCH"));
        }
    }
}
