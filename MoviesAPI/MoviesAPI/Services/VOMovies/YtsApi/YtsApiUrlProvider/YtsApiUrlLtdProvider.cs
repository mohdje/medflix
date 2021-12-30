using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.VOMovies.YtsApi
{
    public class YtsApiUrlLtdProvider : IYtsApiUrlProvider
    {
        public string GetBaseApiUrl()
        {
            return "https://yts.unblockit.tv/api/v2/";
        }

        public string GetPingUrl()
        {
            return GetBaseApiUrl();
        }
    }
}
