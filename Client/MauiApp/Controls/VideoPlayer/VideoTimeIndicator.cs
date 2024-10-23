namespace Medflix.Controls.VideoPlayer
{
    public class VideoTimeIndicator : ContentView
    {
        private Label CurrentTimeLabel;
        private Label RemainingTimeLabel;
        string timeFormat = @"hh\:mm\:ss";

        public VideoTimeIndicator()
        {
            CurrentTimeLabel = CreateLabel();
            RemainingTimeLabel = CreateLabel();

            Content = new FlexLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween,
                Children = {
                    CurrentTimeLabel,
                    RemainingTimeLabel
                }
            };
        }

        private Label CreateLabel()
        {
            return new Label
            {
                FontSize = 22,
                TextColor = Brush.White.Color,
                BackgroundColor = Brush.Transparent.Color,
            };
        }

        public void Update(long currentTimeInMs)
        {
            var timeSpan = TimeSpan.FromMilliseconds(currentTimeInMs);
            CurrentTimeLabel.Text = timeSpan.ToString(timeFormat);
        }

        public void Update(long currentTimeInMs, long totalTimeInMs)
        {
            var timeSpan = TimeSpan.FromMilliseconds(currentTimeInMs);
            CurrentTimeLabel.Text = timeSpan.ToString(timeFormat);
            RemainingTimeLabel.Text = "-" + TimeSpan.FromMilliseconds(totalTimeInMs).Subtract(timeSpan).ToString(timeFormat);
        }
    }
}