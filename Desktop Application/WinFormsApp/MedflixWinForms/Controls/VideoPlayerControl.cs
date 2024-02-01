using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MedflixWinForms.Properties;
using Medflix.Models;
using MedflixWinForms.Utils;
using MedflixWinForms.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using WinFormsApp1;
using MedflixWinForms.Services;
using System.Web;

namespace MedflixWinforms.Controls
{
    public partial class VideoPlayerControl : UserControl
    {
        private MediaPlayer mediaPlayer;
        private VideoPlayerOptions videoPlayerOptions;

        private Form NormalScreenForm;
        private Form FullScreenForm;
        private QualitiesMenuForm qualitiesMenuForm;
        private SubtitlesMenuForm subtitlesMenuForm;
        private CastMenuForm castMenuForm;

        private TransparentPanel VideoViewOverlay;

        private bool fullscreenMode;
        private DateTime lastUserAction;
        private Point lastMousePosition = new Point(-1, -1);
        private string subtitlesSourceUrl;
        private string subtitlesAbsoluteUri;
        private RendererDiscoverer rendererDiscoverer;
        private List<RendererItem> castDevices;
        private string selectedCastDeviceName;
        private string currentDownloadStateBase64Url;
        private DateTime lastProgressSaveTime;

        private MedflixApiService medflixApiService;

        public event EventHandler OnExitVideoPlayer;

        public VideoPlayerControl(Form formContainer, VideoPlayerOptions videoPlayerOptions)
        {
            InitializeComponent();

            SetupMediaPlayer();
            SetupForm(formContainer);

            this.videoPlayerOptions = videoPlayerOptions;
            this.medflixApiService = new MedflixApiService();

        }

        public async void Play(long startTime = 0)
        {
            var selectedVideo = videoPlayerOptions.Sources.FirstOrDefault(video => video.Selected);

            if (selectedVideo == null)
                throw new Exception("No video selected from sources list");

            startTime = startTime > 0 ? startTime : videoPlayerOptions.ResumeToTime;
            var media = VLCHelper.CreateMedia(selectedVideo.Url, new string[] { $"start-time={startTime}" });

            mediaPlayer.Volume = 70;
            mediaPlayer.Mute = false;
            mediaPlayer.Play(media);

            var uri = new Uri(selectedVideo.Url);
            var queryParameters = HttpUtility.ParseQueryString(uri.Query);
            var base64Url = queryParameters["base64Url"];

            if (!string.IsNullOrEmpty(base64Url))
            {
                this.currentDownloadStateBase64Url = base64Url;

                try
                {
                    this.videoPlayerOptions.WatchedMedia.TorrentUrl = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Url));
                    await UpdateLoadingMessageAsync(base64Url);
                }
                catch (Exception)
                {
                    this.LoadingMessage.Text = "Loading";
                }
            }

            if (!string.IsNullOrEmpty(this.subtitlesAbsoluteUri))
                this.mediaPlayer.AddSlave(MediaSlaveType.Subtitle, this.subtitlesAbsoluteUri, true);
        }

        private async Task UpdateLoadingMessageAsync(string base64Url)
        {
            await Task.Delay(2500);

            var downloadState = await this.medflixApiService.GetDownloadStateAsync(base64Url);

            if (downloadState != null && this.currentDownloadStateBase64Url == base64Url)
            {
                this.Invoke(() =>
                {
                    this.LoadingMessage.Text = downloadState?.Message ?? "Loading";
                });

                if (!downloadState.Error && this.LoadingPanel.Visible)
                    await UpdateLoadingMessageAsync(base64Url);
            }
        }

        #region Setup functions
        private void SetupMediaPlayer()
        {
            this.mediaPlayer = VLCHelper.CreateMediaPlayer();
            this.mediaPlayer.PositionChanged += MediaPlayerPositionChanged;
            this.mediaPlayer.TimeChanged += MediaPlayerTimeChanged;
            this.mediaPlayer.Paused += (s, e) => this.PlayPauseButton.BackgroundImage = Resources.play;
            this.mediaPlayer.Playing += MediaPlayerPlaying;
            this.mediaPlayer.Muted += MediaPlayerMuted;
            this.mediaPlayer.Unmuted += MediaPlayerUnmuted;
            this.mediaPlayer.VolumeChanged += MediaPlayerVolumeChanged;
            this.mediaPlayer.Buffering += MediaPlayerBuffering;

            this.VideoView.MediaPlayer = mediaPlayer;
            this.VideoView.MediaPlayer.EnableMouseInput = false;
        }

        private void SetupForm(Form form)
        {
            this.NormalScreenForm = form;
            AddVideoViewOverlay();
            ListenToMouseMove(this);
            this.NormalScreenForm.Focus();
            ListenToKeyDown(this.NormalScreenForm);
        }

        #endregion

        #region Utils
        /// <summary>
        /// This function is needed to catch Mouse Move event. The VideoView does not raise this event.
        /// </summary>
        private void AddVideoViewOverlay()
        {
            VideoViewOverlay = new TransparentPanel();
            this.Controls.Add(VideoViewOverlay);

            // Overlay the Panel on top of the VideoView
            VideoViewOverlay.Tag = "user_action";
            VideoViewOverlay.Dock = DockStyle.Fill;

            VideoViewOverlay.BringToFront();
        }

        /// <summary>
        /// This function is needed because events are not bubbling from child component to the parent
        /// </summary>
        /// <param name="parent"></param>
        private void ListenToMouseMove(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.Tag != null && ctrl.Tag.ToString() == "user_action")
                {
                    ctrl.MouseMove += (s, e) => OnMouseMove(e.Location);
                }

                if (ctrl.HasChildren)
                    ListenToMouseMove(ctrl);
            }
        }

        private void ListenToKeyDown(Form form)
        {
            form.PreviewKeyDown += (s, e) => e.IsInputKey = true;
            form.KeyDown += OnKeyDown;
        }
        private void ShowControls()
        {
            this.Invoke(() =>
            {
                this.ControlsContainer.Visible = true;
                Cursor.Show();
            });

            HideControls();
        }

        private void HideControls()
        {
            Task.Run(async () =>
            {
                while (DateTime.Now - lastUserAction < TimeSpan.FromSeconds(5)
                 || (qualitiesMenuForm != null && qualitiesMenuForm.Created)
                 || (subtitlesMenuForm != null && subtitlesMenuForm.Created)
                 || (castMenuForm != null && castMenuForm.Created))
                {
                    await Task.Delay(4000);
                }
                this.Invoke(() =>
                {
                    this.ControlsContainer.Visible = false;
                    Cursor.Hide();
                });
            });
        }

        private void ToggleFullScreen()
        {
            if (fullscreenMode)
            {
                fullscreenMode = false;

                this.Dock = DockStyle.Fill;
                NormalScreenForm.Controls.Add(this);
                this.BringToFront();

                FullScreenForm.Dispose();
                FullScreenForm = null;

                this.FullScreenButton.BackgroundImage = Resources.full_screen;
            }
            else
            {
                fullscreenMode = true;

                FullScreenForm = new FullscreenForm();
                FullScreenForm.FormBorderStyle = FormBorderStyle.None;
                FullScreenForm.WindowState = FormWindowState.Maximized;
                FullScreenForm.ShowInTaskbar = false;

                FullScreenForm.KeyPreview = true;
                ListenToKeyDown(FullScreenForm);

                this.Dock = DockStyle.Fill;
                FullScreenForm.Controls.Add(this);

                FullScreenForm.Show();
                FullScreenForm.BringToFront();
                FullScreenForm.Focus();

                this.FullScreenButton.BackgroundImage = Resources.full_screen_exit;
            }
        }

        private async void ExitVideoPlayer()
        {
            if (fullscreenMode)
            {
                ToggleFullScreen();
                await Task.Delay(2000);
            }

            this.mediaPlayer.Stop();
            this.mediaPlayer.Dispose();

            this.OnExitVideoPlayer?.Invoke(this, null);
        }

        private void ShowLoading()
        {
            if (!this.LoadingPanel.Visible)
            {
                this.Invoke(() =>
                {
                    this.SizeChanged += CenterLoading;
                    this.LoadingMessage.TextChanged += CenterLoading;
                    this.LoadingMessage.Text = "Loading";
                    this.LoadingPanel.Visible = true;
                    CenterLoading(null, null);
                });
            }
        }

        private void HideLoading()
        {
            if (this.LoadingPanel.Visible)
            {
                this.Invoke(() =>
                {
                    this.SizeChanged -= CenterLoading;
                    this.LoadingMessage.TextChanged -= CenterLoading;
                    this.LoadingPanel.Visible = false;
                });
            }
        }

        private void CenterLoading(object? sender, EventArgs e)
        {
            this.LoadingPanel.Width = this.LoadingMessage.Width + this.LoadingSpinner.Width;
            var x = (this.Width / 2) - (this.LoadingPanel.Width / 2);
            this.LoadingPanel.Location = new Point(x, 0);
            this.LoadingPanel.BringToFront();
        }

        private void SetPlaybackControlsEnable(bool enabled)
        {
            this.Invoke(() =>
            {
                this.PlayPauseButton.Enabled = enabled;
                this.BackwardButton.Enabled = enabled;
                this.ForwardButton.Enabled = enabled;
                this.SoundButton.Enabled = enabled;
                this.VolumeBarContainer.Enabled = enabled;
                this.ProgressTimeBarContainer.Enabled = enabled;
            });
        }

        private async Task SaveProgressionAsync(long currentTime)
        {
            if (this.videoPlayerOptions.WatchedMedia != null && (DateTime.Now - lastProgressSaveTime >= TimeSpan.FromSeconds(10)))
            {
                lastProgressSaveTime = DateTime.Now;
                this.videoPlayerOptions.WatchedMedia.CurrentTime = currentTime / 1000;
                if (this.videoPlayerOptions.WatchedMedia.TotalDuration == 0)
                    this.videoPlayerOptions.WatchedMedia.TotalDuration = this.mediaPlayer.Media.Duration;

                await this.medflixApiService.SaveProgressionAsync(this.videoPlayerOptions.MediaType, this.videoPlayerOptions.WatchedMedia);
            }
        }
        private string ToFormattedVideoTime(long time)
        {
            var timeSpan = TimeSpan.FromMilliseconds(time);
            return $"{timeSpan.Hours.ToString("00")}:{timeSpan.Minutes.ToString("00")}:{timeSpan.Seconds.ToString("00")}";
        }
        #endregion

        #region Form events 

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            OnUserAction();

            if (e.KeyCode == Keys.Space)
                this.mediaPlayer.Pause();
            else if (e.KeyCode == Keys.Right)
                this.mediaPlayer.Time += 10000;
            else if (e.KeyCode == Keys.Left)
                this.mediaPlayer.Time -= 10000;
            else if (e.KeyCode == Keys.Up)
                this.mediaPlayer.Volume += 10;
            else if (e.KeyCode == Keys.Down)
                this.mediaPlayer.Volume -= 10;
            else if (e.KeyCode == Keys.M)
                this.mediaPlayer.ToggleMute();
            else if (e.KeyCode == Keys.F)
                ToggleFullScreen();
            else if (e.KeyCode == Keys.Escape && fullscreenMode)
                ToggleFullScreen();
            else if (e.KeyCode == Keys.S)
                ExitVideoPlayer();
        }
        private void OnMouseMove(Point mousePosition)
        {
            //event raised event if mouse did not move, so check is needed
            if (lastMousePosition != mousePosition)
            {
                lastMousePosition = mousePosition;
                OnUserAction();
            }
        }
        private void OnUserAction()
        {
            lastUserAction = DateTime.Now;
            ShowControls();
        }

        private void VideoPlayerControl_Paint(object sender, PaintEventArgs e)
        {
            this.ControlsContainer.BringToFront();
        }

        private void ProgressTimeBarContainer_Paint(object sender, PaintEventArgs e)
        {
            this.TimebarBackground.Left = 0;
            this.TimebarBackground.Top = (this.ProgressTimeBarContainer.Height / 2) - (this.TimebarBackground.Height / 2);
            this.TimebarBackground.Width = this.ProgressTimeBarContainer.Width;
        }

        private void VolumeBarContainer_Paint(object sender, PaintEventArgs e)
        {
            this.VolumeBarBackground.Left = 0;
            this.VolumeBarBackground.Top = (this.VolumeBarContainer.Height / 2) - (this.VolumeBarBackground.Height / 2);
            this.VolumeBarBackground.Width = this.VolumeBarContainer.Width;
        }

        private void DisposeMenuForm(VideoPlayerMenuForm form)
        {
            form.Close();
            form.Dispose();
        }

        #endregion

        #region MediaPlayer events
        private void MediaPlayerBuffering(object? sender, MediaPlayerBufferingEventArgs e)
        {
            ShowLoading();
            SetPlaybackControlsEnable(false);

        }
        private void MediaPlayerPlaying(object? sender, EventArgs e)
        {
            this.PlayPauseButton.BackgroundImage = Resources.pause;
            HideControls();
        }

        private async void MediaPlayerTimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            HideLoading();
            SetPlaybackControlsEnable(true);
            await SaveProgressionAsync(e.Time);

            this.Invoke(() =>
            {
                this.CurrentTimeLabel.Text = ToFormattedVideoTime(e.Time);
                this.RemainingTimeLabel.Text = $"-{ToFormattedVideoTime(this.mediaPlayer.Media.Duration - e.Time)}";
            });
        }

        private void MediaPlayerPositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
        {
            this.Invoke(() =>
            {
                this.TimebarForeground.Width = (int)(this.ProgressTimeBarContainer.Width * e.Position);
            });
        }

        private void MediaPlayerVolumeChanged(object? sender, MediaPlayerVolumeChangedEventArgs e)
        {
            ShowLoading();
            this.Invoke(() =>
            {
                this.VolumeBarForeground.Width = (int)(this.VolumeBarContainer.Width * e.Volume);
            });
        }

        private void MediaPlayerUnmuted(object? sender, EventArgs e)
        {
            this.SoundButton.BackgroundImage = Resources.sound_loud;
            this.Invoke(() =>
            {
                this.VolumeBarContainer.Visible = true;
            });
        }

        private void MediaPlayerMuted(object? sender, EventArgs e)
        {
            this.SoundButton.BackgroundImage = Resources.sound_off;
            this.Invoke(() =>
            {
                this.VolumeBarContainer.Visible = false;
            });
        }
        #endregion

        #region Video controls events
        private void TimebarClick(object sender, EventArgs e)
        {
            Point point = this.ProgressTimeBarContainer.PointToClient(Cursor.Position);

            var position = (float)point.X / (float)this.TimebarBackground.Width;
            this.Invoke(() =>
            {
                this.TimebarForeground.Width = (int)(this.ProgressTimeBarContainer.Width * position);
            });

            this.mediaPlayer.Position = position;
        }

        private void VolumeBarClick(object sender, EventArgs e)
        {
            Point point = this.VolumeBarContainer.PointToClient(Cursor.Position);

            this.mediaPlayer.Volume = (int)(((float)point.X / (float)this.VolumeBarBackground.Width) * 100);
        }

        private void PlayPauseButton_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.Pause();
        }

        private void BackwardButton_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.Time -= 10000;
        }

        private void ForwardButton_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.Time += 10000;
        }

        private void SoundButton_Click(object sender, EventArgs e)
        {
            this.mediaPlayer.ToggleMute();
        }

        private void FullScreenButton_Click(object sender, EventArgs e)
        {
            ToggleFullScreen();
        }
        private void StopButton_Click(object sender, EventArgs e)
        {
            ExitVideoPlayer();
        }

        #endregion

        #region Video options events
        private void QualitiesButton_Click(object sender, EventArgs e)
        {
            if (qualitiesMenuForm == null || !qualitiesMenuForm.Created)
            {
                qualitiesMenuForm = new QualitiesMenuForm(videoPlayerOptions.Sources, this.QualitiesButton.PointToScreen(new Point(0, 0)));
                qualitiesMenuForm.OnQualitySelected += QualitiesForm_OnQualitySelected;
                qualitiesMenuForm.Deactivate += (s, e) => DisposeMenuForm(qualitiesMenuForm);
                qualitiesMenuForm.Show();
            }
        }

        private void QualitiesForm_OnQualitySelected(object? sender, QualitySelectedEventArgs e)
        {
            foreach (var quality in videoPlayerOptions.Sources)
            {
                quality.Selected = quality == e.VideoQualityOption;
            }

            DisposeMenuForm(qualitiesMenuForm);

            var currentTime = this.mediaPlayer.Time / 1000;
            this.mediaPlayer.Stop();

            Play(currentTime);
        }


        private void SubtitlesButton_Click(object sender, EventArgs e)
        {
            if (subtitlesMenuForm == null || !subtitlesMenuForm.Created)
            {
                var delay = TimeSpan.FromMicroseconds(this.mediaPlayer.SpuDelay).TotalSeconds;
                subtitlesMenuForm = new SubtitlesMenuForm(this.videoPlayerOptions.Subtitles, this.subtitlesSourceUrl, delay, this.SubtitlesButton.PointToScreen(new Point(0, 0)));
                subtitlesMenuForm.OnSubtitlesSelected += SubtitlesMenuForm_OnSubtitlesSelected;
                subtitlesMenuForm.OnOffsetChanged += SubtitlesMenuForm_OnOffsetChanged;
                subtitlesMenuForm.Deactivate += (s, e) => DisposeMenuForm(subtitlesMenuForm);
                subtitlesMenuForm.Show();
            }
        }

        private void SubtitlesMenuForm_OnOffsetChanged(object? sender, double e)
        {
            if (!string.IsNullOrEmpty(this.subtitlesSourceUrl))
            {
                var timeSpan = TimeSpan.FromSeconds(e);
                this.mediaPlayer.SetSpuDelay((long)timeSpan.TotalMicroseconds);
            }
        }

        private async void SubtitlesMenuForm_OnSubtitlesSelected(object? sender, string selectedSubtitlesUrl)
        {
            this.subtitlesSourceUrl = selectedSubtitlesUrl;
            DisposeMenuForm(subtitlesMenuForm);

            if (string.IsNullOrEmpty(selectedSubtitlesUrl))
                this.mediaPlayer.SetSpu(-1);
            else
            {
                var subtitlesFilePath = await this.medflixApiService.GetSubtitlesFileAsync(this.subtitlesSourceUrl);
                if (File.Exists(subtitlesFilePath))
                {
                    this.subtitlesAbsoluteUri = new Uri(subtitlesFilePath).AbsoluteUri;
                    this.mediaPlayer.AddSlave(MediaSlaveType.Subtitle, this.subtitlesAbsoluteUri, true);
                }
            }
        }

        private void CastButton_Click(object sender, EventArgs e)
        {
            if (castMenuForm == null || !castMenuForm.Created)
            {
                castMenuForm = new CastMenuForm(rendererDiscoverer != null, this.CastButton.PointToScreen(new Point(0, 0)));

                castMenuForm.OnStartSearchingDevices += CastMenuForm_OnStartSearchingDevices;
                castMenuForm.OnStopSearchingDevices += CastMenuForm_OnStopSearchingDevices;
                castMenuForm.OnCastDeviceSelected += CastMenuForm_OnCastDeviceSelected;
                castMenuForm.Deactivate += (s, e) => DisposeMenuForm(castMenuForm);
                castMenuForm.Show();

                UpdateCastDevicesList();
            }
        }

        private void CastMenuForm_OnCastDeviceSelected(object? sender, string selectedDeviceName)
        {
            DisposeMenuForm(castMenuForm);
            this.selectedCastDeviceName = selectedDeviceName;
            this.mediaPlayer.SetRenderer(this.castDevices.Single(device => device.Name == selectedDeviceName));
        }

        private void CastMenuForm_OnStartSearchingDevices(object? sender, EventArgs e)
        {
            this.castDevices = new List<RendererItem>();

            if (rendererDiscoverer == null)
            {
                rendererDiscoverer = VLCHelper.CreateRendererDiscover();
                rendererDiscoverer.ItemAdded += (s, e) =>
                {
                    castDevices.Add(e.RendererItem);
                    UpdateCastDevicesList();
                };
                rendererDiscoverer.ItemDeleted += (s, e) =>
                {
                    castDevices.Remove(e.RendererItem);
                    UpdateCastDevicesList();
                };
            }

            rendererDiscoverer.Start();
        }

        private void CastMenuForm_OnStopSearchingDevices(object? sender, EventArgs e)
        {
            this.castDevices.Clear();
            UpdateCastDevicesList();
            rendererDiscoverer.Stop();
            rendererDiscoverer.Dispose();
            rendererDiscoverer = null;
            this.mediaPlayer.SetRenderer(null);
        }

        private void UpdateCastDevicesList()
        {
            if (castMenuForm != null && castMenuForm.Created)
                castMenuForm.UpdateCastDevicesList(castDevices?.Select(device => device.Name).ToArray(), selectedCastDeviceName);
        }

        #endregion
    }
}
