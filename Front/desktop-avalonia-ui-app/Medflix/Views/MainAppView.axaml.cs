using Avalonia.Controls;
using System.Text.Json;
using System.Web;
using System;
using Medflix.Models;
using Medflix.Models.EventArgs;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Medflix.Views
{
    public partial class MainAppView : UserControl
    {
        public event EventHandler<OpenVideoPlayerArgs> OpenVideoPlayerRequest;
        public event EventHandler MainAppViewLoaded;
        public MainAppView()
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

        public void LoadView(Window windowContainer)
        {
            this.WebViewControl.Url = new Uri ("http://localhost:5000/home/index.html"); 

            this.WebViewControl.NavigationCompleted += (s , e) =>
            {
                this.WebViewControl.Height = windowContainer.Height;
                this.WebViewControl.Width = windowContainer.Width;
                this.MainAppViewLoaded?.Invoke(this, null);
            };
        }
    }
}
