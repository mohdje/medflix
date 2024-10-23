using Microsoft.Maui.Controls.Shapes;

namespace Medflix.Controls.AndroidTv
{
    public class TransparentImageButton : ContentView
    {
        string source;
        public string ImgSource
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                Button.ImageSource = ImageSource.FromFile(source);
            }
        }

        private string tooltip;
        public string Tooltip
        {
            get
            {
                return tooltip;

            }
            set
            {
                tooltip = value;
                ToolTipProperties.SetText(Button, value);
            }
        }

        public event EventHandler Clicked;

        Button Button;
        Border Border;

        public TransparentImageButton()
        {
            Button = new Button
            {
                CornerRadius = 100,
                Padding = 10,
                WidthRequest = 50,
                HeightRequest = 50,
                BackgroundColor = Brush.DarkGray.Color.WithAlpha(0.3f),
            };

            Button.Focused += (s, e) => Border.Stroke = Brush.White.Color;
            Button.Unfocused += (s, e) => Border.Stroke = Brush.Transparent.Color;
            Button.Clicked += async (s, e) =>
            {
                var initialBorderColor = Border.Stroke;
                Border.Stroke = Brush.Red.Color;
                await Task.Delay(300);
                Border.Stroke = Brush.White.Color;
                Clicked?.Invoke(this, EventArgs.Empty);
            };

            Border = new Border
            {
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 100
                },
                Stroke = Brush.Transparent.Color,
                StrokeThickness = 4,
                HeightRequest = Button.HeightRequest,
                WidthRequest = Button.WidthRequest,
                Content = Button
            };


            Content = Border;

        }
    }
}