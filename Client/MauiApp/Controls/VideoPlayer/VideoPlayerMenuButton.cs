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
            FontSize = 18;
            Padding = 10;
            HorizontalOptions = LayoutOptions.Fill;
            ApplyDefaultAttributes();

            this.Focused += OnFocused;
            this.Unfocused += OnUnfocused;
        }

        private void ApplyDefaultAttributes()
        {
            TextColor = Brush.Gray.Color;
            FontAttributes = FontAttributes.None;
            BackgroundColor = Brush.Transparent.Color;
        }

        private void ApplySelectedAttributes()
        {
            TextColor = Brush.White.Color;
            FontAttributes = FontAttributes.Bold;
            BackgroundColor = Brush.Transparent.Color;
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
