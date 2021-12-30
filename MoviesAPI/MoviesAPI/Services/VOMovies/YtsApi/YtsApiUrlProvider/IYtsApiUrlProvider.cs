using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.VOMovies.YtsApi
{
    public interface IYtsApiUrlProvider
    {
        string GetBaseApiUrl();

        string GetPingUrl();
    }
}
