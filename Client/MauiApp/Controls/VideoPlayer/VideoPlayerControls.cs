
using Medflix.Services;
using Medflix.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Controls.VideoPlayer
{
    public class VideoPlayerControls : ContentView
    {
        VideoTimeBar TimeBar;
        VideoTimeIndicator VideoTimeIndicator;
        ImageButton PlayPauseButton;
        ImageButton SubtitlesButton;
        ImageButton QualitiesButton;

        public event EventHandler OnPlayPauseButtonClick;
        public event EventHandler OnSubtitlesButtonClick;
        public event EventHandler OnQualitiesButtonClick;
        public event EventHandler OnTimeBarStartNavigating;
        public event EventHandler<double> OnTimeBarNavigated;
        public event EventHandler<bool> OnVisibilityChanged;

        public bool IsShown => this.Opacity == 1;

        public VideoPlayerControls()
        {
            Content = BuildContent();
        }

        private VerticalStackLayout BuildContent()
        {
            BuildControls();

            var buttonsLayout = new FlexLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween,
                Children =
                {
                     new StackLayout { PlayPauseButton },
                     new HorizontalStackLayout { SubtitlesButton, QualitiesButton }
                }
            };

            var mainLayout = new VerticalStackLayout
            {
                Padding = new Thickness(20),
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                Children =
                {
                    VideoTimeIndicator,
                    TimeBar,
                    buttonsLayout
                }
            };

            return mainLayout;
        }


        private void BuildControls()
        {
            TimeBar = new VideoTimeBar();
            TimeBar.OnNavigationStart += (s, timeInMs) =>
            {
                VideoTimeIndicator.Update(timeInMs);
                OnTimeBarStartNavigating?.Invoke(s, EventArgs.Empty);
            };

            TimeBar.OnNavigationEnd += (s, percentage) => OnTimeBarNavigated?.Invoke(s, percentage);

            VideoTimeIndicator = new VideoTimeIndicator();

            PlayPauseButton = new VideoPlayerControlButton(Icons.PauseIcon);
            PlayPauseButton.Clicked += (s, e) => OnPlayPauseButtonClick?.Invoke(this, EventArgs.Empty);

            SubtitlesButton = new VideoPlayerControlButton(Icons.SubtitlesIcon);
            SubtitlesButton.Margin = new Thickness(0, 0, 30, 0);
            SubtitlesButton.Clicked += (s, e) => OnSubtitlesButtonClick?.Invoke(this, EventArgs.Empty);

            QualitiesButton = new VideoPlayerControlButton(Icons.QualitiesIcon);
            QualitiesButton.Clicked += (s, e) => OnQualitiesButtonClick?.Invoke(this, EventArgs.Empty);
        }

        public void NotifyTimeUpdated(long timeInMs, long totalTimeInMs)
        {
            VideoTimeIndicator.Update(timeInMs, totalTimeInMs);

            TimeBar.UpdateProgress(timeInMs, totalTimeInMs);

            PlayPauseButton.Source = Icons.PauseIcon;
        }

        public void NotifyPaused()
        {
            PlayPauseButton.Source = Icons.PlayIcon;
        }

        public void DisableTimeBarNavigation()
        {
            TimeBar.IsEnabled = false;
        }

        public void SetSubtitlesButtonVisibility(bool visible)
        {
            SubtitlesButton.IsVisible = visible;
        }

        public void Show()
        {
            if (IsShown)
                return;

            var fadeInAnimation = new Animation(v => Opacity = v, start: 0, end: 1, Easing.CubicOut);
            fadeInAnimation.Commit(this, nameof(fadeInAnimation));

            PlayPauseButton.IsEnabled = true;
            SubtitlesButton.IsEnabled = true;
            QualitiesButton.IsEnabled = true;
            TimeBar.IsEnabled = true;

            OnVisibilityChanged?.Invoke(this, true);
        }

        public void Hide()
        {
            var fadeOutAnimation = new Animation(v => Opacity = v, start: 1, end: 0, Easing.CubicOut);
            fadeOutAnimation.Commit(this, nameof(fadeOutAnimation));

            PlayPauseButton.IsEnabled = false;
            SubtitlesButton.IsEnabled = false;
            QualitiesButton.IsEnabled = false;
            DisableTimeBarNavigation();
            OnVisibilityChanged?.Invoke(this, false);
        }
    }
}
