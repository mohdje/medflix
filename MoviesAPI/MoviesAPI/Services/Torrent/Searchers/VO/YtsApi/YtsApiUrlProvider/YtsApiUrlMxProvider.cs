using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Torrent
{
    public class YtsApiUrlMxProvider : IYtsApiUrlProvider
    {
        public string GetBaseApiUrl()
        {
            return "https://yts.mx/api/v2/";
        }

        public string GetPingUrl()
        {
            return "https://yts.mx/api";
        }
    }
}
