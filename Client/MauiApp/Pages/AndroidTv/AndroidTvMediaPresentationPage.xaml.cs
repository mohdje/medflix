
using Medflix.Extensions;
using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Services;
using Medflix.Views.AndroidTv;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Medflix.Pages.AndroidTv
{
    public partial class AndroidTvMediaPresentationPage : ContentPage
    {
        string mediaId;
        MediaDetails mediaDetails;
        WatchMediaInfo videoPlayerMediaWatched;
        int? seasonNumber;
        int? episodeNumber;
        bool MediaIsSerie => mediaDetails.SeasonsCount > 0;
        bool MediaIsMovie => mediaDetails.SeasonsCount == 0;

        IEnumerable<string> frenchSubtitlesUrls;
        IEnumerable<string> englishSubtitlesUrls;
        IEnumerable<MediaSource> vfSources;
        IEnumerable<MediaSource> voSources;

        public AndroidTvMediaPresentationPage(string mediaId)
        {
            InitializeComponent();
            this.mediaId = mediaId;
            this.Loaded += async (s, e) => await LoadContent();
        }

        private async Task LoadContent()
        {
            if (PageContent.IsVisible) //When closing video player, PageContent is already set. Just update watchingProgress
            {
                videoPlayerMediaWatched = await MedflixApiService.Instance.GetWatchMediaInfo(mediaId);
                UpdateWatchingProgress();
                return;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                mediaDetails = await MedflixApiService.Instance.GetMediaDetailsAsync(mediaId);

                if (mediaDetails == null)
                {
                    LoadingView.IsVisible = false;
                    return;
                }

                await LoadMediaRessources();

                await GetRecommandationsAsync();

                var isBookmarked = await MedflixApiService.Instance.IsMediaBookmarked(mediaId);
                AddBookmarkButton.IsVisible = !isBookmarked;
                RemoveBookmarkButton.IsVisible = isBookmarked;

                if (!string.IsNullOrEmpty(mediaDetails.LogoImageUrl))
                    LogoTitle.Source = ImageSource.FromUri(new Uri(mediaDetails.LogoImageUrl));
                else
                {
                    LogoTitle.IsVisible = false;
                    TextTitle.IsVisible = true;
                    TextTitle.Text = mediaDetails.Title;
                }

                if (!string.IsNullOrEmpty(mediaDetails.BackgroundImageUrl))
                    BackgroundImage.Source = ImageSource.FromUri(new Uri(mediaDetails.BackgroundImageUrl));

                Year.Text = mediaDetails.Year.ToString();

                Duration.IsVisible = MediaIsMovie;
                Duration.Text = TimeSpan.FromMinutes(mediaDetails.Duration).ToTimeFormat();

                Rating.Text = mediaDetails.Rating.ToString("0.0").Replace(",", ".");

                Genres.IsVisible = mediaDetails.Genres != null;
                Genres.Text = mediaDetails.Genres != null ? String.Join(" - ", mediaDetails.Genres.Select(genre => genre.Name)) : string.Empty;

                EpisodeSelectionButton.IsVisible = MediaIsSerie;

                Director.IsVisible = !string.IsNullOrEmpty(mediaDetails.Director);
                Director.Text = mediaDetails.Director;

                Cast.IsVisible = !string.IsNullOrEmpty(mediaDetails.Cast);
                Cast.Text = mediaDetails.Cast;

                Synopsis.Text = mediaDetails.Synopsis;

                TrailerButton.IsVisible = !string.IsNullOrEmpty(mediaDetails.YoutubeTrailerUrl);

                LoadingView.IsVisible = false;
                PageContent.IsVisible = true;
            });
        }
        private async Task LoadMediaRessources()
        {
            videoPlayerMediaWatched = await MedflixApiService.Instance.GetWatchMediaInfo(mediaId);

            if (MediaIsSerie)
            {
                seasonNumber = videoPlayerMediaWatched?.SeasonNumber ?? 1;
                episodeNumber = videoPlayerMediaWatched?.EpisodeNumber ?? 1;
            }
            UpdateWatchingProgress();

            await GetAvailableSubtitlesAsync();
            await GetAvailableVideoSourcesAsync();
        }
        private async Task GetRecommandationsAsync()
        {
            var recommandations = await MedflixApiService.Instance.GetSimilarMediasAsync(mediaId);

            if (recommandations != null && recommandations.Any())
                Recommandations.Children.Add(new MediaLitePresentationListView("You may also like", recommandations));
            else
                Recommandations.IsVisible = false;
        }
        private async Task GetAvailableSubtitlesAsync()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Subtitles.ShowLoading = true;
            });

            englishSubtitlesUrls = await MedflixApiService.Instance.GetAvailableEnglishSubtitlesAsync(mediaDetails.ImdbId, seasonNumber, episodeNumber);
            frenchSubtitlesUrls = await MedflixApiService.Instance.GetAvailableFrenchSubtitlesAsync(mediaDetails.ImdbId, seasonNumber, episodeNumber);

            var availableSubtitles = new List<string>();
            if (englishSubtitlesUrls != null && englishSubtitlesUrls.Any())
                availableSubtitles.Add("English");
            if (frenchSubtitlesUrls != null && frenchSubtitlesUrls.Any())
                availableSubtitles.Add("French");


            MainThread.BeginInvokeOnMainThread(() =>
            {
                Subtitles.Text = availableSubtitles.Any() ? String.Join(", ", availableSubtitles) : "No subtitles available";
                Subtitles.ShowLoading = false;
            });
        }

        private async Task GetAvailableVideoSourcesAsync()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayButton.IsEnabled = false;
                PlayFromBeginningButton.IsEnabled = false;
                Versions.ShowLoading = true;
            });

            voSources = await MedflixApiService.Instance.GetAvailableVOSources(
             title: mediaDetails.Title,
             year: MediaIsMovie ? mediaDetails.Year : null,
             imdbId: MediaIsSerie ? mediaDetails.ImdbId : null,
             seasonNumber: seasonNumber,
             episodeNumber: episodeNumber);

            vfSources = await MedflixApiService.Instance.GetAvailableVFSources(
                 title: mediaDetails.Title,
                 mediaId: mediaDetails.Id,
                 year: MediaIsMovie ? mediaDetails.Year : null,
                 seasonNumber: seasonNumber,
                 episodeNumber: episodeNumber
                );

            var availableTorrents = new List<string>();
            if (voSources != null && voSources.Any())
                availableTorrents.Add("Original");
            if (vfSources != null && vfSources.Any())
                availableTorrents.Add("French");

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Versions.Text = availableTorrents.Any() ? String.Join(", ", availableTorrents) : "No version available";
                PlayButton.IsVisible = availableTorrents.Any();
                PlayButton.IsEnabled = true;
                PlayFromBeginningButton.IsEnabled = true;
                Versions.ShowLoading = false;
            });
        }

        private void UpdateWatchingProgress()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (videoPlayerMediaWatched != null)
                {
                    WatchingProgressSection.IsVisible = true;
                    PlayFromBeginningButton.IsVisible = true;

                    var timeSpan = TimeSpan.FromSeconds(videoPlayerMediaWatched.TotalDuration).Subtract(TimeSpan.FromSeconds(videoPlayerMediaWatched.CurrentTime));
                    RemainingTime.Text = $"{timeSpan.ToTimeFormat()} remaining";

                    var progress = (double)videoPlayerMediaWatched.CurrentTime / videoPlayerMediaWatched.TotalDuration;
                    await WatchingProgress.ProgressTo(progress, 0, Easing.Default);
                }
                else
                {
                    WatchingProgressSection.IsVisible = false;
                    PlayFromBeginningButton.IsVisible = false;
                }

                EpisodeSelectionButton.Text = $"Season {seasonNumber.GetValueOrDefault(1)}   Episode {episodeNumber.GetValueOrDefault(1)}";
            });
        }

        private void OnTrailerButtonClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushAsync(new MediaTrailerPage(mediaDetails.YoutubeTrailerUrl));
            });
        }

        private void OnEpisodeSelectionButtonClicked(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var page = new SeasonEpisodeSelectionModalPage(mediaId, mediaDetails.SeasonsCount, seasonNumber.GetValueOrDefault(1));
                page.OnEpisodeSelected += async (s, e) =>
                {
                    await Navigation.PopModalAsync();
                    await OnEpisodeSelected(e.SeasonNumber, e.EpisodeNumber, e.WatchMedia);
                };

                await Navigation.PushModalAsync(page);
            });
        }

        private async Task OnEpisodeSelected(int seasonNumber, int episodeNumber, WatchMediaInfo videoPlayerMedia)
        {
            this.seasonNumber = seasonNumber;
            this.episodeNumber = episodeNumber;
            this.videoPlayerMediaWatched = videoPlayerMedia;

            await Task.Run(async () =>
            {
                UpdateWatchingProgress();
                await GetAvailableSubtitlesAsync();
                await GetAvailableVideoSourcesAsync();
            });
        }

        private async void OnPlayButtonClicked(object sender, EventArgs e)
        {
            await PlayMedia();
        }

        private async void OnPlayFromBeginningButton(object sender, EventArgs e)
        {
            await PlayMedia(true);
        }

        private async Task PlayMedia(bool forceRestart = false)
        {
            var videoPlayerOptions = new VideoPlayerParameters();

            var subtitlesSources = new List<SubtitlesSources>();
            if (frenchSubtitlesUrls != null && frenchSubtitlesUrls.Any())
                subtitlesSources.Add(new SubtitlesSources { Language = "French", Urls = frenchSubtitlesUrls.ToArray() });

            if (englishSubtitlesUrls != null && englishSubtitlesUrls.Any())
                subtitlesSources.Add(new SubtitlesSources { Language = "English", Urls = englishSubtitlesUrls.ToArray() });

            videoPlayerOptions.SubtitlesSources = subtitlesSources.ToArray();

            var mediaSources = new List<MediaSources>();
            if (voSources != null && voSources.Any())
                mediaSources.Add(new MediaSources { Language = "Original", Sources = voSources.ToArray() });

            if (vfSources != null && vfSources.Any())
                mediaSources.Add(new MediaSources { Language = "French", Sources = vfSources.ToArray() });

            videoPlayerOptions.MediaSources = mediaSources.ToArray();

            var videoSource = string.Empty;
            if (videoPlayerMediaWatched != null)
            {
                var files = mediaSources.SelectMany(s => s.Sources.Select(t => t.FilePath));
                var torrents = mediaSources.SelectMany(s => s.Sources.Select(t => t.TorrentUrl));

                if (files.Any(url => url == videoPlayerMediaWatched.VideoSource))
                    videoSource = videoPlayerMediaWatched.VideoSource;

                else if (torrents.Any(url => url == videoPlayerMediaWatched.VideoSource))
                    videoSource = videoPlayerMediaWatched.VideoSource;
            }

            videoPlayerOptions.WatchMedia = new WatchMediaInfo
            {
                Media = mediaDetails,
                CurrentTime = forceRestart ? 0 : videoPlayerMediaWatched?.CurrentTime ?? 0,
                EpisodeNumber = episodeNumber.GetValueOrDefault(0),
                SeasonNumber = seasonNumber.GetValueOrDefault(0),
                VideoSource = videoSource
            };

            await Navigation.PushAsync(new VideoPlayerPage(videoPlayerOptions));
        }

        private async void OnAddBookmarkButtonClicked(object sender, EventArgs e)
        {
            var isSuccess = await MedflixApiService.Instance.BookmarkMedia(mediaDetails);

            AddBookmarkButton.IsVisible = !isSuccess;
            RemoveBookmarkButton.IsVisible = isSuccess;

            var message = isSuccess ? "Added to your list with success" : "Error trying to add to your list";

#if ANDROID
            Android.Widget.Toast.MakeText(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, message, Android.Widget.ToastLength.Long).Show();
#endif
        }

        private async void OnRemoveBookmarkButtonClicked(object sender, EventArgs e)
        {
            var isSuccess = await MedflixApiService.Instance.RemoveBookmarkMedia(mediaDetails.Id);

            AddBookmarkButton.IsVisible = isSuccess;
            RemoveBookmarkButton.IsVisible = !isSuccess;

            var message = isSuccess ? "Removed from your list with success" : "Error trying to remove from your list";

#if ANDROID
            Android.Widget.Toast.MakeText(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, message, Android.Widget.ToastLength.Long).Show();
#endif
        }


    }
}