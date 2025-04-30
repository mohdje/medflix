using Medflix.Models.VideoPlayer;
using Medflix.Services;
using Medflix.Utils;
using System.Text.Json;

namespace Medflix.Pages
{
    public partial class MainPage : ContentPage
    {
        JsonSerializerOptions JsonOptions => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public MainPage()
        {
            InitializeComponent();
            
            AppWebView.Navigating += async (s, e) =>
            {
                if (e.Url.StartsWith("http://playmedia"))
                {
                    e.Cancel = true;
                    var jsonParameters = await AppWebView.EvaluateJavaScriptAsync("window.getMediaPlayerParameters();");
                    var videoPlayerParameters = JsonSerializer.Deserialize<VideoPlayerParameters>(jsonParameters, JsonOptions);

                    var mode = await AppWebView.EvaluateJavaScriptAsync("window.getAppMode();");

                    if (videoPlayerParameters != null && !string.IsNullOrEmpty(mode))
                    {
                        if (mode.Equals(Consts.Movies, StringComparison.OrdinalIgnoreCase))
                            MedflixApiService.Instance.SwitchToMoviesMode();
                        else if (mode.Equals(Consts.Series, StringComparison.OrdinalIgnoreCase))
                            MedflixApiService.Instance.SwitchToSeriesMode();

                        var videoPlayerPage = new VideoPlayerPage(videoPlayerParameters);
                        videoPlayerPage.CloseVideoPlayerRequested += async (s, e) =>
                        {
                            await Navigation.PopAsync();
                            await NotifyVideoPlayerClosed();
                        };

                        await Navigation.PushAsync(videoPlayerPage);
                    }
                    else
                        await NotifyVideoPlayerClosed();
                }
            };
            AppWebView.Source = MedflixApiService.Instance.WebAppHomeUrl;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
#if ANDROID
            if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
#endif
        }

        private async Task NotifyVideoPlayerClosed()
        {
            await AppWebView.EvaluateJavaScriptAsync("window.closeNativeMediaPlayer();");
        }
    }
}