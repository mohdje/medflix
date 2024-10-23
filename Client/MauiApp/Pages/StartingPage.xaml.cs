using Medflix.Models;
using Medflix.Services;
using Medflix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Medflix.Pages
{

    public partial class StartingPage : ContentPage
    {
        public event EventHandler OnReady;

        AppConfig AppConfig;
        public StartingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Spinner.IsRunning = true;
            AddressInputArea.IsVisible = false;

            Task.Run(async () => await Init());
        }

        private async Task Init()
        {
            string hostServiceAdress = string.Empty;
            try
            {
                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, Consts.AppCongifFileName);
                var text = File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(text))
                {
                    AppConfig = JsonSerializer.Deserialize<AppConfig>(text);
                    hostServiceAdress = AppConfig.MedflixServiceAddress;
                }

            }
            catch (Exception ex)
            {
            }

            await TrySetHostServiceAddress(hostServiceAdress);
        }

        private async Task TrySetHostServiceAddress(string serviceAddress)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Spinner.IsRunning = true;
                AddressInputArea.IsVisible = false;
            });

            await Task.Delay(1500);

            var isReady = await MedflixApiService.Instance.SetHostServiceAddressAsync(serviceAddress);

            if (isReady)
            {
                if (AppConfig != null)
                    AppConfig.MedflixServiceAddress = serviceAddress;
                else
                    AppConfig = new AppConfig { MedflixServiceAddress = serviceAddress };

                var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, Consts.AppCongifFileName);
                var appConfigJson = JsonSerializer.Serialize(AppConfig);
                File.WriteAllText(filePath, appConfigJson);

                OnReady?.Invoke(this, EventArgs.Empty);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Spinner.IsRunning = false;
                AddressInputArea.IsVisible = true;
            });
        }

        private void EntryCompleted(object sender, EventArgs e)
        {
            Task.Run(async () => await TrySetHostServiceAddress(AddressInput.Text));
        }
    }
}