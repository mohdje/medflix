using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsApi
{
    public class YtsApiUrlMxProvider : IYtsApiUrlProvider
    {
        public string GetBaseApiUrl()
        {
            return "https://yts.mx/api/v2/";
        }
    }
}
