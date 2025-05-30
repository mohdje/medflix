
using Medflix.Services;
using Microsoft.Maui.Controls.Shapes;

namespace Medflix.Controls.VideoPlayer
{
    public class VideoTimeBar : ContentView
    {
        public event EventHandler<long> OnNavigationStart;
        public event EventHandler<double> OnNavigationEnd;

        public bool IsEnabled
        {
            get
            {
                return HiddenButton.IsVisible;
            }
            set
            {
                HiddenButton.IsVisible = value;
            }
        }

        Frame ProgressBar;
        Grid ProgressBarContainer;
        Button HiddenButton;
        Border Border;

        long totalDurationInMs;
        double pourcentProgress;
        long navigationAmountInMs = 10000; //navigate 10 seconds per left/right button click
        double pourcentNavigation => (double)navigationAmountInMs / totalDurationInMs;

        DateTime LastNavigationDateTime;
        public VideoTimeBar()
        {
            ProgressBar = new Frame
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Start,
                WidthRequest = 0,
                BackgroundColor = Brush.Red.Color,
                BorderColor = Brush.Transparent.Color,
                ZIndex = 2,
            };

            ProgressBarContainer = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = 10,
                Margin = new Thickness(0, 10),
                Children =
                {
                    ProgressBar,
                    new Frame
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill,
                        BackgroundColor = Brush.Gray.Color.WithAlpha(0.5f),
                        BorderColor = Brush.Transparent.Color,
                        ZIndex = 1,
                    }
                }
            };

            HiddenButton = new Button
            {
                BackgroundColor = Brush.Transparent.Color,
                HeightRequest = 1,
            };
            HiddenButton.Focused += OnFocused;
            HiddenButton.Unfocused += OnUnfocused;

            Border = new Border
            {
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 10
                },
                StrokeThickness = 0,
                Content = new StackLayout
                {
                    Children =
                    {
                        HiddenButton,
                        ProgressBarContainer
                    }
                }
            };
            Content = Border;

            AddGestureEvent();
        }

        private void AddGestureEvent()
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Phone || DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.NumberOfTapsRequired = 1;
                tapGestureRecognizer.Tapped += async (s, e) =>
                {
                    if (IsEnabled)
                    {
                        var position = e.GetPosition(ProgressBarContainer);
                        if (position.HasValue)
                        {
                            pourcentProgress = position.Value.X / ProgressBarContainer.Width;
                            await StartNavigationAsync();
                        }
                    }
                };
                Content.GestureRecognizers.Add(tapGestureRecognizer);
            }
        }

        private void OnFocused(object? sender, FocusEventArgs e)
        {
            Border.BackgroundColor = Brush.DarkGray.Color.WithAlpha(0.4f);

            RemoteCommandActionNotifier.Instance.PreventRightButton = true;
            RemoteCommandActionNotifier.Instance.PreventLeftButton = true;
            RemoteCommandActionNotifier.Instance.OnLeftButtonPressed += OnLeftButtonPressed;
            RemoteCommandActionNotifier.Instance.OnRightButtonPressed += OnRightButtonPressed;
        }

        private async void OnLeftButtonPressed(object? sender, EventArgs e)
        {
            await SetNavigationOffsetAsync(-pourcentNavigation);
        }

        private async void OnRightButtonPressed(object? sender, EventArgs e)
        {
            await SetNavigationOffsetAsync(pourcentNavigation);
        }
        private async Task SetNavigationOffsetAsync(double offset)
        {
            if (pourcentProgress >= 0 && pourcentProgress <= 1)
                pourcentProgress += offset;

            await StartNavigationAsync();
        }

        private async Task StartNavigationAsync()
        {
            LastNavigationDateTime = DateTime.Now;

            var newTimeInMs = (long)(pourcentProgress * totalDurationInMs);
            OnNavigationStart?.Invoke(this, newTimeInMs);

            UpdateProgressBar();

            await Task.Delay(2500).ContinueWith(t =>
            {
                if ((DateTime.Now - LastNavigationDateTime).TotalSeconds >= 2)
                {
                    OnNavigationEnd?.Invoke(this, pourcentProgress);
                }
            });
        }

        private void OnUnfocused(object? sender, FocusEventArgs e)
        {
            Border.Stroke = Brush.Transparent.Color;
            Border.BackgroundColor = Brush.Transparent.Color;

            RemoteCommandActionNotifier.Instance.PreventLeftButton = false;
            RemoteCommandActionNotifier.Instance.PreventRightButton = false;
            RemoteCommandActionNotifier.Instance.OnLeftButtonPressed -= OnLeftButtonPressed;
            RemoteCommandActionNotifier.Instance.OnRightButtonPressed -= OnRightButtonPressed;
        }

        public void UpdateProgress(long timeInMs, long totalTimeInMs)
        {
            totalDurationInMs = totalTimeInMs;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                pourcentProgress = timeInMs / (double)totalTimeInMs;
                UpdateProgressBar();
            });
        }

        private void UpdateProgressBar()
        {
            ProgressBar.WidthRequest = ProgressBarContainer.Width * pourcentProgress;
        }
    }
}