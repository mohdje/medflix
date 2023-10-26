using Avalonia.Controls;
using System.Text.Json;
using System.Web;
using System;
using Medflix.Models;
using Medflix.Models.EventArgs;

namespace Medflix.Views
{
    public partial class WebView : UserControl
    {
        public event EventHandler<OpenVideoPlayerArgs> OpenVideoPlayerRequest;
        public WebView()
        {
            InitializeComponent();

            this.WebViewControl.NavigationStarting += onNavigation;
        }

        private void onNavigation(object? sender, WebViewCore.Events.WebViewUrlLoadingEventArg e)
        {
            var uri = e.Url;
            if (uri!.Host == "openvideoplayer")
            {
                e.Cancel = true;
                var queryParameters = HttpUtility.ParseQueryString(uri.Query);
                var optionsParameter = queryParameters[0];

                var optionsJSON = HttpUtility.UrlDecode(optionsParameter);

                var options = JsonSerializer.Deserialize<VideoPlayerOptions>(optionsJSON, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                if (OpenVideoPlayerRequest != null)
                    OpenVideoPlayerRequest.Invoke(this, new OpenVideoPlayerArgs(options));
            }
        }
    }
}
