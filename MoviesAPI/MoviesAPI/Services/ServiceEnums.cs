using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{

    public enum VOMovieService
    {
        [Description("Yts.mx - Api")]
        YtsApiMx = 0,

        [Description("Yts.mx - Site")]
        YtsHtmlMx = 1,

        [Description("Yts.one - Site")]
        YtsHtmlOne = 2,

        [Description("Yts.pm - Site")]
        YtsHtmlPm = 3
    }

    public enum VFMoviesService
    {
        [Description("ZeTorrents")]
        ZeTorrents = 0,

        [Description("Torrent911")]
        Torrent911 = 1,

        [Description("OxTorrent")]
        OxTorrent = 2
    }

    public enum SubtitlesService
    {
        [Description("YtsSubs - Site")]
        YtsSubs = 1,

       [Description("OpenSubtitles - Site")]
        OpenSubtitlesHtml = 0

       
    }
}
