using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Medflix.Models;
using Medflix.Pages;
using Medflix.Services;
using Medflix.Utils;

namespace Medflix.Views
{
    class MainPageView : WebView
    {
        public event EventHandler OnVideoPlayerPageRequested;
        JsonSerializerOptions JsonOptions => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        public MainPageView()
        {
            Navigating += OnNavigating;
        }

        private async void OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith("http"))
            {
                Uri uri = new Uri(e.Url);
                if (uri.Host == "runcsharp")
                {
                    e.Cancel = true;
                    var queryParameters = HttpUtility.ParseQueryString(uri.Query);
                    if (queryParameters != null && queryParameters.Count == 1)
                    {
                        await Task.Run(() => CallCSharpFunction(queryParameters[0]));
                    }
                }
            }
        }

        private void CallCSharpFunction(string runCSharpObjectBase64)
        {
            var bytes = Convert.FromBase64String(runCSharpObjectBase64);
            var runCSharpObjectJSON = Encoding.UTF8.GetString(bytes);

            var navigationParameter = JsonSerializer.Deserialize<WebViewNavigationParameter>(runCSharpObjectJSON, JsonOptions);

            if (navigationParameter.FunctionName == "ToVideoPlayerPage")
            {
                OnVideoPlayerPageRequested?.Invoke(this, null);
            }

        }
    }
}
