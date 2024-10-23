using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medflix.Controls.AndroidTv
{
    public class BorderedButton : ContentView
    {
        private Button button;
        private const int CornerRadius = 8;
        public Button Button 
        {
            get
            {
                return button; 
            }
            set
            {
                button = value;
                button.Focused += ButtonFocused;
                button.Unfocused += ButtonUnfocused;
                button.Clicked += ButtonClicked;
                button.CornerRadius = CornerRadius;
                Border.Content = button;
            }
        }

        public string Text
        {
            get
            {
                return button.Text;
            }
            set
            {
                button.Text = value;
            }
        }


        Border Border;
        public BorderedButton()
        {
            Border = new Border
            {
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = CornerRadius,
                },
                StrokeThickness = 4,
                HeightRequest = 55,
                Stroke = Brush.Transparent.Color
            };

            Content = Border;
        }

        private void ButtonFocused(object? sender, FocusEventArgs e)
        {
            Border.Stroke = Brush.White.Color;
        }

        private void ButtonUnfocused(object? sender, FocusEventArgs e)
        {
            Border.Stroke = Brush.Transparent.Color;
        }

        private async void ButtonClicked(object? sender, EventArgs e)
        {
            Border.Stroke = Brush.Red.Color;
            await Task.Delay(200);
            Border.Stroke = Brush.Transparent.Color;
        }


    }
}