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
        YtsHtmlPm = 3,

        [Description("Yts.ltd - Api")]
        YtsApiLtd = 4,

        [Description("Yts.ltd - Site")]
        YtsHtmlLtd = 5,
    }

    public enum VFMoviesService
    {
        [Description("ZeTorrents - Site")]
        ZeTorrents = 0,

        [Description("Torrent911 - Site")]
        Torrent911 = 1,

        [Description("OxTorrent - Site")]
        OxTorrent = 2
    }

    public enum SubtitlesService
    {
        [Description("OpenSubtitles - Site")]
        OpenSubtitlesHtml = 0
    }
}
