using Medflix.Models.VideoPlayer;
using Medflix.Services;
using Medflix.Utils;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medflix.Controls.VideoPlayer
{
    public class VideoSubtitles : ContentView
    {
        private Label SubtitlesLabel;
        private IEnumerable<Subtitles> Subtitles;

        public VideoSubtitles()
        {
            IsVisible = false;

            SubtitlesLabel = new Label
            {
                FontSize = 24,
                FontAttributes = FontAttributes.Bold,
                TextColor = Brush.White.Color,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Shadow = new Shadow { Brush = Brush.Black.Color, Offset = new Point(2, 2), Opacity = 0.8f },
            };

            Content = new FlexLayout
            {
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.End,
                JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center,

                Children = {
                    SubtitlesLabel
                }
            };
        }

        public async Task DisplaySubtitles(string url)
        {
            Subtitles = await MedflixApiService.Instance.GetSubtitlesAsync(url);
            IsVisible = true;
        }

        public void HideSubtitles()
        {
            Subtitles = null;
            SubtitlesLabel.Text = string.Empty;
            IsVisible = false;
        }
        public void Update(long timeInMs)
        {
            var timeInS = timeInMs / 1000;
            var subtitles = Subtitles?.FirstOrDefault(s => timeInS >= s.StartTime && timeInS <= s.EndTime);

            if(subtitles?.Text.Contains("<i>") ?? false)
            {
                SubtitlesLabel.FontAttributes = FontAttributes.Italic;
                SubtitlesLabel.Text = $"{subtitles?.Text.Replace("<i>", "").Replace("</i>", "")} ";//add extra space because last char is cropped in Italic (Android only)
            }
            else
            {
                SubtitlesLabel.FontAttributes = FontAttributes.None;
                SubtitlesLabel.Text = subtitles?.Text;
            }
        }

        public void NotifyScreenSizeChanged(double width)
        {
            SubtitlesLabel.FontSize = width / 35;
        }
    }
}