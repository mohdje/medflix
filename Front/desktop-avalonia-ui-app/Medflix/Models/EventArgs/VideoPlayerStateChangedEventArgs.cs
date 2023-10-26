using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.EventArgs
{
    public class VideoPlayerEventArgs
    {
        public VLCState State { get; set; }
        public double Progress { get; set; }
        public long CurrentTime { get; set; }
        public long RemainingTime { get; set; }
        public bool Muted { get; set; }

        public bool CanPlay { get; set; }
    }
}
