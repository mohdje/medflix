using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.EventArgs
{
    public class OpenVideoPlayerArgs
    {
        public VideoPlayerOptions VideoPlayerOptions { get; }

        public OpenVideoPlayerArgs(VideoPlayerOptions videoPlayerOptions)
        {
            VideoPlayerOptions = videoPlayerOptions;
        }
    }
}
