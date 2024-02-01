
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MedflixWinforms.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using MedflixWinForms.Utils;
using Microsoft.Extensions.Hosting;
using MedflixWinForms.Services;
using MedflixWinForms.Forms;
using System.ComponentModel;
using Medflix.Models;
using System.Text.Json;

namespace MedflixWinforms
{
    public partial class MainForm : Form
    {
        private IHost webhost;
        private VideoPlayerOptions videoPlayerOptions;
        private bool isVlcReady;

        public MainForm()
        {
            InitializeComponent();

            InitVLCAsync();
            InitWebHostAsync();
        }

        private void InitVLCAsync()
        {
            VLCHelper.InitFinished += VLCHelper_InitFinished;
            Task.Run(() => VLCHelper.InitVLC());
        }

        private void InitWebHostAsync()
        {
            Task.Run(async () =>
            {
                webhost = WebHostStreaming.AppStart.CreateHost(new string[0], true);
                webhost.Start();
                await CheckUpdateAsync();
            }).ContinueWith(t => StartWebView());
        }

        private void StartWebView()
        {
            this.Invoke(() =>
            {
                this.WebView.EnsureCoreWebView2Async().ContinueWith(t =>
                {
                    this.Invoke(() =>
                    {
                        this.WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                        this.WebView.CoreWebView2.WebMessageReceived += WebMessageReceived;
                        this.WebView.Source = new Uri(Consts.MainAppViewUrl);
                        this.WebView.Visible = true;
                    });
                });
            });
        }

        private async Task CheckUpdateAsync()
        {
            var appUpdateService = new AppUpdateService();
            var updateAvailabe = await appUpdateService.IsNewReleaseAvailableAsync();
            if (updateAvailabe)
            {
                this.Invoke(() =>
                {
                    var updateForm = new UpdateForm(appUpdateService);
                    updateForm.OnStartExtractUpdate += (s, e) =>
                    {
                        updateForm.Close();
                        this.Close();
                    };
                    updateForm.Show();
                });

            }
        }
        private void VLCHelper_InitFinished(object? sender, EventArgs e)
        {
            isVlcReady = true;
            if (videoPlayerOptions != null)
            {
                this.Invoke(() =>
                {
                    StartVideo();
                });
            }
        }

        private void WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            var webMessage = e.TryGetWebMessageAsString();
            if (string.IsNullOrEmpty(webMessage))
                return;

            try
            {
                videoPlayerOptions = JsonSerializer.Deserialize<VideoPlayerOptions>(webMessage, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                if (isVlcReady)
                    StartVideo();
            }
            catch
            {
                NotifyVideoPlayerClosed();
            }
        }

        private void NotifyVideoPlayerClosed()
        {
            videoPlayerOptions = null;

            this.WebView.CoreWebView2.PostWebMessageAsString("videoPlayerClosed");
        }
        private void StartVideo()
        {
            this.WebView.Visible = false;

            var VideoPlayerControl = new VideoPlayerControl(this, videoPlayerOptions);

            VideoPlayerControl.Dock = DockStyle.Fill;
            VideoPlayerControl.OnExitVideoPlayer += (s, e) =>
            {
                this.Controls.Remove(VideoPlayerControl);
                VideoPlayerControl.Dispose();
                this.WebView.Visible = true;
                NotifyVideoPlayerClosed();
            };

            this.Controls.Add(VideoPlayerControl);

            VideoPlayerControl.BringToFront();

            Task.Delay(1000).ContinueWith(t => { VideoPlayerControl.Play(); });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            webhost.StopAsync();
            base.OnClosing(e);
        }
    }


}