using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medflix.Controls.AndroidTv
{
    public class ThreeStateButton : ContentView
    {
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

                if (!Button.IsFocused)
                {
                    Button.BackgroundColor = value ? SelectedBackgroundColor : UnfocusedBackgroundColor;
                    Button.TextColor = value ? SelectedTextColor : UnfocusedTextColor;
                }
            }
        }

        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                Button.Text = text; 
            }
        }

        public Color UnfocusedBackgroundColor { get; set; }
        public Color FocusedBackgroundColor { get; set; }
        public Color SelectedBackgroundColor { get; set; }

        public Color UnfocusedTextColor { get; set; }
        public Color FocusedTextColor { get; set; }
        public Color SelectedTextColor { get; set; }

        public event EventHandler OnClick;

        Button Button;
        public ThreeStateButton()
        {
            Button = new Button
            {
                FontAttributes = FontAttributes.Bold
            };

            Button.Focused += (s, e) => 
            {
                Button.BackgroundColor = FocusedBackgroundColor; 
                Button.TextColor = FocusedTextColor; 
            };
            Button.Unfocused += (s, e) =>
            {
                Button.BackgroundColor = selected ? SelectedBackgroundColor : UnfocusedBackgroundColor;
                Button.TextColor = selected ? SelectedTextColor : UnfocusedTextColor;
            };
            Button.Clicked += (s, e) => 
            {
                OnClick?.Invoke(this, EventArgs.Empty);
            };

            Content = Button;
        }

        public void SetFocus()
        {
           Button.Focus();
        }
    }
}