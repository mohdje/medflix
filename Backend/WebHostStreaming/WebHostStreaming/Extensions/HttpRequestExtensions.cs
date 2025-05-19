using Microsoft.AspNetCore.Http;
using System.Linq;

namespace WebHostStreaming.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string MEDFLIX_CLIENT_IDENTIFIER_PREFIX = "MEDFLIX_CLIENT";
        public static string GetClientAppIdentifier(this HttpRequest request)
        {
            if (request.Headers.TryGetValue("User-Agent", out var userAgent))
            {
                var userAgentClientAppIdientifier = userAgent.ToString().Split(" ").FirstOrDefault();

                if (!string.IsNullOrEmpty(userAgentClientAppIdientifier) && userAgentClientAppIdientifier.StartsWith(MEDFLIX_CLIENT_IDENTIFIER_PREFIX))
                    return userAgentClientAppIdientifier.Replace(MEDFLIX_CLIENT_IDENTIFIER_PREFIX, string.Empty);
            }

            return null;
        }
    }
}
