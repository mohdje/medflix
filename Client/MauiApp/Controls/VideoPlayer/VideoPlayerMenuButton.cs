using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medflix.Controls.VideoPlayer
{
    public class VideoPlayerMenuButton : Button
    {
        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if(value)
                    ApplySelectedAttributes();
                else
                    ApplyDefaultAttributes();
            }
        }

        public VideoPlayerMenuButton(string text)
        {
            Text = text;
            FontSize = 16;
            Padding = 5;
            HorizontalOptions = LayoutOptions.Fill;
            CornerRadius = 5;
            Margin = 5;
            WidthRequest = 150;

            ApplyDefaultAttributes();

            this.Focused += OnFocused;
            this.Unfocused += OnUnfocused;
        }

        private void ApplyDefaultAttributes()
        {
            TextColor = Brush.DarkGray.Color;
            FontAttributes = FontAttributes.None;
            BackgroundColor = Brush.Transparent.Color;
        }

        private void ApplySelectedAttributes()
        {
            TextColor = Brush.White.Color;
            FontAttributes = FontAttributes.Bold;
            BackgroundColor = Brush.DarkRed.Color;
        }

        private void OnFocused(object? sender, FocusEventArgs e)
        {
            TextColor = Brush.White.Color;
            FontAttributes = FontAttributes.None;
            BackgroundColor = Brush.Gray.Color;
        }

        private void OnUnfocused(object? sender, FocusEventArgs e)
        {
            if (_selected)
                ApplySelectedAttributes();
            else
                ApplyDefaultAttributes();
        }
    }
}
