using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesAPI.Services
{
    public abstract class BaseService
    {
        protected abstract string GetPingUrl();
        public async Task<bool> PingAsync()
        {
            try
            {
                var result = await HttpRequester.GetAsync(new Uri(GetPingUrl()));
                return !string.IsNullOrEmpty(result);
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
