using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.Torrent
{
    public interface IYtsApiUrlProvider
    {
        string GetBaseApiUrl();

        string GetPingUrl();
    }
}
