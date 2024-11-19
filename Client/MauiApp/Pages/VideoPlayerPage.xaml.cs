
using Android.Views;
using LibVLCSharp.Shared;
using Medflix.Controls;
using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Services;
using Medflix.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Threading.Channels;


namespace Medflix.Pages
{
    public partial class VideoPlayerPage : ContentPage
    {
        MediaPlayerViewModel MediaPlayerViewModel => ((MediaPlayerViewModel)BindingContext);

        DateTime LastUserActionDateTime;

        DateTime LastSavedProgressionDateTime;
        VideoPlayerParameters VideoPlayerParameters;

        bool shouldRedraw;

        public VideoPlayerPage(VideoPlayerParameters videoPlayerParameters)
        {
            InitializeComponent();
            VideoPlayerParameters = videoPlayerParameters;

            InitVideoPlayerControls();

            var videoUrl = SelectDefaultVideoUrl(videoPlayerParameters);
            PlayMedia(videoUrl, (long)(videoPlayerParameters.WatchMedia.CurrentTime * 1000));

            VideoPlayerMenu.Init(videoPlayerParameters.SubtitlesSources, videoPlayerParameters.MediaSources, videoUrl);
        }

        private string SelectDefaultVideoUrl(VideoPlayerParameters videoPlayerParameters)
        {
            if (!string.IsNullOrEmpty(videoPlayerParameters.WatchMedia.VideoSource))
                return videoPlayerParameters.WatchMedia.VideoSource;
            else
            {
                var mediaSource = videoPlayerParameters.MediaSources.SelectMany(ms => ms.Sources).FirstOrDefault(s => !string.IsNullOrEmpty(s.FilePath));
                if (mediaSource != null)
                    return mediaSource.FilePath;
                else
                    return videoPlayerParameters.MediaSources.First().Sources.First().TorrentUrl;
            }
        }
        private void InitVideoPlayerControls()
        {
            PlayerControls.SetSubtitlesButtonVisibility(VideoPlayerParameters.SubtitlesSources.Any());

            PlayerControls.OnPlayPauseButtonClick += (s, e) => MediaPlayerViewModel.TogglePlay();
            PlayerControls.OnSubtitlesButtonClick += (s, e) => VideoPlayerMenu.ShowSubtitlesMenu();
            PlayerControls.OnQualitiesButtonClick += (s, e) => VideoPlayerMenu.ShowVideoQualitiesMenu();

            PlayerControls.OnTimeBarStartNavigating += (s, e) => MediaPlayerViewModel.MediaPlayer.SetPause(true);
            PlayerControls.OnTimeBarNavigated += (s, percentage) => MediaPlayerViewModel.Seek(percentage);

            VideoPlayerMenu.OnDisplaySubtitlesClick += async (s, url) => await Subtitles.DisplaySubtitles(url);
            VideoPlayerMenu.OnNoSubtitlesClick += (s, e) => Subtitles.HideSubtitles();
            VideoPlayerMenu.OnVideoQualityClick += (s, url) => PlayMedia(url, (long)(VideoPlayerParameters.WatchMedia.CurrentTime * 1000));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MediaPlayerViewModel.OnAppearing();
            RemoteCommandActionNotifier.Instance.OnButtonPressed += OnUserAction;

#if ANDROID
                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Window.AddFlags(WindowManagerFlags.KeepScreenOn |
                            WindowManagerFlags.DismissKeyguard |
                            WindowManagerFlags.ShowWhenLocked |
                            WindowManagerFlags.TurnScreenOn);
#endif
        }


        protected override void OnDisappearing()
        {
#if ANDROID
                        Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn |
                                    WindowManagerFlags.DismissKeyguard |
                                    WindowManagerFlags.ShowWhenLocked |
                                    WindowManagerFlags.TurnScreenOn);
#endif
            RemoteCommandActionNotifier.Instance.OnButtonPressed -= OnUserAction;
            MediaPlayerViewModel.OnDisappearing();
            base.OnDisappearing();
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

                Task.Run(async () =>
                {
                    await LoadingSpinner.StartListenningDownloadState(MediaPlayerViewModel.MediaUrl);
                });
            }
        }

        private void OnUserAction(object? sender, EventArgs e)
        {
            LastUserActionDateTime = DateTime.Now;
            PlayerControls.Show();
        }

        private void PlayMedia(string mediaUrl, long startTime)
        {
            int? seasonNumber = VideoPlayerParameters.WatchMedia.SeasonNumber == 0 ? null : VideoPlayerParameters.WatchMedia.SeasonNumber;
            int? episodeNumber = VideoPlayerParameters.WatchMedia.EpisodeNumber == 0 ? null : VideoPlayerParameters.WatchMedia.EpisodeNumber;

            var url = MedflixApiService.Instance.BuildStreamUrl(mediaUrl, seasonNumber, episodeNumber);
            VideoPlayerParameters.WatchMedia.VideoSource = mediaUrl;

            MediaPlayerViewModel.PlayMedia(url, startTime);
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
            MainThread.BeginInvokeOnMainThread(async () =>
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

                if (PlayerControls.IsShown)
                {
                    PlayerControls.NotifyTimeUpdated(e.Time, MediaPlayerViewModel.MediaPlayer.Media.Duration);
                    PlayerControls.EnableTimeBarNavigation();
                }


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

        #endregion
    }
}
