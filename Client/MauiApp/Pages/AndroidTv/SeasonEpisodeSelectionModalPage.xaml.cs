using Medflix.Controls.AndroidTv;
using Medflix.Models.Media;
using Medflix.Services;
using Medflix.Views.AndroidTv;

namespace Medflix.Pages
{
    public partial class SeasonEpisodeSelectionModalPage : ContentPage
    {
        public event EventHandler<EpisodeSelectedEventArgs> OnEpisodeSelected;
        string mediaId;

        int seasonNumber = 1;
        IEnumerable<Episode> episodes;

        public SeasonEpisodeSelectionModalPage(string mediaId, int seasonCount, int selectedSeason)
        {
            InitializeComponent();

            this.mediaId = mediaId;
            this.seasonNumber = selectedSeason;

            for (int i = 1; i <= seasonCount; i++)
                SeasonsList.Children.Add(BuildSeasonButton(i));

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            var firstButton = SeasonsList.Children[seasonNumber - 1] as ThreeStateButton;
            firstButton.Selected = true;
            firstButton.SetFocus();

            LoadSeasonAsync();
        }

        private async Task LoadSeasonAsync()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Spinner.IsVisible = true;
                NoEpisodesMessage.IsVisible = false;
                PlusButton.IsVisible = false;
                EpisodesList.Children.Clear();
            });

            var episodesForSeason = await MedflixApiService.Instance.GetEpisodesAsync(mediaId, seasonNumber);

            if (episodesForSeason != null && episodesForSeason.Any())
            {
                episodes = episodesForSeason.OrderBy(e => e.EpisodeNumber);
                DisplayNextEpisodes();
            }
            else
            {
                NoEpisodesMessage.IsVisible = true;
                Spinner.IsVisible = false;
            }
        }
        private void DisplayNextEpisodes()
        {
            var startIndex = EpisodesList.Children.Count;
            var episodesToDisplay = 5;
            var range = new Range(EpisodesList.Children.Count, startIndex + episodesToDisplay);

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Spinner.IsVisible = true;
                PlusButton.IsVisible = false;

                EpisodesList.Children.LastOrDefault()?.Focus();

                foreach (var episode in episodes.Take(range))
                {
                    var watchMediaInfo = await MedflixApiService.Instance.GetEpisodeWatchMediaInfo(mediaId, seasonNumber, episode.EpisodeNumber);
                    var progress = watchMediaInfo?.Progress ?? 0;

                    var episodePresentationView = new EpisodePresentationView(episode, progress);
                    episodePresentationView.OnClick += (s, e) =>
                    {
                        OnEpisodeSelected?.Invoke(
                            this,
                            new EpisodeSelectedEventArgs
                            {
                                SeasonNumber = seasonNumber,
                                EpisodeNumber = episode.EpisodeNumber,
                                IsLastEpisodeOfSeason = episode.EpisodeNumber == episodes.Max(ep => ep.EpisodeNumber),
                                WatchMedia = watchMediaInfo
                            });
                    };
                    EpisodesList.Children.Add(episodePresentationView);
                }

                Spinner.IsVisible = false;
                PlusButton.IsVisible = EpisodesList.Children.Count != episodes.Count();
            });
        }

        private ThreeStateButton BuildSeasonButton(int seasonNumber)
        {
            var button = new ThreeStateButton
            {
                SelectedBackgroundColor = Color.FromArgb("#1f1f1f"),
                SelectedTextColor = Brush.White.Color,
                FocusedBackgroundColor = Brush.Red.Color,
                FocusedTextColor = Brush.White.Color,
                UnfocusedBackgroundColor = Color.FromArgb("#424141"),
                UnfocusedTextColor = Color.FromArgb("#747171"),
                Margin = new Thickness(7),
                Text = $"Season {seasonNumber}",
                Selected = false
            };
            button.OnClick += async (s, e) => await OnSeasonButtonClick(seasonNumber);

            return button;
        }

        private async Task OnSeasonButtonClick(int seasonNumber)
        {
            this.seasonNumber = seasonNumber;

            var previsouSelectedButton = SeasonsList.Children.Select(control => control as ThreeStateButton).Single(btn => btn.Selected);
            previsouSelectedButton.Selected = false;

            var selectedButton = SeasonsList.Children.ElementAt(seasonNumber - 1) as ThreeStateButton;
            selectedButton.Selected = true;

            await LoadSeasonAsync();
        }

        private void OnPlusButtonClicked(object sender, EventArgs e)
        {
            DisplayNextEpisodes();
        }
    }
}