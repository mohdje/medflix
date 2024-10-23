using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Pages.AndroidTv;

namespace Medflix.Views.AndroidTv
{
    public partial class MediaHorizontalCoverView : ContentView
    {
        string mediaId;
        public MediaHorizontalCoverView(MediaDetails media)
        {
            InitializeComponent();

            MediaButton.Focused += OnFocused;
            MediaButton.Unfocused += OnUnFocused;
            MediaButton.Clicked += OnClicked;

            if(!string.IsNullOrEmpty(media.BackgroundImageUrl))
                MediaButton.ImageSource = ImageSource.FromUri(new Uri(media.BackgroundImageUrl));

            if (!string.IsNullOrEmpty(media.LogoImageUrl))
                LogoTitle.Source = ImageSource.FromUri(new Uri(media.LogoImageUrl));

            mediaId = media.Id;
        }

        private void OnClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var initialBorderColor = ButtonBorder.Stroke;
                ButtonBorder.Stroke = Brush.Red;
                await Task.Delay(300);
                ButtonBorder.Stroke = Color.FromArgb("#5f5f5f");

                await Navigation.PushAsync(new AndroidTvMediaPresentationPage(mediaId));

            });
        }

        private void OnUnFocused(object sender, FocusEventArgs e)
        {
            ButtonBorder.Stroke = Color.FromArgb("#5f5f5f");
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            ButtonBorder.Stroke = Brush.White;
        }
    }
}