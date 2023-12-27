using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using DryIoc.ImTools;
using DynamicData;
using DynamicData.Kernel;
using LibVLCSharp.Avalonia.Unofficial;
using LibVLCSharp.Shared;
using Medflix.Models;
using Medflix.Services;
using Medflix.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebHostStreaming.Models;

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

    private string currentDownloadStateBase64Url;

    private bool exitFullScreenOnUserAction;

    public VideoPlayerView(VideoPlayerOptions videoPlayerOptions, MedflixApiService medflixApiService)
    {
        InitializeComponent();

        VlcPlayer = new VlcPlayer();

        SetupEvents();
        SetupOptionsAsync(videoPlayerOptions);
        SetupMediaInfoPanel(videoPlayerOptions.WatchedMedia);

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
        VlcPlayer.OnStreamDevicesListChange += MediaPlayerOnStreamDevicesListChange;

        this.VolumeBar.ValueChanged += OnVolumeBarValueChanged;

        await PlaySelectedSourceAsync();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        VlcPlayer.Dispose();
    }

    public async Task NotifyWindowStateChanged(bool isFullScreen)
    {
        this.ExitFullScreenButton.IsVisible = isFullScreen;
        this.EnterFullScreenButton.IsVisible = !isFullScreen;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && isFullScreen)
        {
            await Task.Delay(2000);
            exitFullScreenOnUserAction = true;
        }
        else
            exitFullScreenOnUserAction = false;
    }

    public void NotifyKeyPressed(KeyEventArgs e)
    {
        ShowControls();

        switch (e.Key)
        {
            case Key.Space:
                VlcPlayer.TogglePlay();
                break;
            case Key.Left:
                VlcPlayer.MoveBackward();
                break;
            case Key.Right:
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

         if (exitFullScreenOnUserAction)
            this.OnExitFullScreenRequest?.Invoke(this, null);
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
        this.SearchStreamDevicesToggleSwitch.IsCheckedChanged += (s, e) =>
        {
            if (this.SearchStreamDevicesToggleSwitch.IsChecked.GetValueOrDefault())
            {
                this.SearchingStreamDevices.IsVisible = true;
                VlcPlayer.StartSearchStreamDevices();
            }
            else
            {
                this.SearchingStreamDevices.IsVisible = false;
                VlcPlayer.StopStreamToDevice();
                VlcPlayer.StopSearchStreamDevices();
                this.StreamDevicesList.Children.Clear();
            }
        };
        this.CastButton.Click += (s, e) =>
        {
            this.StreamDevicesListContainer.IsVisible = !this.StreamDevicesListContainer.IsVisible;
            this.SourcesListContainer.IsVisible = false;
            this.SubtitlesOptionsContainer.IsVisible = false;
        }; 
        this.SubtitlesButton.Click += (s, e) =>
        {
            this.SubtitlesOptionsContainer.IsVisible = !this.SubtitlesOptionsContainer.IsVisible;
            this.SourcesListContainer.IsVisible = false;
            this.StreamDevicesListContainer.IsVisible = false;
        };
        this.SettingsButton.Click += (s, e) =>
        {
            this.SourcesListContainer.IsVisible = !this.SourcesListContainer.IsVisible;
            this.SubtitlesOptionsContainer.IsVisible = false;
            this.StreamDevicesListContainer.IsVisible = false;
        };
        this.PointerMoved += async (s, e) =>
        {
            if (exitFullScreenOnUserAction)
            {
                this.OnExitFullScreenRequest?.Invoke(this, null);
                await Task.Delay(1000);
            }
            ShowControls();
        };
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

                            this.SubtitlesOffset.IsEnabled = true;
                            this.SubtitlesOptionsContainer.IsVisible = false;

                            var subtitlesFilePath = await this.medflixApiService.GetSubtitlesFileAsync(subtitlesUrl);
                            if (!string.IsNullOrEmpty(subtitlesFilePath))
                            {
                                VlcPlayer.AddSubtitles($"file:///{subtitlesFilePath}");
                            }
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
                this.SubtitlesOptionsContainer.IsVisible = false;
                this.SubtitlesOffset.IsEnabled = false;
                VlcPlayer.DisableSubtitles();
            };

            this.SubtitlesOffset.ValueChanged += (s, e) =>
            {
                if (e.NewValue.HasValue)
                {
                    var delay = TimeSpan.FromSeconds((double)e.NewValue);
                    VlcPlayer.SetSubtitlesDelay(delay);
                }
            };
        }
        else
            this.SubtitlesButton.IsVisible = false;
    }

    private void SetupMediaInfoPanel(WatchedMediaDto media)
    {
        //TODO To display the image ,it should be downloaded first and then pass local path as argument. Or use a third library
        //this.MediaInfoPanelImage.Source = new Bitmap(AssetLoader.Open(new Uri(media.CoverImageUrl)));
        this.MediaInfoPanelTitle.Text = media.Title;

        var time = TimeSpan.FromSeconds(media.TotalDuration);
        var hours = time.Hours > 0 ? $"{time.Hours}h " : string.Empty;
        var minutes = time.Minutes > 0 ? $"{time.Minutes}min" : string.Empty;
        this.MediaInfoPanelSubInfo.Text = $"{media.Year} | {hours}{minutes}";

        this.MediaInfoPanelSynopsis.Text = media.Synopsis;
    }

    #endregion

    #region Private methods

    private void ShowControls()
    {
        userActionTime = DateTime.Now;
        var delayInSeconds = 3;
        VideoPlayerControls.Opacity = 1;
        VideoPlayerControls.Cursor = new Cursor(StandardCursorType.Arrow);
        this.MediaInfoPanel.IsVisible = false;

        Task.Delay((delayInSeconds + 1) * 1000).ContinueWith(t =>
        {
            var span = DateTime.Now - userActionTime;

            if (span.TotalSeconds >= delayInSeconds)
            {
                Dispatcher.UIThread.Invoke(async () =>
                {
                    while (this.SourcesListContainer.IsVisible 
                    || this.SubtitlesOptionsContainer.IsVisible
                    || this.StreamDevicesListContainer.IsVisible)
                    {
                        await Task.Delay(2000);
                    }

                    VideoPlayerControls.Opacity = 0;
                    VideoPlayerControls.Cursor = new Cursor(StandardCursorType.None);

                    if(this.VlcPlayer.IsPaused)
                        this.MediaInfoPanel.IsVisible = true;
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

        var startTime = !string.IsNullOrEmpty(this.CurrentTime.Text) ? FromFormattedVideoTime(this.CurrentTime.Text) : (this.videoPlayerOptions.ResumeToTime);

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
                this.videoPlayerOptions.WatchedMedia.TorrentUrl = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Url));
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

    private async Task SaveProgressionAsync(long currentTime)
    {
        if (this.videoPlayerOptions.WatchedMedia != null && (DateTime.Now - lastProgressSaveTime >= TimeSpan.FromSeconds(10)))
        {
            lastProgressSaveTime = DateTime.Now;
            this.videoPlayerOptions.WatchedMedia.CurrentTime = currentTime / 1000;
            if (this.videoPlayerOptions.WatchedMedia.TotalDuration == 0)
                this.videoPlayerOptions.WatchedMedia.TotalDuration = this.VlcPlayer.TotalDuration;

            await this.medflixApiService.SaveProgressionAsync(this.videoPlayerOptions.MediaType, this.videoPlayerOptions.WatchedMedia);
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
        VlcPlayer.MoveBackward();
    }

    private void OnForwardButtonClick(object? sender, RoutedEventArgs e)
    {

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

            await SaveProgressionAsync(e.CurrentTime);
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
            this.CastButton.IsVisible = videoOpened;

            this.PlayButton.IsVisible = e.State == VLCState.Stopped || e.State == VLCState.Paused;
            this.PauseButton.IsVisible = e.State == VLCState.Playing;

            this.SoundLoudButton.IsVisible = videoOpened && !e.Muted;
            this.SoundOffButton.IsVisible = videoOpened && e.Muted;
            this.VolumeBar.IsVisible = this.SoundLoudButton.IsVisible;

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
    private void MediaPlayerOnStreamDevicesListChange(object? sender, RendererItem[] e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            this.StreamDevicesList.Children.Clear();

            this.SearchingStreamDevices.IsVisible = !e.Any();

            foreach (var streamDevice in e)
            {
                var textBlock = new TextBlock();

                textBlock.Text = streamDevice.Name.Length >= 18 ? $"{streamDevice.Name.Substring(0, 17)}..." : streamDevice.Name;

                textBlock.Padding = new Thickness(15, 7);

                textBlock.Classes.Add($"videoOption");
                if (VlcPlayer.SelectedStreamDevice == streamDevice)
                    textBlock.Classes.Add("selected");

                textBlock.PointerPressed += async (s, e) =>
                {
                    if (textBlock.Classes.Contains("selected"))
                        return;

                    foreach (TextBlock txtBlock in this.StreamDevicesList.Children)
                        txtBlock.Classes.Remove("selected");

                    textBlock.Classes.Add("selected");

                    VlcPlayer.StartStreamToDevice(streamDevice);

                    this.StreamDevicesListContainer.IsVisible = false;
                };

                this.StreamDevicesList.Children.Add(textBlock);
            }
        });
    }

    #endregion
}
