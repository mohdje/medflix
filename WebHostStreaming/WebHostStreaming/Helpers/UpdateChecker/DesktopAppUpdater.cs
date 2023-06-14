using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using WebHostStreaming.Helpers;
using WebHostStreaming.Models;
using System.Linq;
using System.IO;
using System;

namespace WebHostStreaming.Helpers
{
    public class DesktopAppUpdater : IAppUpdater
    {
        static HttpClient httpClient;

        const string UPDATE_CHECKER_URL = "https://api.github.com/repos/mohdje/medflix/releases/latest";

        AppRelease NewRelease;
        public DesktopAppUpdater()
        {
            if(httpClient == null)
            {
                httpClient = new HttpClient();
               
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.GitHubApiKey);
                httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "C# App");
            }
        }

        public async Task<bool> IsNewReleaseAvailableAsync()
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            var response = await httpClient.GetAsync(UPDATE_CHECKER_URL);

            if (response.IsSuccessStatusCode)
            {
                var responseObject = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseObject))
                {
                    var latestRelease = Newtonsoft.Json.JsonConvert.DeserializeObject<AppRelease>(responseObject);
                    if (latestRelease.Name != AppConfiguration.VersionName)
                    {
                        NewRelease = latestRelease;
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> DownloadNewReleaseAsync(string filePath)
        {
            if (NewRelease == null)
                throw new NullReferenceException("NewRelease is null. Call IsNewVersionAvailable() first to check if a new version is available");

            string releaseUrl = string.Empty;
            if (AppConfiguration.IsWindowsVersion)
                releaseUrl = NewRelease.Assets.SingleOrDefault(asset => asset.Name.Contains("Windows"))?.Url;
            else if(AppConfiguration.IsMacosVersion)
                releaseUrl = NewRelease.Assets.SingleOrDefault(asset => asset.Name.Contains("MacOs"))?.Url;

            if (!string.IsNullOrWhiteSpace(releaseUrl))
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();

                var response = await httpClient.GetAsync(releaseUrl);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using (var fileStream = await response.Content.ReadAsStreamAsync())
                        {
                            var bytes = new byte[fileStream.Length];
                            await fileStream.ReadAsync(bytes, 0, bytes.Length);

                            File.WriteAllBytes(filePath, bytes);
                        }
                        return true;
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            return false;
        }
    }
}
