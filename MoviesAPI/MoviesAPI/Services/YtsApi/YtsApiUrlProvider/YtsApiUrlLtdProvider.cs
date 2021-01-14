using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesAPI.Services.YtsApi
{
    public class YtsApiUrlLtdProvider : IYtsApiUrlProvider
    {
        public string GetBaseApiUrl()
        {
            return "https://yts.unblockit.ltd/api/v2/";
        }
    }
}
