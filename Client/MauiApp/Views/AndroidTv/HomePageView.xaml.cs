using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Services;

namespace Medflix.Views.AndroidTv
{
    public partial class HomePageView : ContentView
    {
        public HomePageView()
        {
            InitializeComponent();
            this.Loaded += async (s, e) =>
            {
                if (HomeContent.Children.Count == 1)
                    await LoadContentAsync();
            };
        }

        private async Task LoadContentAsync()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Spinner.IsVisible = true;
            });

            IEnumerable<MediaDetails> mediasOfToday = null;
            IEnumerable<MediaDetails> recommandations = null;
            IEnumerable<MediaDetails> popularMedias = null;
            IEnumerable<MediaDetails> netflix = null;
            IEnumerable<MediaDetails> amazonPrime = null;
            IEnumerable<MediaDetails> disneyPlus = null;
            IEnumerable<MediaDetails> appleTv = null;

            var tasks = new Task[]
            {
                MedflixApiService.Instance.GetMediasOfTodaysAsync().ContinueWith(t => mediasOfToday = t.Result),
                MedflixApiService.Instance.GetMediasOfTodaysAsync().ContinueWith(t => recommandations = t.Result),
                MedflixApiService.Instance.GetPopularMediasAsync().ContinueWith(t => popularMedias = t.Result),
                MedflixApiService.Instance.GetPopularNetflixAsync().ContinueWith(t => netflix = t.Result),
                MedflixApiService.Instance.GetPopularAmazonPrimeAsync().ContinueWith(t => amazonPrime = t.Result),
                MedflixApiService.Instance.GetPopularDisneyPlusAsync().ContinueWith(t => disneyPlus = t.Result),
                MedflixApiService.Instance.GetPopularAppleTvAsync().ContinueWith(t => appleTv = t.Result)
            };

            await Task.WhenAll(tasks);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Spinner.IsVisible = false;
                if(mediasOfToday != null && mediasOfToday.Any())
                    HomeContent.Children.Add(new MediaHorizontalCoverListView(mediasOfToday));
                if (mediasOfToday != null && mediasOfToday.Any())
                    HomeContent.Children.Add(new MediaLitePresentationListView("Recommandations for you", recommandations));
                if (popularMedias != null && popularMedias.Any())
                    HomeContent.Children.Add(new MediaLitePresentationListView("Popular", popularMedias));
                if (netflix != null && netflix.Any())
                    HomeContent.Children.Add(new MediaLitePresentationListView("Popular on Netflix", netflix));
                if (amazonPrime != null && amazonPrime.Any())
                    HomeContent.Children.Add(new MediaLitePresentationListView("Popular on Amzon Prime", amazonPrime));
                if (disneyPlus != null && disneyPlus.Any())
                    HomeContent.Children.Add(new MediaLitePresentationListView("Popular on Disney Plus", disneyPlus));
                if (appleTv != null && appleTv.Any())
                    HomeContent.Children.Add(new MediaLitePresentationListView("Popular on Apple Plus", appleTv));
            });
        }
    }
}