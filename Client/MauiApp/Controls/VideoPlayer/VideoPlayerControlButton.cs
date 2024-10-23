namespace Medflix.Controls.VideoPlayer
{
    public class VideoPlayerControlButton : ImageButton
    {
        public VideoPlayerControlButton(ImageSource imageSource)
        {
            Source = imageSource;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
            HeightRequest = 50;
            WidthRequest = 50;
            Padding = 10;
            CornerRadius = 100;
            BackgroundColor = Brush.Transparent.Color;

            this.Focused += (s, e) =>
            {
                BackgroundColor = Brush.DarkGray.Color.WithAlpha(0.5f);
            };
            this.Unfocused += (s, e) =>
            {
                BackgroundColor = Brush.Transparent.Color;
            };
        }
    }
}