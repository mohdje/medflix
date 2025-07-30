using Medflix.Controls.VideoPlayer;
using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using Medflix.Services;

namespace Medflix.Views
{

    public partial class VideoPlayerMenuView : ContentView
    {
        public event EventHandler OnNoSubtitlesClick;
        public event EventHandler<string> OnDisplaySubtitlesClick;
        public event EventHandler<MediaSource> OnVideoQualityClick;

        string selectedSubtitlesUrl;
        MediaSource selectedMediaSource;

        public VideoPlayerMenuView()
        {
            InitializeComponent();

            IsVisible = false;

            this.CloseButton.IsVisible = DeviceInfo.Current.Idiom == DeviceIdiom.Desktop || DeviceInfo.Current.Idiom == DeviceIdiom.Phone;
        }

        public void ShowSubtitlesMenu(SubtitlesSources[] subtitlesSources)
        {
            var menuOptions = new Dictionary<string, VideoPlayerMenuButton[]>();

            var noSubtitlesButton = new VideoPlayerMenuButton("No subtitles")
            {
                Selected = string.IsNullOrEmpty(selectedSubtitlesUrl)
            };
            noSubtitlesButton.Clicked += (s, e) =>
            {
                selectedSubtitlesUrl = string.Empty;
                OnNoSubtitlesClick?.Invoke(this, EventArgs.Empty);
            };
            menuOptions.Add("Default", [noSubtitlesButton]);

            foreach (var sub in subtitlesSources)
            {
                var subtitlesList = new List<VideoPlayerMenuButton>();
                for (int i = 0; i < sub.Urls.Length; i++)
                {
                    var url = sub.Urls[i];
                    var videoPlayerMenuButton = new VideoPlayerMenuButton($"{sub.Language} ({i + 1})");

                    if (selectedSubtitlesUrl == url)
                        videoPlayerMenuButton.Selected = true;
                    else
                    {
                        videoPlayerMenuButton.Clicked += (s, e) =>
                        {
                            selectedSubtitlesUrl = url; 
                            OnDisplaySubtitlesClick?.Invoke(this, url);
                        };
                    }

                    subtitlesList.Add(videoPlayerMenuButton);
                }
                menuOptions.Add(sub.Language, subtitlesList.ToArray());
            }

            ShowMenu("Subtitles", menuOptions);
        }

        public void ShowVersionQualitiesMenu(MediaSource[] mediaSources, MediaSource defaultMediaSource)
        {
            var languages = mediaSources.Select(m => m.Language).Distinct();

            var menuOptions = new Dictionary<string, VideoPlayerMenuButton[]>();

            foreach (var language in languages)
            {
                var qualitiesList = new List<VideoPlayerMenuButton>();

                var mediaSourcesForLanguage = mediaSources.Where(m => m.Language == language).OrderBy(m => m.Quality);

                foreach (var mediaSource in mediaSourcesForLanguage)
                {
                    var index = qualitiesList.Count(btn => btn.Text.Contains(mediaSource.Quality));

                    var text = $"{mediaSource.Quality}";
                    if(index > 0)
                        text += $" ({index + 1})";

                    var button = new VideoPlayerMenuButton(text);

                    if ((selectedMediaSource == null && defaultMediaSource.Equals(mediaSource)) || selectedMediaSource == mediaSource)
                        button.Selected = true;
                    else
                    {
                        button.Clicked += (s, e) =>
                        {
                            selectedMediaSource = mediaSource;
                            OnVideoQualityClick?.Invoke(this, mediaSource);
                        };
                    }

                    qualitiesList.Add(button);
                }

                menuOptions.Add(language, qualitiesList.ToArray());
            }

            ShowMenu("Versions", menuOptions);
        }

        private void ShowMenu(string title, Dictionary<string, VideoPlayerMenuButton[]> menuOptions)
        {
            MenuViewContainer.WidthRequest = DeviceInfo.Current.Idiom == DeviceIdiom.Desktop ? 0.5 * Width : Width;
            MenuViewContainer.HeightRequest = DeviceInfo.Current.Idiom == DeviceIdiom.Desktop ? 0.5 * Height : Height;

            if (DeviceInfo.Current.Idiom == DeviceIdiom.TV)
                RemoteCommandActionNotifier.Instance.OnBackButtonPressed += OnCloseClicked;

            this.CloseButton.Clicked += OnCloseClicked;

            MenuTitle.Text = title;

            MenuContainer.Clear();
            MenuContainer.ColumnDefinitions.Clear();

            for (int i = 0; i < menuOptions.Count; i++)
                MenuContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            var columnPosition = -1;
            VideoPlayerMenuButton selectedButton = null;
            ScrollView selectedScrollView = null;

            foreach (var menu in menuOptions)
            {
                var menuListContainer = new Grid();

                menuListContainer.RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = 50 },
                    new RowDefinition { Height = GridLength.Star }
                };
                columnPosition++;
                Grid.SetColumn(menuListContainer, columnPosition);

                var menuLabel = new Label
                {
                    Text = menu.Key,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    Padding = new Thickness(10),
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Brush.White.Color
                };
                Grid.SetRow(menuLabel, 0);

                menuListContainer.Add(menuLabel);

                var menuOptionsList = new VerticalStackLayout();

                foreach (var button in menu.Value)
                {
                    button.Clicked += async (s, e) =>
                    {
                        await Task.Delay(300);
                        Hide();
                    };

                    if (DeviceInfo.Current.Idiom == DeviceIdiom.TV)
                    { 
                        button.Unfocused += async (s, e) =>
                        {
                            await Task.Delay(50);
                            if (!menuOptions.SelectMany(m => m.Value).Any(btn => btn.IsFocused))
                                button.Focus();
                        };
                    }

                    if (button.Selected)
                        selectedButton = button;

                    menuOptionsList.Add(button);
                }

                var scrollView = new ScrollView
                {
                    Orientation = ScrollOrientation.Vertical,
                    VerticalOptions = LayoutOptions.Fill,
                    Content = menuOptionsList
                };
                
                Grid.SetRow(scrollView, 1);
                menuListContainer.Add(scrollView);

                MenuContainer.Add(menuListContainer);
            }

            IsVisible = true;

            if(selectedButton != null)
                selectedButton.Focus();
        }

        private void OnCloseClicked(object? sender, EventArgs e)
        {
            Hide();
        }

        private void Hide()
        {
            IsVisible = false;
            MenuContainer.Clear();

            if (DeviceInfo.Current.Idiom == DeviceIdiom.TV)
                RemoteCommandActionNotifier.Instance.OnBackButtonPressed -= OnCloseClicked;

            this.CloseButton.Clicked -= OnCloseClicked;
        }
    }
}