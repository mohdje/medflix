using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsApi
{
    public interface IYtsApiUrlProvider
    {
        string GetBaseApiUrl();

        string GetPingUrl();
    }
}
