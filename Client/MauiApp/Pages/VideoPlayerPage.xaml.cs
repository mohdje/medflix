
#if ANDROID 
using Android.Views;
#endif
using LibVLCSharp.Shared;
using Medflix.Controls;
using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Services;
using Medflix.ViewModels;
using Medflix.Views;
using System;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;
using static Microsoft.Maui.ApplicationModel.Permissions;


namespace Medflix.Pages
{
    public partial class VideoPlayerPage : ContentPage
    {
        public event EventHandler CloseVideoPlayerRequested;
        MediaPlayerViewModel MediaPlayerViewModel => ((MediaPlayerViewModel)BindingContext);

        DateTime LastUserActionDateTime;

        DateTime LastSavedProgressionDateTime;
        VideoPlayerParameters VideoPlayerParameters;

        bool shouldRedraw;
        MediaSource defaultMediaSource;

        public VideoPlayerPage(VideoPlayerParameters videoPlayerParameters)
        {
            InitializeComponent();

            VideoPlayerParameters = videoPlayerParameters;

            defaultMediaSource = SelectDefaultMediaSource(videoPlayerParameters);

            InitVideoPlayerControls(defaultMediaSource);

            if(DeviceInfo.Current.Platform != DevicePlatform.WinUI)
                PlayMedia(defaultMediaSource, (long)(videoPlayerParameters.WatchMedia.CurrentTime * 1000));
        }

        private MediaSource SelectDefaultMediaSource(VideoPlayerParameters videoPlayerParameters)
        {
            var mediaSource = videoPlayerParameters.MediaSources.FirstOrDefault(s => !string.IsNullOrEmpty(s.FilePath));
            if (mediaSource != null)
                return mediaSource;

            mediaSource = !string.IsNullOrEmpty(videoPlayerParameters.WatchMedia.VideoSource) ? videoPlayerParameters.MediaSources.FirstOrDefault(m => videoPlayerParameters.WatchMedia.VideoSource == m.TorrentUrl) : null;
            if (mediaSource != null)
                return mediaSource;
            
            return videoPlayerParameters.MediaSources.First();
        }
        private void InitVideoPlayerControls(MediaSource defaultMediaSource)
        {
            var episodeInfo = VideoPlayerParameters.WatchMedia.Media.SeasonsCount > 0 ? $" (Season {VideoPlayerParameters.WatchMedia.SeasonNumber} Ep. {VideoPlayerParameters.WatchMedia.EpisodeNumber})" : null;
            MediaTitle.Text = VideoPlayerParameters.WatchMedia.Media.Title + episodeInfo;

            PlayerControls.OnVisibilityChanged += (s, isVisible) => TopBar.IsVisible = isVisible;

            PlayerControls.SetSubtitlesButtonVisibility(VideoPlayerParameters.SubtitlesSources.Any());

            PlayerControls.OnPlayPauseButtonClick += (s, e) => MediaPlayerViewModel.TogglePlay();
            PlayerControls.OnSubtitlesButtonClick += (s, e) => VideoPlayerMenu.ShowSubtitlesMenu(VideoPlayerParameters.SubtitlesSources);
            PlayerControls.OnQualitiesButtonClick += (s, e) => VideoPlayerMenu.ShowVersionQualitiesMenu(VideoPlayerParameters.MediaSources, defaultMediaSource);
            PlayerControls.OnEnterFullscreenButtonClick += (s, e) => SetFullscreen(true);
            PlayerControls.OnExitFullscreenButtonClick += (s, e) => SetFullscreen(false);

            PlayerControls.OnTimeBarStartNavigating += (s, e) => MediaPlayerViewModel.MediaPlayer.SetPause(true);
            PlayerControls.OnTimeBarNavigated += (s, percentage) => MediaPlayerViewModel.Seek(percentage);

            VideoPlayerMenu.OnDisplaySubtitlesClick += async (s, url) => await Subtitles.DisplaySubtitles(url);
            VideoPlayerMenu.OnNoSubtitlesClick += (s, e) => Subtitles.HideSubtitles();
            VideoPlayerMenu.OnVideoQualityClick += (s, mediaSource) => PlayMedia(mediaSource, (long)(VideoPlayerParameters.WatchMedia.CurrentTime * 1000));

            this.LeaveVideoPlayerConfirmationView.OnConfirm += (s, e) =>
            {
                SetFullscreen(false);
                CloseVideoPlayerRequested?.Invoke(this, EventArgs.Empty);
            };
            this.LeaveVideoPlayerConfirmationView.OnCancel += (s, e) =>
            {
                this.MediaPlayerViewModel.MediaPlayer.Play();
                LeaveVideoPlayerConfirmationView.Hide();
            };

            if(DeviceInfo.Current.Idiom == DeviceIdiom.Desktop)
            {
                var mouseRecognizer = new PointerGestureRecognizer();
                mouseRecognizer.PointerMoved += OnUserAction;
                Content.GestureRecognizers.Add(mouseRecognizer);
            }
            else if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
            {
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += OnUserAction;
                Content.GestureRecognizers.Add(tapGestureRecognizer);
            }
            else if (DeviceInfo.Current.Idiom == DeviceIdiom.TV)
            {
                ClosePlayerButton.IsVisible = false;
            }
        }

        private void SetFullscreen(bool fullscreen)
        {
        #if WINDOWS

            var window = GetParentWindow().Handler.PlatformView as MauiWinUIWindow;
            var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);


            switch (appWindow.Presenter)
            {
                case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                    if (fullscreen)
                    {
                        overlappedPresenter.SetBorderAndTitleBar(false, false);
                        overlappedPresenter.Maximize();
                    }
                    else
                    {
                        overlappedPresenter.SetBorderAndTitleBar(true, true);
                        overlappedPresenter.Restore();
                    }

                    break;
            }
        #endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
          
            if(DeviceInfo.Current.Platform != DevicePlatform.WinUI) 
                MediaPlayerViewModel.OnAppearing();

            if (DeviceInfo.Current.Idiom == DeviceIdiom.TV)
            {
                RemoteCommandActionNotifier.Instance.PreventBackButton = true;
                RemoteCommandActionNotifier.Instance.OnBackButtonPressed += OnCloseButtonPressed;
                RemoteCommandActionNotifier.Instance.OnButtonPressed += OnUserAction;
            }

            #if ANDROID
            if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
                            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
               
            Platform.CurrentActivity.Window.AddFlags(WindowManagerFlags.KeepScreenOn |
                        WindowManagerFlags.DismissKeyguard |
                        WindowManagerFlags.ShowWhenLocked |
                        WindowManagerFlags.TurnScreenOn |
                        WindowManagerFlags.Fullscreen);

            Platform.CurrentActivity.Window.DecorView.SystemUiFlags = SystemUiFlags.ImmersiveSticky |
                                    SystemUiFlags.HideNavigation |
                                    SystemUiFlags.Fullscreen |
                                    SystemUiFlags.Immersive;
            #endif
        }

        protected override async void OnDisappearing()
        {
            #if ANDROID
                Platform.CurrentActivity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn |
                            WindowManagerFlags.DismissKeyguard |
                            WindowManagerFlags.ShowWhenLocked |
                            WindowManagerFlags.TurnScreenOn | 
                            WindowManagerFlags.Fullscreen);

                Platform.CurrentActivity.Window.DecorView.SystemUiFlags = SystemUiFlags.Visible;
#endif

            if (DeviceInfo.Current.Idiom == DeviceIdiom.TV)
            {
                RemoteCommandActionNotifier.Instance.OnButtonPressed -= OnUserAction;
                RemoteCommandActionNotifier.Instance.OnBackButtonPressed -= OnCloseButtonPressed;
                RemoteCommandActionNotifier.Instance.PreventBackButton = false;
            }
          
            base.OnDisappearing();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            shouldRedraw = true;

            Subtitles.NotifyScreenSizeChanged(width);
        }

        private void VideoView_MediaPlayerChanged(object sender, MediaPlayerChangedEventArgs e)
        {
            MediaPlayerViewModel.OnVideoViewInitialized();

            shouldRedraw = true;

            if (MediaPlayerViewModel.MediaPlayer != null)
            {
                LastUserActionDateTime = DateTime.Now.AddSeconds(5);
                LastSavedProgressionDateTime = DateTime.Now;

                MediaPlayerViewModel.MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
                MediaPlayerViewModel.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
                MediaPlayerViewModel.MediaPlayer.Buffering += MediaPlayer_Buffering;
                MediaPlayerViewModel.MediaPlayer.Paused += MediaPlayer_Paused;
            }
        }

        private void OnUserAction(object? sender, EventArgs e)
        {
            LastUserActionDateTime = DateTime.Now;
            PlayerControls.Show();
        }

        private void OnCloseButtonPressed(object? sender, EventArgs e)
        {
            if (!this.VideoPlayerMenu.IsVisible)
            {
                this.MediaPlayerViewModel.MediaPlayer.SetPause(true);
                this.LeaveVideoPlayerConfirmationView.Show();
            }
        }

        private void PlayMedia(MediaSource mediaSource, long startTime)
        {
            int? seasonNumber = VideoPlayerParameters.WatchMedia.SeasonNumber == 0 ? null : VideoPlayerParameters.WatchMedia.SeasonNumber;
            int? episodeNumber = VideoPlayerParameters.WatchMedia.EpisodeNumber == 0 ? null : VideoPlayerParameters.WatchMedia.EpisodeNumber;

            var streamUrl = MedflixApiService.Instance.BuildStreamUrl(
                mediaSource,
                VideoPlayerParameters.WatchMedia.Media.Id, 
                seasonNumber, 
                episodeNumber);
            
            VideoPlayerParameters.WatchMedia.VideoSource = !string.IsNullOrEmpty(mediaSource.FilePath) ? mediaSource.FilePath : mediaSource.TorrentUrl;

            VersionQuality.Text = $"{mediaSource.Language} - {mediaSource.Quality}";

            MediaPlayerViewModel.PlayMedia(streamUrl, startTime);

            Task.Run(async () =>
            {
                await LoadingSpinner.StartListenningDownloadState(MediaPlayerViewModel.MediaUrl);
            });
        }

        private void VideoView_HandlerChanged(object sender, EventArgs e)
        {
            #if WINDOWS
                var windowsView = ((LibVLCSharp.Platforms.Windows.VideoView)VideoView.Handler.PlatformView);

                windowsView.Initialized += (s, e) =>
                {
                    MediaPlayerViewModel.Initialize(e.SwapChainOptions);
                    
                    PlayMedia(defaultMediaSource, (long)(VideoPlayerParameters.WatchMedia.CurrentTime * 1000));

                    MediaPlayerViewModel.OnAppearing();
                };
            #endif
        }

        #region MediaPlayer Events

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingSpinner.StopListenningDownloadState();
                LoadingSpinner.ShowMessage("An error occured", false);
            });
        }

        private void MediaPlayer_Buffering(object sender, MediaPlayerBufferingEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingSpinner.ShowMessage(string.Empty, true);
                PlayerControls.DisableTimeBarNavigation();
            });
        }

        private void MediaPlayer_Paused(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayerControls.NotifyPaused();
            });
        }

        private void MediaPlayer_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            //If back button pressed MediaPlayerViewModel is disposed. MediaPlayer_TimeChanged can be triggered at the same time
            if (MediaPlayerViewModel?.MediaPlayer?.Media == null)
                return;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ManageVideoViewDisplay();

                if (PlayerControls.IsShown)
                    PlayerControls.NotifyTimeUpdated(e.Time, MediaPlayerViewModel.MediaPlayer.Media.Duration);

                if (Subtitles.IsVisible)
                    Subtitles.Update(e.Time);

                LoadingSpinner.StopListenningDownloadState();

                if (!VideoPlayerMenu.IsVisible && PlayerControls.IsShown && (DateTime.Now - LastUserActionDateTime).TotalSeconds >= 4)
                    PlayerControls.Hide();

                if ((DateTime.Now - LastSavedProgressionDateTime).TotalSeconds >= 15)
                {
                    LastSavedProgressionDateTime = DateTime.Now;
                    VideoPlayerParameters.WatchMedia.CurrentTime = e.Time / 1000;
                    VideoPlayerParameters.WatchMedia.TotalDuration = MediaPlayerViewModel.MediaPlayer.Media.Duration / 1000;

                    await MedflixApiService.Instance.SaveProgressionAsync(VideoPlayerParameters.WatchMedia);
                }
            });
        }

        private void ManageVideoViewDisplay()
        {
            if (shouldRedraw)
            {
                VideoView.WidthRequest = 0;
                VideoView.HeightRequest = 0;
                shouldRedraw = false;
            }
            else if (VideoView.WidthRequest == 0)
            {
                VideoView.WidthRequest = Width;
                VideoView.HeightRequest = Height;
            }

            if (VideoView.Opacity == 0)
                VideoView.Opacity = 1;
        }

        #endregion

    }
}
