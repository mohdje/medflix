namespace Medflix.Controls.VideoPlayer
{
    public class VideoPlayerControlButton : ImageButton
    {
        public VideoPlayerControlButton()
        {
            Init();
        }
        public VideoPlayerControlButton(ImageSource imageSource)
        {
            Source = imageSource;
            Init();
        }

        private void Init()
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            HeightRequest = 50;
            WidthRequest = 50;
            Padding = 10;
            CornerRadius = 100;
            BackgroundColor = Brush.Transparent.Color;
            this.Focused += (s, e) => OnFocus();
            this.Unfocused += (s, e) => OnUnfocus();

            var mouseRecognizer = new PointerGestureRecognizer();
            mouseRecognizer.PointerEntered += (s, e) => OnFocus();
            mouseRecognizer.PointerExited += (s, e) => OnUnfocus();

            this.GestureRecognizers.Add(mouseRecognizer);
        }

        private void OnFocus()
        {
            BackgroundColor = Brush.DarkGray.Color.WithAlpha(0.5f);
        }

        private void OnUnfocus()
        {
            BackgroundColor = BackgroundColor = Brush.Transparent.Color;
        }
    }
}