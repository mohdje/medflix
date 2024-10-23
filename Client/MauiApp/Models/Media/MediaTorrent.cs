using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models.Media
{
    public class MediaTorrent
    {
        public string DownloadUrl { get; set; }

        public string Quality { get; set; }

        public string LanguageVersion { get; set; }
    }
}
