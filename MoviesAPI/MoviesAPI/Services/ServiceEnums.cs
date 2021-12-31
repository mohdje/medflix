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

        [Description("Yts.ltd - Api")]
        YtsApiLtd = 1,

        [Description("Yts.mx - Site")]
        YtsHtmlMx = 2,

        [Description("Yts.ltd - Site")]
        YtsHtmlLtd = 3,

        [Description("Yts.one - Site")]
        YtsHtmlOne = 4,

        [Description("Yts.pm - Site")]
        YtsHtmlPm = 5
    }

    public enum VFMoviesService
    {
        [Description("OxTorrent - Site")]
        OxTorrent = 0
    }

    public enum SubtitlesService
    {
        [Description("OpenSubtitles - Site")]
        OpenSubtitlesHtml = 0
    }
}
