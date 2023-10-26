using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using DryIoc.ImTools;
using DynamicData;
using DynamicData.Kernel;
using LibVLCSharp.Avalonia.Unofficial;
using LibVLCSharp.Shared;
using Medflix.Models;
using Medflix.Services;
using Medflix.Tools;
using Medflix.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Medflix.Views;

public partial class VideoPlayerView : UserControl
{
    VlcPlayer VlcPlayer;

    public event EventHandler OnEnterFullScreenRequest;
    public event EventHandler OnExitFullScreenRequest;
    public event EventHandler OnToggleFullScreenRequest;
    public event EventHandler OnCloseRequest;

    private DateTime userActionTime;
    private DateTime lastProgressSaveTime;

    private VideoPlayerOptions videoPlayerOptions;

    private MedflixApiService medflixApiService;
    private Subtitle[] subtitles;

    private string currentDownloadStateBase64Url;

    public VideoPlayerView(VideoPlayerOptions videoPlayerOptions, MedflixApiService medflixApiService)
    {
        InitializeComponent();

        VlcPlayer = new VlcPlayer();

        SetupEvents();
        SetupOptionsAsync(videoPlayerOptions);

        this.medflixApiService = medflixApiService;
    }

    #region Window Events
    protected async override void OnLoaded(RoutedEventArgs e)
    {
        VlcPlayer.BindToVideoView(this.VlcVideoView);

        VlcPlayer.OnStateChanged += MediaPlayerOnStateChanged;
        VlcPlayer.OnPositionChanged += MediaPlayerPositionChanged;
        VlcPlayer.OnTimeChanged += MediaPlayerTimeChanged;
        VlcPlayer.OnMutedStateChanged += MediaPlayerMutedStateChanged;
        VlcPlayer.OnError += MediaPlayerOnError;
        VlcPlayer.OnBuffering += MediaPlayerOnBuffering;

        this.VolumeBar.ValueChanged += OnVolumeBarValueChanged;

        await PlaySelectedSourceAsync();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        VlcPlayer.Dispose();
    }

    public void OnFullScreenStateChanged(bool isFullScreen)
    {
        this.ExitFullScreenButton.IsVisible = isFullScreen;
        this.EnterFullScreenButton.IsVisible = !isFullScreen;
    }

    public void OnKeyPressed(KeyEventArgs e)
    {
        ShowControls();

        switch (e.Key)
        {
            case Key.Space:
                VlcPlayer.TogglePlay();
                break;
            case Key.Left:
                this.LoadingSpinner.IsVisible = true;
                VlcPlayer.MoveBackward();
                break;
            case Key.Right:
                this.LoadingSpinner.IsVisible = true;
                VlcPlayer.MoveForward();
                break;
            case Key.Up:
                this.VolumeBar.Value += 10;
                break;
            case Key.Down:
                this.VolumeBar.Value -= 10;
                break;
            case Key.M:
                this.VlcPlayer.ToggleMute();
                break;
            case Key.F:
                this.OnToggleFullScreenRequest?.Invoke(this, null);
                break;
            case Key.Escape:
                this.OnExitFullScreenRequest?.Invoke(this, null);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Setup methods
    private void SetupEvents()
    {
        this.PlayButton.Click += OnPlayButtonClick;
        this.PauseButton.Click += OnPauseButtonClick;
        this.EnterFullScreenButton.Click += OnEnterFullScreenButtonClick;
        this.ExitFullScreenButton.Click += OnExitFullScreenButtonClick;
        this.CloseButton.Click += OnCloseButtonClick;
        this.SoundLoudButton.Click += OnSoundLoudButtonClick;
        this.SoundOffButton.Click += OnSoundOffButtonClick;
        this.BackwardButton.Click += OnBackwardButtonClick;
        this.ForwardButton.Click += OnForwardButtonClick;

        this.SubtitlesButton.Click += (s, e) =>
        {
            this.SubtitlesOptionsContainer.IsVisible = !this.SubtitlesOptionsContainer.IsVisible;
            this.SourcesListContainer.IsVisible = false;
        };
        this.SettingsButton.Click += (s, e) =>
        {
            this.SourcesListContainer.IsVisible = !this.SourcesListContainer.IsVisible;
            this.SubtitlesOptionsContainer.IsVisible = false;
        };

        this.PointerMoved += (s, e) => ShowControls();
    }

    private async Task SetupOptionsAsync(VideoPlayerOptions videoPlayerOptions)
    {
        this.videoPlayerOptions = videoPlayerOptions;

        await Task.Run(() => Dispatcher.UIThread.Invoke(() =>  BuildSourcesList()));
        await Task.Run(() => Dispatcher.UIThread.Invoke(() => BuildSubtitlesOptions()));
    }
    private void BuildSourcesList()
    {
        var qualityIndexes = new Dictionary<string, int>();

        foreach (var source in this.videoPlayerOptions.Sources.OrderBy(s => s.Quality))
        {
            var textBlock = new TextBlock();

            if (qualityIndexes.ContainsKey(source.Quality))
            {
                qualityIndexes[source.Quality]++;
                textBlock.Text = $"{source.Quality} ({qualityIndexes[source.Quality]})";
            }
            else
            {
                qualityIndexes.Add(source.Quality, 0);
                textBlock.Text = $"{source.Quality}";
            }

            textBlock.Padding = new Thickness(15, 7);

            textBlock.Classes.Add($"videoOption");
            if (source.Selected)
                textBlock.Classes.Add("selected");

            textBlock.PointerPressed += async (s, e) =>
            {
                if (textBlock.Classes.Contains("selected"))
                    return;

                foreach (TextBlock txtBlock in this.SourcesList.Children)
                    txtBlock.Classes.Remove("selected");

                textBlock.Classes.Add("selected");

                this.videoPlayerOptions.Sources.ForEach(s => s.Selected = false);
                source.Selected = true;

                this.SourcesListContainer.IsVisible = false;
                await PlaySelectedSourceAsync();
            };

            this.SourcesList.Children.Add(textBlock);
        }
    }

    private void BuildSubtitlesOptions()
    {
        if (this.videoPlayerOptions.Subtitles != null && this.videoPlayerOptions.Subtitles.Any())
        {
            var columnDefinitions = this.videoPlayerOptions.Subtitles.Length == 1 ? "200" : String.Join(",", this.videoPlayerOptions.Subtitles.Select(t => "100"));
            this.SubtitlesGrid.ColumnDefinitions = new ColumnDefinitions(columnDefinitions);

            var columnIndex = 0;
            foreach (var subtitlesList in this.videoPlayerOptions.Subtitles)
            {
                var textBlock = new TextBlock();
                textBlock.Text = subtitlesList.Language;
                textBlock.Classes.Add("videoOptionHeader");
                textBlock.Width = this.videoPlayerOptions.Subtitles.Length == 1 ? 200 : 100;

                Grid.SetColumn(textBlock, columnIndex);
                Grid.SetRow(textBlock, 0);

                this.SubtitlesGrid.Children.Add(textBlock);

                var scrollViewer = new ScrollViewer();
                Grid.SetColumn(scrollViewer, columnIndex);
                Grid.SetRow(scrollViewer, 1);

                var stackPanel = new StackPanel();
                stackPanel.Orientation = Avalonia.Layout.Orientation.Vertical;

                scrollViewer.Content = stackPanel;

                var languageIndex = 1;
                foreach (var subtitlesUrl in subtitlesList.SubtitlesSourceUrls)
                {
                    var subTextBlock = new TextBlock();
                    subTextBlock.Classes.Add("videoOption");
                    subTextBlock.Padding = new Thickness(7);
                    subTextBlock.Text = $"{subtitlesList.Language} {languageIndex}";
                    subTextBlock.PointerPressed += async (s, e) =>
                    {
                        if (!subTextBlock.Classes.Contains("selected"))
                        {
                            foreach (var ctrl in SubtitlesGrid.GetLogicalDescendants())
                            {
                                var txtBlock = ctrl as TextBlock;
                                txtBlock?.Classes.Remove("selected");
                            }

                            this.NoSubtitlesOption.Classes.Remove("selected");
                            subTextBlock.Classes.Add("selected");

                            this.SubtitlesOptionsContainer.IsVisible = false;

                            this.subtitles = await this.medflixApiService.GetSubtitlesAsync(subtitlesUrl);
                        }
                    };
                    stackPanel.Children.Add(subTextBlock);
                    languageIndex++;
                }

                this.SubtitlesGrid.Children.Add(scrollViewer);

                columnIndex++;
            }

            this.NoSubtitlesOption.Classes.Add("selected");
            this.NoSubtitlesOption.PointerPressed += (s, e) =>
            {
                foreach (var ctrl in SubtitlesGrid.GetLogicalDescendants())
                {
                    var txtBlock = ctrl as TextBlock;
                    txtBlock?.Classes.Remove("selected");
                }

                this.NoSubtitlesOption.Classes.Add("selected");

                this.subtitles = null;
                this.SubtitlesOptionsContainer.IsVisible = false;
                this.SubtitlesText.IsVisible = false;
            };

            this.SubtitlesFontSize.ValueChanged += (s, e) =>
            {
                if (e.NewValue.HasValue)
                    this.SubtitlesText.FontSize = (double)e.NewValue.Value;
            };
        }
        else
            this.SubtitlesButton.IsVisible = false;

    }

    #endregion

    #region Private methods

    private void ShowControls()
    {
        userActionTime = DateTime.Now;
        var delayInSeconds = 3;
        VideoPlayerControls.Opacity = 1;
        VideoPlayerControls.Cursor = new Cursor(StandardCursorType.Arrow);

        Task.Delay((delayInSeconds + 1) * 1000).ContinueWith(t =>
        {
            var span = DateTime.Now - userActionTime;

            if (span.TotalSeconds >= delayInSeconds)
            {
                Dispatcher.UIThread.Invoke(async () =>
                {
                    while (this.SourcesListContainer.IsVisible || this.SubtitlesOptionsContainer.IsVisible)
                    {
                        await Task.Delay(2000);
                    }

                    VideoPlayerControls.Opacity = 0;
                    VideoPlayerControls.Cursor = new Cursor(StandardCursorType.None);
                });
            }
        });
    }
    private string ToFormattedVideoTime(long time)
    {
        var timeSpan = TimeSpan.FromMilliseconds(time);
        return $"{timeSpan.Hours.ToString("00")}:{timeSpan.Minutes.ToString("00")}:{timeSpan.Seconds.ToString("00")}";
    }

    private double FromFormattedVideoTime(string formattedVideoTime)
    {
        var times = formattedVideoTime.Split(":");
        var hours = TimeSpan.FromHours(double.Parse(times[0]));
        var minutes = TimeSpan.FromMinutes(double.Parse(times[1]));
        var seconds = TimeSpan.FromSeconds(double.Parse(times[2]));

        return hours.Add(minutes).Add(seconds).TotalSeconds;
    }

    private async Task PlaySelectedSourceAsync()
    {
        var url = this.videoPlayerOptions?.Sources.AsList().FirstOrDefault(s => s.Selected)?.Url;

        this.LoadingSpinner.IsVisible = true;
        this.StatusMessage.IsVisible = false;

        var startTime = !string.IsNullOrEmpty(this.CurrentTime.Text) ? FromFormattedVideoTime(this.CurrentTime.Text) : (this.videoPlayerOptions.ResumeToTime / 1000);

        if (!string.IsNullOrEmpty(url))
        {
            this.CurrentTime.Text = string.Empty;
            this.RemainingTime.Text = string.Empty;

            VlcPlayer.Play(url, startTime);

            var uri = new Uri(url);
            var queryParameters = HttpUtility.ParseQueryString(uri.Query);
            var base64Url = queryParameters["base64Url"];

            if (!string.IsNullOrEmpty(base64Url))
            {
                this.currentDownloadStateBase64Url = base64Url;
                await GetDownloadStateAsync(base64Url);
            }
        }
    }

    private async Task GetDownloadStateAsync(string base64Url)
    {
        await Task.Delay(2500);

        var downloadState = await this.medflixApiService.GetDownloadStateAsync(base64Url);

        if (downloadState != null && this.currentDownloadStateBase64Url == base64Url)
        {
            this.StatusMessage.IsVisible = true;
            this.StatusMessage.Text = downloadState?.Message;

            if (!downloadState.Error && this.LoadingSpinner.IsVisible)
                await GetDownloadStateAsync(base64Url);
        }
    }

    private void UpdateSubtitleText(double time)
    {
        if (this.subtitles != null && this.subtitles.Any())
        {
            var italicMarkers = new string[] { "<i>", "</i>" };
            var offset = (double)(this.SubtitlesOffset.Value ?? 0) * 1000;
            var subtitle = this.subtitles.FirstOrDefault(sub => (((sub.StartTime * 1000) + offset) <= time) && (time <= ((sub.EndTime * 1000) + offset)));

            if (subtitle != null)
            {
                var text = subtitle.Text;

                this.SubtitlesText.IsVisible = true;

                if (italicMarkers.Any(marker => subtitle.Text.Contains(marker)))
                {
                    this.SubtitlesText.FontStyle = FontStyle.Italic;
                    italicMarkers.ForEach(marker => text = text.Replace(marker, string.Empty));
                }
                else
                    this.SubtitlesText.FontStyle = FontStyle.Normal;

                this.SubtitlesText.Text = text;
            }
            else
                this.SubtitlesText.IsVisible = false;
        }
    }

    #endregion

    #region Buttons Handlers
    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        VlcPlayer.Stop();

        if (OnCloseRequest != null)
            OnCloseRequest.Invoke(this, null);
    }

    private void OnExitFullScreenButtonClick(object? sender, RoutedEventArgs e)
    {
        if (OnExitFullScreenRequest != null)
            OnExitFullScreenRequest.Invoke(this, null);
    }

    private void OnEnterFullScreenButtonClick(object? sender, RoutedEventArgs e)
    {
        if (OnEnterFullScreenRequest != null)
            OnEnterFullScreenRequest.Invoke(this, null);
    }

    private void OnPlayButtonClick(object? sender, RoutedEventArgs e)
    {
        VlcPlayer.Play();
    }

    private void OnPauseButtonClick(object? sender, RoutedEventArgs e)
    {
        VlcPlayer.Pause();
    }

    private void OnSoundLoudButtonClick(object? sender, RoutedEventArgs e)
    {
        VlcPlayer.Mute();
    }


    private void OnSoundOffButtonClick(object? sender, RoutedEventArgs e)
    {
        VlcPlayer.UnMute();
    }

    private void OnBackwardButtonClick(object? sender, RoutedEventArgs e)
    {
        this.LoadingSpinner.IsVisible = true;

        VlcPlayer.MoveBackward();
    }

    private void OnForwardButtonClick(object? sender, RoutedEventArgs e)
    {
        this.LoadingSpinner.IsVisible = true;

        VlcPlayer.MoveForward();
    }

    #endregion

    #region Sliders Handlers
    private void OnVolumeBarValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        VlcPlayer.SetVolume(Convert.ToInt32(e.NewValue));
    }

    private void ProgressBarValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        VlcPlayer.SetPosition(e.NewValue);
    }
    #endregion

    #region MediaPlayer Handlers
    private void MediaPlayerTimeChanged(object? sender, Models.EventArgs.VideoPlayerEventArgs e)
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            this.LoadingSpinner.IsVisible = false;
            this.StatusMessage.IsVisible = false;
            this.ProgressBar.IsEnabled = true;

            this.CurrentTime.Text = ToFormattedVideoTime(e.CurrentTime);
            this.RemainingTime.Text = "-" + ToFormattedVideoTime(e.RemainingTime);

            UpdateSubtitleText(e.CurrentTime);

            if (DateTime.Now - lastProgressSaveTime >= TimeSpan.FromSeconds(10))
            {
                lastProgressSaveTime = DateTime.Now;
                this.videoPlayerOptions.WatchedMedia.CurrentTime = e.CurrentTime / 1000;
                await this.medflixApiService.SaveProgressionAsync(this.videoPlayerOptions.MediaType, this.videoPlayerOptions.WatchedMedia);
            }
        });
    }

    private void MediaPlayerPositionChanged(object? sender, Models.EventArgs.VideoPlayerEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.ProgressBar.ValueChanged -= ProgressBarValueChanged;
            this.ProgressBar.Value = e.Progress;
            this.ProgressBar.ValueChanged += ProgressBarValueChanged;
        });
    }

    private void MediaPlayerOnStateChanged(object? sender, Models.EventArgs.VideoPlayerEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var videoOpened = e.State == VLCState.Playing || e.State == VLCState.Paused;

            this.ProgressBar.IsVisible = videoOpened;
            this.CurrentTime.IsVisible = videoOpened;
            this.RemainingTime.IsVisible = videoOpened;

            this.PlayButton.IsVisible = e.State == VLCState.Stopped || e.State == VLCState.Paused;
            this.PauseButton.IsVisible = e.State == VLCState.Playing;

            this.SoundLoudButton.IsVisible = videoOpened && !e.Muted;
            this.SoundOffButton.IsVisible = videoOpened && e.Muted;
            this.VolumeBar.IsVisible = videoOpened && this.SoundLoudButton.IsVisible;

            this.BackwardButton.IsVisible = videoOpened;
            this.ForwardButton.IsVisible = videoOpened;
        });
    }

    private void MediaPlayerOnBuffering(object? sender, Models.EventArgs.VideoPlayerEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.LoadingSpinner.IsVisible = true;
            this.ProgressBar.IsEnabled = false;
        });
    }

    private void MediaPlayerMutedStateChanged(object? sender, Models.EventArgs.VideoPlayerEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.SoundLoudButton.IsVisible = !e.Muted;
            this.SoundOffButton.IsVisible = e.Muted;
            this.VolumeBar.IsVisible = !e.Muted;
        });
    }

    private void MediaPlayerOnError(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.LoadingSpinner.IsVisible = false;
            this.StatusMessage.IsVisible = true;
            this.StatusMessage.Text = "An error occured";
        });

    }
    #endregion
}
