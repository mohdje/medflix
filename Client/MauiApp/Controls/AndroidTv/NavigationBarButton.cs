using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Medflix.Controls.AndroidTv
{
    public class NavigationBarButton : ContentView
    {
        private ImageButton ImageButton;

        private bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                Opacity = value ? 1 : 0.3;
            }
        }
        public event EventHandler Clicked;
        public ImageSource Source
        {
            get
            {
                return ImageButton.Source;
            }
            set
            {
                ImageButton.Source = value;
            }
        }
        public NavigationBarButton()
        {
            ImageButton = new ImageButton
            {
                WidthRequest = 30,
                HeightRequest = 30,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 5),
                CornerRadius = 100
            };
            ImageButton.Focused += (s, e) => OnFocused();
            ImageButton.Unfocused += (s, e) => OnUnFocused();
            ImageButton.Clicked += (s, e) => OnClicked();
            Content = ImageButton;
        }

        private void OnFocused()
        {
            ImageButton.BackgroundColor = Color.FromRgba(160, 160, 160, 0.4);
        }

        private void OnUnFocused()
        {
            ImageButton.BackgroundColor = Brush.Transparent.Color;
        }

        private void OnClicked()
        {
            Selected = true;
            Clicked?.Invoke(this, null);
        }
    }
}