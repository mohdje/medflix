﻿using Shared = LibVLCSharp.Shared;
using System.ComponentModel;
using Medflix.Models;
using LibVLCSharp.Shared;

namespace Medflix.ViewModels
{
    public class MediaPlayerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Shared.LibVLC LibVLC { get; set; }
        private bool IsLoaded { get; set; }
        private bool IsVideoViewInitialized { get; set; }

        private long ResumeTime { get; set; }
        private bool PlaybackReady => MediaPlayer.IsPlaying || MediaPlayer.WillPlay;

        private void Set<T>(string propertyName, ref T field, T value)
        {
            if (field == null && value != null || field != null && !field.Equals(value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private Shared.MediaPlayer _mediaPlayer;
        public Shared.MediaPlayer MediaPlayer
        {
            get => _mediaPlayer;
            private set => Set(nameof(MediaPlayer), ref _mediaPlayer, value);
        }

        public string MediaUrl { get; private set; }


        public MediaPlayerViewModel()
        {
          if(DeviceInfo.Current.Platform != DevicePlatform.WinUI) 
               Initialize();
        }

        public void Initialize(string[] swapchainOptions = null)
        {
            try
            {
                LibVLC = swapchainOptions != null ? new LibVLC(enableDebugLogs: true, swapchainOptions) : new LibVLC(enableDebugLogs: true);
                LibVLC.SetUserAgent(AppConfig.Instance.AppIdentifier, AppConfig.Instance.AppIdentifier);
            }
            catch (Exception ex)
            {

            }
        }

        public void OnAppearing()
        {
            IsLoaded = true;
            Play();
        }

        internal void OnDisappearing()
        {
            if (MediaPlayer != null)
            {
                MediaPlayer.Stop();

                MediaPlayer.Dispose();
            }
              
            LibVLC.Dispose();
        }

        public void OnVideoViewInitialized()
        {
            IsVideoViewInitialized = true;
            Play();
        }

        public void PlayMedia(string url, long? timeInMs = null)
        {
            if (MediaPlayer != null)
                MediaPlayer.Stop();

            MediaUrl = url;
            ResumeTime = timeInMs.GetValueOrDefault(-1);

            using var media = new Shared.Media(LibVLC, new Uri(url));

            MediaPlayer = new Shared.MediaPlayer(LibVLC)
            {
                Media = media
            };

            MediaPlayer.Playing += (s, e) =>
            {
                MediaPlayer.SetSpu(-1);//disable subtitles included in the video

                if (ResumeTime > 0)
                {
                    MediaPlayer.Position = ResumeTime / (float)MediaPlayer.Media.Duration;
                    ResumeTime = -1;
                }
            };
        }

        private void Play()
        {
            if (IsLoaded && IsVideoViewInitialized)
            {
                MediaPlayer.Play();
            }
        }

        public void TogglePlay()
        {
            if (PlaybackReady)
            {
                if (MediaPlayer.IsPlaying)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Play();
            }
        }

        public void Seek(double position)
        {
            if (PlaybackReady)
            {
                MediaPlayer.Position = (float)position;
                MediaPlayer.SetPause(false);
            }
        }
    }
}
