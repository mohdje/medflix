using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedflixWinForms.Utils
{

    internal class VLCHelper
    {
        private static LibVLC libVLC;

        public static event EventHandler InitFinished;

        private static bool initCalled;
        public static void InitVLC()
        {
            initCalled = true;
            // Initialize LibVLC
            Core.Initialize();

            // Create a new LibVLC instance
            libVLC = new LibVLC();

            InitFinished?.Invoke(null, null);
        }

        public static MediaPlayer CreateMediaPlayer()
        {
            CheckLibVLC();

            return new MediaPlayer(libVLC);
        }

        public static Media CreateMedia(string videoPath, string[] options)
        {
            CheckLibVLC();

            return new Media(libVLC, new Uri(videoPath), options);
        }

        public static RendererDiscoverer CreateRendererDiscover()
        {
            CheckLibVLC();

            var name = libVLC.RendererList.Any() ? libVLC.RendererList[0].Name : null;
            return new RendererDiscoverer(libVLC, name);
        }

        private static void CheckLibVLC()
        {
            if (!initCalled) throw new Exception("Call InitVLCAsync first");
            else if (libVLC == null) throw new Exception("Suscribe to InitFinished to be notified when LibVLC is ready to be used");
        }
    }
}
