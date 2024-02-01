using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Linq;
using System.IO;
using System;
using Medflix.Models;
using System.Diagnostics;
using System.Reflection;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System.Collections.Generic;
using MedflixWinForms.Models;
using MedflixWinForms.Utils;

namespace MedflixWinForms.Services
{
    public class AppUpdateService : HttpClientService
    {
        AppRelease NewRelease;

        MedflixHttpHeaders MedflixHttpHeaders;
        public AppUpdateService()
        {
            MedflixHttpHeaders = new MedflixHttpHeaders();
            MedflixHttpHeaders.DefaultHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            MedflixHttpHeaders.DefaultHeaders.Add("User-Agent", "C# App");
        }

        public async Task<bool> IsNewReleaseAvailableAsync()
        {
            MedflixHttpHeaders.Accept = new MediaTypeWithQualityHeaderValue("application/vnd.github+json");

            var latestRelease = await GetAsync<AppRelease>(Consts.LatestReleaseUrl, MedflixHttpHeaders);
            if (latestRelease != null && latestRelease.Name != Consts.AppVersionName)
            {
                NewRelease = latestRelease;
                return true;
            }

            return false;
        }

        public async Task<bool> DownloadNewReleaseAsync()
        {
            if (NewRelease == null)
                throw new NullReferenceException("NewRelease is null. Call IsNewVersionAvailable() first to check if a new version is available");

            string releaseUrl = NewRelease.Assets.SingleOrDefault(asset => asset.Name.Contains("windows", StringComparison.OrdinalIgnoreCase))?.Url; 

            if (!string.IsNullOrEmpty(releaseUrl))
            {
                MedflixHttpHeaders.Accept = null;
                return await DownloadAsync(releaseUrl, AppFiles.NewReleasePackage, MedflixHttpHeaders);
            }

            return false;
        }

        public bool StartExtractUpdate()
        {
            if (Directory.Exists(AppFolders.ExtractUpdateProgramFolder))
            {
                //rename folder so it can be replaced during package extraction
                Directory.Move(AppFolders.ExtractUpdateProgramFolder, AppFolders.ExtractUpdateProgramTempFolder);

                var arguments = new List<string>();
                arguments.Add(AppFiles.NewReleasePackage);
                arguments.Add(AppContext.BaseDirectory);
                arguments.Add(AppFiles.WindowsDesktopApp);
                Process.Start(AppFiles.WindowsExtractUpdateProgram, arguments);

                return true;

            }
            return false;
        }
    }
}
