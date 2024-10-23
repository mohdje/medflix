
using Medflix.Services;

namespace Medflix.Controls.VideoPlayer
{
    public class LoadingSpinner : ContentView
    {
        private Label Message;
        private ActivityIndicator Spinner;

        private bool _isListenningToDownloadState;
        private string _currentUrl;
        public LoadingSpinner()
        {
            Message = new Label { TextColor = Brush.White.Color, FontSize = 22 };
            Spinner = new ActivityIndicator { Color = Brush.White.Color };

            Content = new FlexLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Direction = Microsoft.Maui.Layouts.FlexDirection.Column,
                AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center,
                JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center,
                Children = {
                    Message,
                    Spinner
                }
            };
        }

        public async Task StartListenningDownloadState(string torrentUrl)
        {
            _isListenningToDownloadState = true;
            _currentUrl = torrentUrl;

            UpdateUI("Loading", true);

            await GetDownloadState(torrentUrl);
        }

        public void StopListenningDownloadState()
        {
            _isListenningToDownloadState = false;
            _currentUrl = string.Empty;
            UpdateUI(string.Empty, false);
        }

        private async Task GetDownloadState(string torrentUrl)
        {
            if (_currentUrl != torrentUrl || !_isListenningToDownloadState)
                return;

            var result = await MedflixApiService.Instance.GetDownloadStateAsync(torrentUrl);

            if (_currentUrl == torrentUrl && _isListenningToDownloadState)
            {
                if (result != null && result.Error)
                {
                    _isListenningToDownloadState = false;
                    _currentUrl = string.Empty;
                    UpdateUI(result.Message, false);
                }
                else
                {
                    if(result != null)
                        UpdateUI(result.Message, true);

                    await Task.Delay(2000);
                    await GetDownloadState(torrentUrl);
                }
            }
        }

        public void ShowMessage(string text, bool withSpinner)
        {
            if (_isListenningToDownloadState)
                return;

            UpdateUI(text, withSpinner);
        }

        private void UpdateUI(string text, bool showSpinner)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Message.Text = text;
                Spinner.IsRunning = showSpinner;
            });
        }
    }
}