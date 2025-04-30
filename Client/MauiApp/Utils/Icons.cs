using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Utils
{
    public static class Icons
    {
        public static ImageSource PlayIcon => GetImageSourceFromFile("play");
        public static ImageSource PauseIcon => GetImageSourceFromFile("pause");
        public static ImageSource SubtitlesIcon => GetImageSourceFromFile("subtitles");
        public static ImageSource QualitiesIcon => GetImageSourceFromFile("settings");
        public static ImageSource FullscreenIcon => GetImageSourceFromFile("fullscreen");
        public static ImageSource FullscreenExitIcon => GetImageSourceFromFile("fullscreen_exit");

        private static ImageSource GetImageSourceFromFile(string filename)
        {
            string imgExtension = DeviceInfo.Current.Platform == DevicePlatform.WinUI ? "png" : "svg";
            return ImageSource.FromFile($"{filename}.{imgExtension}");
        }
    }
}
