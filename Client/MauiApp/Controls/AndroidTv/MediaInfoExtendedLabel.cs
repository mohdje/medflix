namespace Medflix.Controls.AndroidTv
{
    public class MediaInfoExtendedLabel : ContentView
    {
        public string Title
        {
            get
            {
                return TitleLabel.Text;
            }
            set
            {
                TitleLabel.Text = value;
            }
        }
        public string Text
        {
            get
            {
                return TextLabel.Text;
            }
            set
            {
                IsVisible = !string.IsNullOrEmpty(value);
                TextLabel.Text = value;
            }
        }

        public bool ShowLoading
        {
            get
            {
                return Spinner.IsVisible;
            }
            set
            {
                Spinner.IsVisible = value;
                TextLabel.IsVisible = !value;
            }
        }

        Label TitleLabel;
        Label TextLabel;
        ActivityIndicator Spinner;

        public MediaInfoExtendedLabel()
        {
            TitleLabel = new Label
            {
                FontSize = 15,
                TextColor = Brush.White.Color,
                Margin = new Thickness(0, 0, 10, 0)
            };
            TextLabel = new Label
            {
                FontSize = 15,
                TextColor = Color.FromArgb("#807f7f"),
                MaximumWidthRequest = 700,
                LineBreakMode = LineBreakMode.WordWrap
            };
            Spinner = new ActivityIndicator
            {
                IsRunning = true,
                IsVisible = false,
                HeightRequest = 20,
                WidthRequest = 20,
                Color = Brush.White.Color
            };


            Content = new HorizontalStackLayout
            {
                Children = {
                    TitleLabel,
                    TextLabel,
                    Spinner
                }
            };
        }
    }
}