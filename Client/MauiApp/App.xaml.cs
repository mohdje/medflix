﻿using Medflix.Models;
using Medflix.Models.VideoPlayer;
using Medflix.Pages;
using Medflix.Pages.AndroidTv;
using Medflix.Services;
using Medflix.Utils;

namespace Medflix
{
    public partial class App : Application
    {
        public App()
        {
            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("MedflixCustomization", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
                handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
                handler.PlatformView.Settings.MediaPlaybackRequiresUserGesture = false;
                handler.PlatformView.Settings.UserAgentString = "Mozilla/5.0";
                handler.PlatformView.Settings.ForceDark = Android.Webkit.ForceDarkMode.Off;
#endif
            });

            this.UserAppTheme = AppTheme.Light;

            InitializeComponent();

            var startingPage = new StartingPage();
            startingPage.OnReady += (s, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (DeviceInfo.Current.Idiom == DeviceIdiom.TV)
                        MainPage = new NavigationPage(new AndroidTvMainPage());
                    else
                        MainPage = new NavigationPage(new MainPage());
                });
            };

            MainPage = startingPage;
        }
    }
}