using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Models
{
    public class MedflixHttpHeaders
    {
        public Dictionary<string, string> DefaultHeaders { get; set; }

        public AuthenticationHeaderValue Authorization { get; set; }

        public MediaTypeWithQualityHeaderValue Accept { get; set; }

        public MedflixHttpHeaders()
        {
            DefaultHeaders = new Dictionary<string, string>();
        }
    }
}
