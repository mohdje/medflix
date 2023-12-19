using System;
using LibVLCSharp.Shared;
using LibVLCSharp.Avalonia.Unofficial;
using Medflix.Models.EventArgs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medflix.Tools
{
    public class VlcPlayer : IDisposable
    {
        static LibVLC? _libVLC;
        private MediaPlayer? MediaPlayer;
        private bool IsVideoOpened => this.MediaPlayer != null && (this.MediaPlayer.State == VLCState.Playing || this.MediaPlayer.State == VLCState.Paused);

        public event EventHandler OnError;
        public event EventHandler<VideoPlayerEventArgs> OnStateChanged;
        public event EventHandler<VideoPlayerEventArgs> OnPositionChanged;
        public event EventHandler<VideoPlayerEventArgs> OnTimeChanged;
        public event EventHandler<VideoPlayerEventArgs> OnMutedStateChanged;
        public event EventHandler<VideoPlayerEventArgs> OnBuffering;
        public event EventHandler<VideoPlayerEventArgs> OnPlaying;


        public static void InitLibVLC()
        {
            if (_libVLC == null)
            {
                Task.Run(() =>
                {
                    _libVLC = new LibVLC();
                });
            }
        }
        public VlcPlayer()
        {

            if (!Avalonia.Controls.Design.IsDesignMode)
            {
                //var os = AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem;
                //if (os == OperatingSystemType.WinNT)
                //{
                //    var libVlcDirectoryPath = Path.Combine(Environment.CurrentDirectory, "libvlc", IsWin64() ? "win-x64" : "win-x86");
                //    Core.Initialize(libVlcDirectoryPath);
                //}
                //else
                //{
                //	Core.Initialize();
                //}
                if (_libVLC == null)
                    throw new Exception("Call InitLibVLC first");
              

                //_libVLC.Log += VlcLogger_Event;

                MediaPlayer = new MediaPlayer(_libVLC);

                MediaPlayer.Opening += MediaPlayerStateChanged;
                MediaPlayer.Playing += MediaPlayerStateChanged;
                MediaPlayer.Paused += MediaPlayerStateChanged;
                MediaPlayer.Stopped += MediaPlayerStateChanged;
                MediaPlayer.PositionChanged += MediaPlayerPositionChanged;
                MediaPlayer.TimeChanged += MediaPlayerTimeChanged;
                MediaPlayer.Muted += MediaPlayerMuted;
                MediaPlayer.Unmuted += MediaPlayerUnmuted;
                MediaPlayer.EncounteredError += MediaPlayerError;
                MediaPlayer.Buffering += MediaPlayerBuffering;
                MediaPlayer.Playing += MediaPlayerPlaying; 
            }
        }

        public void Dispose()
        {
            this.MediaPlayer.Stop();
            this.MediaPlayer = null;
        }

        #region Public Methods
        public void BindToVideoView(VideoView videoView)
        {
            videoView.MediaPlayer = this.MediaPlayer;
            videoView.MediaPlayer.SetHandle(videoView.PlatformHandle);
        }
        public void Play(string mediaUrl = null, double? startTime = null)
        {
            if (_libVLC != null && MediaPlayer != null)
            {
                if (MediaPlayer.Media != null && string.IsNullOrEmpty(mediaUrl))
                    MediaPlayer.Play();
                else
                {
                    List<string> MediaAdditionalOptions = new List<string>();

                    if (startTime.HasValue)
                        MediaAdditionalOptions.Add($"start-time={startTime.Value}");

                    using var media = new Media(
                        _libVLC,
                        new Uri(mediaUrl),
                        MediaAdditionalOptions.ToArray()
                        );

                    MediaPlayer.Play(media);

                    media.Dispose();
                }
            }
        }
        public void Pause()
        {
            if (MediaPlayer != null && MediaPlayer.CanPause)
                MediaPlayer.Pause();
        }

        public void TogglePlay()
        {
            if (MediaPlayer != null && MediaPlayer.State == VLCState.Playing)
                this.Pause();
            else
                this.Play();
        }

        public void Stop()
        {
            if (IsVideoOpened) this.MediaPlayer.Stop();
        }

        public void SetPosition(double position)
        {
            if (IsVideoOpened)
            {
                var msTime = (long)Math.Floor(this.MediaPlayer.Media.Duration * (position / 100));
                var timeSpan = TimeSpan.FromMilliseconds(msTime);
                this.MediaPlayer.SeekTo(timeSpan);
            }
        }

        public void ToggleMute()
        {
            if (IsVideoOpened) this.MediaPlayer.Mute = !MediaPlayer.Mute;
        }
        public void Mute()
        {
            if (IsVideoOpened != null) this.MediaPlayer.Mute = true;
        }

        public void UnMute()
        {
            if (IsVideoOpened != null) this.MediaPlayer.Mute = false;
        }

        public void SetVolume(int volume)
        {
            if (IsVideoOpened) this.MediaPlayer.Volume = volume;
        }

        public void MoveForward()
        {
            if (IsVideoOpened) this.MediaPlayer.Time += 10000;
        }

        public void MoveBackward()
        {
            if (IsVideoOpened) this.MediaPlayer.Time -= 10000;
        }

        public long? GetCurrentTime()
        {
            return IsVideoOpened ? this.MediaPlayer.Time : null;
        }

        public long GetTotalDuration()
        {
            return IsVideoOpened ? this.MediaPlayer.Media.Duration/1000 : 0;
        }

        public bool IsPaused => IsVideoOpened && this.MediaPlayer.State == VLCState.Paused;

        #endregion

        #region MediaPlayer Events
        private void MediaPlayerError(object? sender, EventArgs e)
        {
            if (this.OnError != null)
                this.OnError.Invoke(this, e);
        }

        private void MediaPlayerUnmuted(object? sender, EventArgs e)
        {
            if (this.OnMutedStateChanged != null)
                this.OnMutedStateChanged.Invoke(this, new VideoPlayerEventArgs { Muted = false });
        }

        private void MediaPlayerMuted(object? sender, EventArgs e)
        {
            if (this.OnMutedStateChanged != null)
                this.OnMutedStateChanged.Invoke(this, new VideoPlayerEventArgs { Muted = true });
        }

        private void MediaPlayerTimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
        {
            if (this.OnTimeChanged != null)
                this.OnTimeChanged.Invoke(this, new VideoPlayerEventArgs { CurrentTime = e.Time, RemainingTime = MediaPlayer.Media.Duration - e.Time });
        }

        private void MediaPlayerPositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
        {
            if (this.OnPositionChanged != null)
                this.OnPositionChanged.Invoke(this, new VideoPlayerEventArgs { Progress = e.Position * 100 });
        }

        private void MediaPlayerStateChanged(object? sender, EventArgs e)
        {
            if (MediaPlayer != null && this.OnStateChanged != null)
                this.OnStateChanged.Invoke(this, new VideoPlayerEventArgs { State = MediaPlayer.State, Muted = MediaPlayer.Mute });
        }

        private void MediaPlayerPlaying(object? sender, EventArgs e)
        {
            this.MediaPlayer.SetSpu(-1);
            this.OnPlaying?.Invoke(this, null);
        }

        private void MediaPlayerBuffering(object? sender, MediaPlayerBufferingEventArgs e)
        {
            this.OnBuffering?.Invoke(this, null);
        }

      
        #endregion
    }
}
