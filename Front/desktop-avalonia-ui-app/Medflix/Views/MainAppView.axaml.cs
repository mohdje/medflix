using Avalonia.Controls;
using System.Text.Json;
using System;
using Medflix.Models;
using Medflix.Models.EventArgs;
using Medflix.Tools;

namespace Medflix.Views
{
    public partial class MainAppView : UserControl
    {
        public event EventHandler<OpenVideoPlayerArgs> OpenVideoPlayerRequest;
        public event EventHandler MainAppViewLoaded;

        Window ParentWindow;
        public MainAppView()
        {
            InitializeComponent();
        }

        private void WebMessageReceived(object? sender, WebViewCore.Events.WebViewMessageReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message))
                return;

            try
            {
                var options = JsonSerializer.Deserialize<VideoPlayerOptions>(e.Message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                if (OpenVideoPlayerRequest != null)
                    OpenVideoPlayerRequest.Invoke(this, new OpenVideoPlayerArgs(options));
            }
            catch
            {
                NotifyVideoPlayerClosed();
            }
        }

        public void LoadView(Window windowContainer)
        {
            this.ParentWindow = windowContainer;

            this.WebViewControl.NavigationCompleted += (s, e) =>
            {
                this.ShowView();
                this.MainAppViewLoaded?.Invoke(this, null);
            };

            this.WebViewControl.WebMessageReceived += WebMessageReceived;

            this.WebViewControl.Url = new Uri (Consts.MainAppViewUrl); 
        }

        public void NotifyVideoPlayerClosed()
        {
            ShowView();
            this.WebViewControl.PostWebMessageAsString("close", null);
        }

        private void ShowView()
        {
            this.WebViewControl.Height = this.ParentWindow.Height;
            this.WebViewControl.Width = this.ParentWindow.Width;
        }
    }
}
