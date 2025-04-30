using Medflix.Models;
using Medflix.Services;

namespace Medflix.Pages
{

    public partial class StartingPage : ContentPage
    {
        public event EventHandler OnReady;

        public StartingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrEmpty(AppConfig.Instance.MedflixServiceAddress))
            {
                Task.Run(async () =>
                {
                    var hostAdressValid = await TryHostServiceAddress(AppConfig.Instance.MedflixServiceAddress);
                    if (hostAdressValid)
                        OnReady?.Invoke(this, EventArgs.Empty);
                });
            }
            else
                ShowLoading(false);
        }

        private async Task<bool> TryHostServiceAddress(string serviceAddress)
        {
            ShowLoading(true);

            await Task.Delay(1500);

            var isServiceAddressValid = await MedflixApiService.Instance.TrySetHostServiceAddressAsync(serviceAddress);

            if (!isServiceAddressValid)
                ShowLoading(false);

            return isServiceAddressValid;
        }

        private void ShowLoading(bool showSpinner)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Spinner.IsRunning = showSpinner;
                AddressInputArea.IsVisible = !showSpinner;
            });
        }

        private void EntryCompleted(object sender, EventArgs e)
        {
            Task.Run(async () => {
                await TryHostServiceAddress(AddressInput.Text);
                var hostAdressValid = await TryHostServiceAddress(AddressInput.Text);
                if (hostAdressValid)
                {
                    AppConfig.Instance.UpdateHostServiceAddress(AddressInput.Text);
                    OnReady?.Invoke(this, EventArgs.Empty);
                }
            });
        }
    }
}