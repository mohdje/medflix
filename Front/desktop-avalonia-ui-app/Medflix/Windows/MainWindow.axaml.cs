using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Medflix.Models;
using Medflix.Services;
using Medflix.Tools;
using Medflix.Views;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Medflix.Windows;

public partial class MainWindow : Window
{
    private WindowState previousWindowState;

    private VideoPlayerView videoPlayerView;

    private IHost webhost;
    private MedflixApiService medflixApiService;
    private AppUpdateService appUpdateService;



    public MainWindow()
    {
        InitializeComponent();

        this.MainAppView.OpenVideoPlayerRequest += (s, e) => AddVideoPlayerView(e.VideoPlayerOptions);
        this.MainAppView.MainAppViewLoaded += (s, e) => this.SplashScreen.IsVisible = false;


        medflixApiService = new MedflixApiService();
        appUpdateService = new AppUpdateService();

        VlcPlayer.InitLibVLC();

        webhost = WebHostStreaming.AppStart.CreateHost(new string[0], true);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        webhost?.StopAsync();
    }
    ~MainWindow()
    {
        webhost?.StopAsync();
    }

    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        await Task.Run(() => webhost.Start());

        await Task.Delay(3000);

        this.MainAppView.LoadView(this);

        await CheckUpdateAsync();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        videoPlayerView?.NotifyKeyPressed(e);
    }

    private void AddVideoPlayerView(VideoPlayerOptions videoPlayerOptions)
    {
        this.videoPlayerView = new VideoPlayerView(videoPlayerOptions, medflixApiService);

        this.videoPlayerView.OnEnterFullScreenRequest += OnEnterFullScreenRequest;
        this.videoPlayerView.OnExitFullScreenRequest += OnExitFullScreenRequest;
        this.videoPlayerView.OnToggleFullScreenRequest += OnToggleFullScreenRequest;
        this.videoPlayerView.OnCloseRequest += OnVideoPlayerViewCloseRequest;

        this.MainWindowContainer.Children.Add(this.videoPlayerView);
    }

    private async Task CheckUpdateAsync()
    {
        var updateAvailabe = await this.appUpdateService.IsNewReleaseAvailableAsync();
        if (updateAvailabe)
        {
            var modalWindow = new UpdateModalWindow();
            modalWindow.OnConfirm += async (s, e) =>
            {
                try
                {
                    modalWindow.NotifyDownloadInProgress();
                    var success = await this.appUpdateService.DownloadNewReleaseAsync();
                    if (success)
                    {
                        modalWindow.NotifyInstallUpdate();
                        await Task.Delay(3000);

                        var updateStarted = this.appUpdateService.StartExtractUpdate();
                        if (updateStarted)
                            this.Close();
                    }
                }
                catch (Exception ex)
                {

                }
                modalWindow.NotifyError();
                await Task.Delay(3000);
                modalWindow.Close();
            };
            modalWindow.OnDecline += (s, e) => modalWindow.Close();
            modalWindow.Show();
        }
    }

    private void OnVideoPlayerViewCloseRequest(object? sender, EventArgs e)
    {
        if (this.WindowState == WindowState.FullScreen)
            this.WindowState = this.previousWindowState;

        this.MainWindowContainer.Children.Remove(videoPlayerView);
        this.videoPlayerView = null;
        this.MainAppView.NotifyVideoPlayerClosed();
    }

    #region Fullscreen Request Handlers
    private async void OnEnterFullScreenRequest(object? sender, System.EventArgs e)
    {
        if (this.WindowState != WindowState.FullScreen)
        {
            this.previousWindowState = this.WindowState;
            this.WindowState = WindowState.FullScreen;
            await this.videoPlayerView.NotifyWindowStateChanged(true);
        }
    }

    private async void OnExitFullScreenRequest(object? sender, System.EventArgs e)
    {
        if (this.WindowState == WindowState.FullScreen)
        {
            this.WindowState = this.previousWindowState;
            await this.videoPlayerView.NotifyWindowStateChanged(false);
        }
    }

    private async void OnToggleFullScreenRequest(object? sender, System.EventArgs e)
    {
        if (this.WindowState == WindowState.FullScreen)
            this.WindowState = this.previousWindowState;
        else
        {
            this.previousWindowState = this.WindowState;
            this.WindowState = WindowState.FullScreen;
        }

        await this.videoPlayerView.NotifyWindowStateChanged(this.WindowState == WindowState.FullScreen);
    }

    #endregion
}
