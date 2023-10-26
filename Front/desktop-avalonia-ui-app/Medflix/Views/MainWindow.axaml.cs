using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Medflix.Models;
using Medflix.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Medflix.Views;

public partial class MainWindow : Window
{
    private WindowState previousWindowState;
    private VideoPlayerView videoPlayerView;
    private IHost webhost;
    private MedflixApiService medflixApiService;
    public MainWindow()
    {
        InitializeComponent();

        this.WebView.OpenVideoPlayerRequest += (s, e) => AddVideoPlayerView(e.VideoPlayerOptions);

        medflixApiService = new MedflixApiService();
        webhost = WebHostStreaming.AppStart.CreateHost(new string[0], true);

        //TestVideoView();
    }

    private void TestVideoView()
    {
        //"http://localhost:5000/torrent/stream/movies?base64Url=aaaa"
        var options = new VideoPlayerOptions();
        options.Sources = new VideoOption[]
        {
            new VideoOption{ Quality = "1080p", Url = @"D:\Projets\Streaming\R&D\data\myvideo.mkv", Selected = true },
            new VideoOption{ Quality = "720p", Url = @"https://download.samplelib.com/mp4/sample-15s.mp4" },
            new VideoOption{ Quality = "1080p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" },
            new VideoOption{ Quality = "720p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" },
            new VideoOption{ Quality = "1080p", Url = @"http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4" }
        };
        options.ResumeToTime = 0;
        options.Subtitles = new SubtitleOption[]
        {
            new SubtitleOption(){ Language = "English", SubtitlesSourceUrls = new string[] { "0", "0", "0", "0", "0", "0" } },
            new SubtitleOption(){ Language = "French", SubtitlesSourceUrls = new string[] { "2", "3" } },
        };
        options.WatchedMedia = new WebHostStreaming.Models.WatchedMediaDto();
        options.WatchedMedia.SeasonNumber = 1;
        options.WatchedMedia.EpisodeNumber = 1;
        options.WatchedMedia.CoverImageUrl = "cover";
        options.WatchedMedia.Id = "252525";
        options.WatchedMedia.Rating = 7.5f;
        options.WatchedMedia.Synopsis = "this is a movie...";
        options.WatchedMedia.Title = "Breaking Bad";
        options.WatchedMedia.TotalDuration = 3600;
        options.WatchedMedia.Year = 2022;
        options.MediaType = "series";

        AddVideoPlayerView(options);
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        webhost.Start();

        Task.Delay(2000).ContinueWith(t =>
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                this.WebView.IsVisible = true;
                this.SplashScreen.IsVisible = false;
            });
        });
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        videoPlayerView?.OnKeyPressed(e);
    }

    private void AddVideoPlayerView(VideoPlayerOptions videoPlayerOptions)
    {
        this.videoPlayerView = new VideoPlayerView(videoPlayerOptions, medflixApiService);

        this.videoPlayerView.OnEnterFullScreenRequest += OnEnterFullScreenRequest;
        this.videoPlayerView.OnExitFullScreenRequest += OnExitFullScreenRequest;
        this.videoPlayerView.OnToggleFullScreenRequest += OnToggleFullScreenRequest;
        this.videoPlayerView.OnCloseRequest += OnVideoPlayerViewCloseRequest;

        this.MainVindowContainer.Children.Add(this.videoPlayerView);
    }

    private void OnVideoPlayerViewCloseRequest(object? sender, EventArgs e)
    {
        if(this.WindowState == WindowState.FullScreen)
            this.WindowState = this.previousWindowState;

        this.MainVindowContainer.Children.Remove(videoPlayerView);
        this.videoPlayerView = null;
    }

    #region Fullscreen Request Handlers
    private void OnEnterFullScreenRequest(object? sender, System.EventArgs e)
    {
        if (this.WindowState != WindowState.FullScreen)
        {
            this.previousWindowState = this.WindowState;
            this.WindowState = WindowState.FullScreen;
            this.videoPlayerView.OnFullScreenStateChanged(true);
        }
    }

    private void OnExitFullScreenRequest(object? sender, System.EventArgs e)
    {
        if (this.WindowState == WindowState.FullScreen)
        {
            this.WindowState = this.previousWindowState;
            this.videoPlayerView.OnFullScreenStateChanged(false);
        }
    }

    private void OnToggleFullScreenRequest(object? sender, System.EventArgs e)
    {
        if (this.WindowState == WindowState.FullScreen)
            this.WindowState = this.previousWindowState;
        else
        {
            this.previousWindowState = this.WindowState;
            this.WindowState = WindowState.FullScreen;
        }

        this.videoPlayerView.OnFullScreenStateChanged(this.WindowState == WindowState.FullScreen);
    }
    #endregion
}
