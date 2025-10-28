
using Medflix.Extensions;
using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Services;
using Medflix.Views.AndroidTv;

namespace Medflix.Pages.AndroidTv
{
    public partial class AndroidTvMediaPresentationPage : ContentPage
    {
        string mediaId;
        MediaDetails mediaDetails;
        WatchMediaInfo watchedMediaInfo;
        int? selectedSeasonNumber;
        int? selectedEpisodeNumber;
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
                var result = MediaIsSerie ? await MedflixApiService.Instance.GetEpisodeWatchMediaInfo(mediaId, selectedSeasonNumber!.Value, selectedEpisodeNumber!.Value)
                                          : await MedflixApiService.Instance.GetWatchMediaInfo(mediaId);
                if (result != null)
                    UpdateWatchingProgress(result);

                return;
            }

            mediaDetails = await MedflixApiService.Instance.GetMediaDetailsAsync(mediaId);
            if (mediaDetails == null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ShowMessage("Error loading media information");
                    LoadingView.IsVisible = false;
                });
                return;
            }

            UpdateMediaInfo();

            await Task.WhenAll(
              MedflixApiService.Instance.GetWatchMediaInfo(mediaId).ContinueWith(async t =>
              {
                  UpdateWatchingProgress(t.Result);

                  if (MediaIsSerie)
                  {
                      selectedSeasonNumber = t.Result?.SeasonNumber ?? 1;
                      selectedEpisodeNumber = t.Result?.EpisodeNumber ?? 1;
                  }

                  UpdateSelectEpisodeButton();
                  UpdateNextEpisodeButtonVisibility();

                  await GetMediaResourcesAsync();
              }),
              MedflixApiService.Instance.IsMediaBookmarked(mediaId).ContinueWith(t => UpdateBookmarkButtons(t.Result)),
              GetRecommandationsAsync()
            );

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingView.IsVisible = false;
                PageContent.IsVisible = true;
            });
        }

        private async Task OnEpisodeSelected(int seasonNumber, int episodeNumber, WatchMediaInfo watchMediaInfo, bool? isLastEpisodeOfSeason = null)
        {
            this.selectedSeasonNumber = seasonNumber;
            this.selectedEpisodeNumber = episodeNumber;

            UpdateSelectEpisodeButton();
            UpdateNextEpisodeButtonVisibility(isLastEpisodeOfSeason);
            UpdateWatchingProgress(watchMediaInfo);
            await GetMediaResourcesAsync();
        }

        private async Task PlayMedia(bool forceRestart = false)
        {
            var subtitlesSources = new List<SubtitlesSources>();
            if (frenchSubtitlesUrls != null && frenchSubtitlesUrls.Any())
                subtitlesSources.Add(new SubtitlesSources { Language = "French", Urls = frenchSubtitlesUrls.ToArray() });

            if (englishSubtitlesUrls != null && englishSubtitlesUrls.Any())
                subtitlesSources.Add(new SubtitlesSources { Language = "English", Urls = englishSubtitlesUrls.ToArray() });

            var mediaSources = new List<MediaSource>();
            if (voSources != null && voSources.Any())
                mediaSources.AddRange(voSources);

            if (vfSources != null && vfSources.Any())
                mediaSources.AddRange(vfSources);

            var videoPlayerParameters = new VideoPlayerParameters
            {
                SubtitlesSources = [.. subtitlesSources],
                MediaSources = [.. mediaSources],
                WatchMedia = new WatchMediaInfo
                {
                    Media = mediaDetails,
                    CurrentTime = forceRestart ? 0 : watchedMediaInfo?.CurrentTime ?? 0,
                    EpisodeNumber = selectedEpisodeNumber.GetValueOrDefault(0),
                    SeasonNumber = selectedSeasonNumber.GetValueOrDefault(0),
                    VideoSource = watchedMediaInfo?.VideoSource
                }
            };

            var videoPlayerPage = new VideoPlayerPage(videoPlayerParameters);
            videoPlayerPage.CloseVideoPlayerRequested += async (s, e) =>
            {
                await Navigation.PopAsync();
            };

            await Navigation.PushAsync(videoPlayerPage);
        }

        private static void ShowMessage(string message)
        {
#if ANDROID
            Android.Widget.Toast.MakeText(Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, message, Android.Widget.ToastLength.Long).Show();
#endif
        }

        #region UI Updates

        private void UpdateMediaInfo()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
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
                Duration.Text = MediaIsMovie ? TimeSpan.FromMinutes(mediaDetails.Duration).ToTimeFormat() : string.Empty;
                Rating.Text = mediaDetails.Rating.ToString("0.0").Replace(",", ".");
                Genres.Text = mediaDetails.Genres != null ? String.Join(" - ", mediaDetails.Genres.Select(genre => genre.Name)) : string.Empty;

                Director.Text = mediaDetails.Director;
                Cast.Text = mediaDetails.Cast;
                Synopsis.Text = mediaDetails.Synopsis;

                TrailerButton.IsVisible = !string.IsNullOrEmpty(mediaDetails.YoutubeTrailerUrl);
            });
        }

        private void UpdateWatchingProgress(WatchMediaInfo info)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                watchedMediaInfo = info;
                if (watchedMediaInfo != null)
                {
                    WatchingProgressSection.IsVisible = true;
                    PlayFromBeginningButton.IsVisible = true;

                    var timeSpan = TimeSpan.FromSeconds(watchedMediaInfo.TotalDuration).Subtract(TimeSpan.FromSeconds(watchedMediaInfo.CurrentTime));
                    RemainingTime.Text = $"{timeSpan.ToTimeFormat()} remaining";

                    var progress = (double)watchedMediaInfo.CurrentTime / watchedMediaInfo.TotalDuration;
                    await WatchingProgress.ProgressTo(progress, 0, Easing.Default);
                }
                else
                {
                    WatchingProgressSection.IsVisible = false;
                    PlayFromBeginningButton.IsVisible = false;
                }
            });
        }

        private void UpdateSelectEpisodeButton()
        {
            MainThread.BeginInvokeOnMainThread(() =>
           {
               EpisodeSelectionButton.IsVisible = MediaIsSerie;
               EpisodeSelectionButton.Text = $"Season {selectedSeasonNumber.GetValueOrDefault(1)}   Episode {selectedEpisodeNumber.GetValueOrDefault(1)}";
           });
        }

        private void UpdateBookmarkButtons(bool isBookmarked)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                AddBookmarkButton.IsVisible = !isBookmarked;
                RemoveBookmarkButton.IsVisible = isBookmarked;
            });
        }

        private void UpdateNextEpisodeButtonVisibility(bool? isLastEpisodeOfSeason = null)
        {
            if (!MediaIsSerie)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NextEpisodeButton.IsVisible = false;
                });
                return;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                isLastEpisodeOfSeason ??= await MedflixApiService.Instance.IsLastEpisodeOfSeason(mediaId, selectedSeasonNumber!.Value, selectedEpisodeNumber!.Value);
                var isLastSeason = selectedSeasonNumber!.Value >= mediaDetails.SeasonsCount;

                if (isLastEpisodeOfSeason!.Value && isLastSeason)
                {
                    NextEpisodeButton.IsVisible = false;
                }
                else if (isLastEpisodeOfSeason!.Value)
                {
                    NextEpisodeButton.IsVisible = true;
                    NextEpisodeButton.Text = $"Next Season";
                }
                else
                {
                    NextEpisodeButton.IsVisible = true;
                    NextEpisodeButton.Text = $"Next Episode";
                }
            });
        }

        #endregion

        #region Fetching Resources

        private async Task GetMediaResourcesAsync()
        {
            await Task.WhenAll(GetAvailableSubtitlesAsync(), GetAvailableVideoSourcesAsync());
        }
        private async Task GetAvailableSubtitlesAsync()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Subtitles.ShowLoading = true;
            });

            await Task.WhenAll(
                MedflixApiService.Instance.GetAvailableEnglishSubtitlesAsync(mediaDetails.ImdbId, selectedSeasonNumber, selectedEpisodeNumber).ContinueWith(t => englishSubtitlesUrls = t.Result),
                MedflixApiService.Instance.GetAvailableFrenchSubtitlesAsync(mediaDetails.ImdbId, selectedSeasonNumber, selectedEpisodeNumber).ContinueWith(t => frenchSubtitlesUrls = t.Result)
                );

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

            var tasks = new List<Task<IEnumerable<MediaSource>>>()
            {
                MedflixApiService.Instance.GetAvailableVOSources(
                     title: mediaDetails.Title,
                     year: MediaIsMovie ? mediaDetails.Year : null,
                     mediaId: mediaDetails.Id,
                     imdbId: mediaDetails.ImdbId,
                     seasonNumber: selectedSeasonNumber,
                     episodeNumber: selectedEpisodeNumber)
                    .ContinueWith(t => voSources = t.Result),
                 MedflixApiService.Instance.GetAvailableVFSources(
                     title: mediaDetails.Title,
                     mediaId: mediaDetails.Id,
                     year: MediaIsMovie ? mediaDetails.Year : null,
                     seasonNumber: selectedSeasonNumber,
                     episodeNumber: selectedEpisodeNumber)
                    .ContinueWith(t => vfSources = t.Result)
            };

            await Task.WhenAll(tasks);

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
        private async Task GetRecommandationsAsync()
        {
            var recommandations = await MedflixApiService.Instance.GetSimilarMediasAsync(mediaId);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (recommandations != null && recommandations.Any())
                    Recommandations.Children.Add(new MediaLitePresentationListView("You may also like", recommandations));
                else
                    Recommandations.IsVisible = false;
            });
        }

        #endregion

        #region Button Click Events
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
                var page = new SeasonEpisodeSelectionModalPage(mediaId, mediaDetails.SeasonsCount, selectedSeasonNumber.GetValueOrDefault(1));
                page.OnEpisodeSelected += async (s, e) =>
                {
                    await Navigation.PopModalAsync();
                    await OnEpisodeSelected(e.SeasonNumber, e.EpisodeNumber, e.WatchMedia, e.IsLastEpisodeOfSeason);
                };

                await Navigation.PushModalAsync(page);
            });
        }

        private async void OnNextEpisodeButtonClicked(object sender, EventArgs e)
        {
            int episodeNumber;
            int seasonNumber;

            if (NextEpisodeButton.Text == "Next Season")
            {
                seasonNumber = selectedSeasonNumber!.Value + 1;
                episodeNumber = 1;
            }
            else
            {
                seasonNumber = selectedSeasonNumber!.Value;
                episodeNumber = selectedEpisodeNumber!.Value + 1;
            }

            var watchMediaInfo = await MedflixApiService.Instance.GetEpisodeWatchMediaInfo(mediaId, seasonNumber, episodeNumber);
            await OnEpisodeSelected(seasonNumber, episodeNumber, watchMediaInfo);
        }

        private async void OnPlayButtonClicked(object sender, EventArgs e)
        {
            await PlayMedia();
        }

        private async void OnPlayFromBeginningButtonClicked(object sender, EventArgs e)
        {
            await PlayMedia(true);
        }

        private async void OnAddBookmarkButtonClicked(object sender, EventArgs e)
        {
            var isSuccess = await MedflixApiService.Instance.BookmarkMedia(mediaDetails);

            UpdateBookmarkButtons(isSuccess);

            ShowMessage(isSuccess ? "Added to your list with success" : "Error trying to add to your list");
        }

        private async void OnRemoveBookmarkButtonClicked(object sender, EventArgs e)
        {
            var isSuccess = await MedflixApiService.Instance.RemoveBookmarkMedia(mediaDetails.Id);

            UpdateBookmarkButtons(!isSuccess);

            ShowMessage(isSuccess ? "Removed from your list with success" : "Error trying to remove from your list");
        }

        #endregion
    }
}