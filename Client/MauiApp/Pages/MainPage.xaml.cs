using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Services;
using System.Text.Json;

namespace Medflix.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            MainPageWebView.OnVideoPlayerPageRequested += (s, e) => ToVideoPlayerPage();
        }

        private void ToVideoPlayerPage()
        {
        //    var videoOptions = new VideoPlayerOptions();
        //    videoOptions.SubtitlesSources = new SubtitlesSources[]
        //    {
        //        new SubtitlesSources{ Language ="French", Urls= new string [] {"french1", "french2"}},
        //        new SubtitlesSources{ Language ="English", Urls= new string [] {"English1", "English2", "English3"}}
        //    };
        //    videoOptions.VideosInfos = new VideoInfo[]
        //    {
        //        new VideoInfo { Quality ="1080p", Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"},
        //        new VideoInfo { Quality ="1080p", Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"},
        //        new VideoInfo { Quality ="720p.Webrip", Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"},
        //        new VideoInfo { Quality ="1080p.Webrip", Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"},
        //        new VideoInfo { Quality ="720p.Webrip", Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", Selected = true},
        //        new VideoInfo { Quality ="480p", Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"},
        //        new VideoInfo { Quality ="1080p", Url = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"},
        //    };
        //    videoOptions.ResumeToTime = 0;

        //    var mediaDto = new PlayerMedia
        //    {
        //        EpisodeNumber = 1,
        //        SeasonNumber = 2,
        //        Genres = new Genre[]
        //        {
        //            new Genre()
        //        },
        //        Id = "122",
        //        Rating = 5,
        //        Synopsis = "An awesome movie",
        //        Title = "The expendataor",
        //        TotalDuration = 1234,
        //        Year = 2022,
        //        Type = "movies"
        //    };
        //    videoOptions.Media = mediaDto;

            //MainThread.BeginInvokeOnMainThread(async () =>
            //{
            //    await Navigation.PushAsync(new VideoPlayerPage(videoOptions));
            //});
        }

    }
}