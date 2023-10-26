using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models
{
    public class VideoOption
    {
        public string Url { get; set; }
        public string Quality { get; set; }
        public bool Selected { get; set; }
    }
}
