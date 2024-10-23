using Medflix.Models.VideoPlayer;
using Medflix.Services;
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

            SubtitlesLabel.FontAttributes = subtitles?.Text.Contains("<i>") ?? false ? FontAttributes.Italic : FontAttributes.None;
            SubtitlesLabel.Text = subtitles?.Text.Replace("<i>", "").Replace("</i>", "");
        }
    }
}