using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.Media
{
    public class MediaSource
    {
        public string TorrentUrl { get; set; }
        public string FilePath { get; set; }
        public string Quality { get; set; }
    }
}
