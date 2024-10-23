using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Pages.AndroidTv;

namespace Medflix.Views.AndroidTv
{
    public partial class MediaLitePresentationView : ContentView
	{
        string mediaId;
		public MediaLitePresentationView(MediaDetails media)
		{
			InitializeComponent ();

            MediaButton.Focused += OnFocused;
            MediaButton.Unfocused += OnUnfocused;
            MediaButton.Clicked += OnClicked;

            MediaButton.ImageSource = !string.IsNullOrEmpty(media.CoverImageUrl) ? ImageSource.FromUri(new Uri(media.CoverImageUrl)) : ImageSource.FromFile("default_cover.svg");

            Year.Text = media.Year.ToString();
            Rating.Text = media.Rating.ToString("0.0").Replace(",", ".");
            Title.Text = media.Title;

            mediaId = media.Id;  
        }

        public MediaLitePresentationView(WatchMediaInfo watchMediaInfo) : this(watchMediaInfo.Media) 
        {
            WatchingProgress.IsVisible = true;
            WatchingProgress.ProgressTo(watchMediaInfo.Progress, 0, Easing.Default);

            if(watchMediaInfo.SeasonNumber > 0)
            {
                SeasonEpisode.IsVisible = true;
                SeasonEpisode.Text = $"Season {watchMediaInfo.SeasonNumber} Episode {watchMediaInfo.EpisodeNumber}";
            }
        }

        private void OnClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var initialBorderColor = ButtonBorder.Stroke;
                ButtonBorder.Stroke = Brush.Red.Color;

                await Task.Delay(300);

                ButtonBorder.Stroke = initialBorderColor;

                await Navigation.PushAsync(new AndroidTvMediaPresentationPage(mediaId));

            });
        }

        private void OnUnfocused(object sender, FocusEventArgs e)
        {
            ButtonBorder.Stroke = Brush.Transparent.Color;
            InfoPanel.IsVisible = false;

        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            ButtonBorder.Stroke = Brush.White.Color;
            InfoPanel.IsVisible = true;
        }
    }
}