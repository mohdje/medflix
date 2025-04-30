using Medflix.Models.VideoPlayer;
using Medflix.Services;

namespace Medflix.Views
{

    public partial class VideoPlayerSideMenuView : ContentView
    {
        public event EventHandler OnNoSubtitlesClick;
        public event EventHandler<string> OnDisplaySubtitlesClick;
        public event EventHandler<string> OnVideoQualityClick;

        VideoPlayerMenu subtitlesMenuContainer;
        VideoPlayerMenu qualitiesMenuContainer;

        public VideoPlayerSideMenuView()
        {
            InitializeComponent();

            IsVisible = false;

            this.CloseButton.IsVisible = DeviceInfo.Current.Idiom == DeviceIdiom.Desktop || DeviceInfo.Current.Idiom == DeviceIdiom.Phone;
        }

        public void Init(SubtitlesSources[] subtitlesSources, MediaSources[] mediaSources, string defaultVideoUrl)
        {
            subtitlesMenuContainer = new VideoPlayerMenu("Subtitles");
            subtitlesMenuContainer.AddMenu("No subtitles", true, onClickAction: () => OnNoSubtitlesClick?.Invoke(this, null));
            foreach (var sub in subtitlesSources)
            {
                var urlsMenuContainer = new VideoPlayerMenu($"{sub.Language} subtitles");
                for (int i = 0; i < sub.Urls.Length; i++)
                {
                    var url = sub.Urls[i];
                    urlsMenuContainer.AddMenu($"{sub.Language} {i + 1}", onClickAction: () => OnDisplaySubtitlesClick?.Invoke(this, url));
                }

                subtitlesMenuContainer.AddMenu(sub.Language, subMenusContainer: urlsMenuContainer, onClickAction: () => ShowMenus(urlsMenuContainer));
            }

            qualitiesMenuContainer = new VideoPlayerMenu("Versions");
            var qualityIndexes = new Dictionary<string, int>();
            foreach (var mediaSource in mediaSources)
            {
                var urlsMenuContainer = new VideoPlayerMenu($"{mediaSource.Language} qualities");

                bool parentSelected = false;
                foreach (var videoSource in mediaSource.Sources.OrderBy(t => t.Quality))
                {
                    var text = "";
                    bool sourceSelected = false;
                    if (qualityIndexes.ContainsKey(videoSource.Quality))
                    {
                        qualityIndexes[videoSource.Quality]++;
                        text = $"{videoSource.Quality} ({qualityIndexes[videoSource.Quality]})";
                    }
                    else
                    {
                        qualityIndexes.Add(videoSource.Quality, 0);
                        text = $"{videoSource.Quality}";
                    }

                    if (defaultVideoUrl == videoSource.TorrentUrl || defaultVideoUrl == videoSource.FilePath)
                    {
                        parentSelected = true;
                        sourceSelected = true;
                    }

                    urlsMenuContainer.AddMenu(text, selected: sourceSelected, onClickAction: () => OnVideoQualityClick?.Invoke(this, !string.IsNullOrEmpty(videoSource.FilePath) ? videoSource.FilePath : videoSource.TorrentUrl));
                }

                qualitiesMenuContainer.AddMenu(mediaSource.Language, selected: parentSelected, subMenusContainer: urlsMenuContainer, onClickAction: () => ShowMenus(urlsMenuContainer));
            }

        }

        public void ShowSubtitlesMenu()
        {
            ShowMenus(subtitlesMenuContainer);
        }

        public void ShowVideoQualitiesMenu()
        {
            ShowMenus(qualitiesMenuContainer);
        }

        private void ShowMenus(VideoPlayerMenu playerMenusContainer)
        {
            RemoteCommandActionNotifier.Instance.PreventLeftButton = true;
            RemoteCommandActionNotifier.Instance.PreventRightButton = true;
            RemoteCommandActionNotifier.Instance.OnBackButtonPressed += OnCloseClicked;
            this.CloseButton.Clicked += OnCloseClicked;

            MenuContainer.Clear();

            foreach (var menu in playerMenusContainer.Menus)
            {
                if(menu.Child == null)
                {
                    menu.Button.Clicked += async (s, e) =>
                    {
                        await Task.Delay(300);
                        Hide();
                    };                 
                }

                MenuContainer.Add(menu.Button);
            }

            var menuToFocus = playerMenusContainer.Menus.FirstOrDefault(m => m.Selected) ?? playerMenusContainer.Menus.First();
            menuToFocus.Button.Focus();

            MenuTitle.Text = playerMenusContainer.Title;

            IsVisible = true;
        }

        private void OnCloseClicked(object? sender, EventArgs e)
        {
            Hide();
        }

        private void Hide()
        {
            IsVisible = false;
            MenuContainer.Clear();

            RemoteCommandActionNotifier.Instance.PreventLeftButton = false;
            RemoteCommandActionNotifier.Instance.PreventRightButton = false;
            RemoteCommandActionNotifier.Instance.OnBackButtonPressed -= OnCloseClicked;

            this.CloseButton.Clicked -= OnCloseClicked;
        }
    }
}