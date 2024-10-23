using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Utils
{
    public static class Icons
    {
        const string PLAY_SVG = "play.svg";
        const string PAUSE_SVG = "pause.svg";
        const string SUBTITLES_SVG = "subtitles.svg";
        const string QUALITIES_SVG = "settings.svg";

        public static ImageSource PlayIcon => ImageSource.FromFile(PLAY_SVG);
        public static ImageSource PauseIcon => ImageSource.FromFile(PAUSE_SVG);
        public static ImageSource SubtitlesIcon => ImageSource.FromFile(SUBTITLES_SVG);
        public static ImageSource QualitiesIcon => ImageSource.FromFile(QUALITIES_SVG);
    }
}
