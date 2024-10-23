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
        }

        public void Init(SubtitlesSources[] subtitlesSources, TorrentSources[] torrentSources, string defaultVideoUrl)
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
            foreach (var torrentSource in torrentSources)
            {
                var urlsMenuContainer = new VideoPlayerMenu($"{torrentSource.Language} qualities");

                bool selected = false;
                foreach (var torrent in torrentSource.Torrents.OrderBy(t => t.Quality))
                {
                    var text = "";
                    if (qualityIndexes.ContainsKey(torrent.Quality))
                    {
                        qualityIndexes[torrent.Quality]++;
                        text = $"{torrent.Quality} ({qualityIndexes[torrent.Quality]})";
                    }
                    else
                    {
                        qualityIndexes.Add(torrent.Quality, 0);
                        text = $"{torrent.Quality}";
                    }

                    if (defaultVideoUrl == torrent.DownloadUrl)
                        selected = true;

                    urlsMenuContainer.AddMenu(text, selected: defaultVideoUrl == torrent.DownloadUrl, onClickAction: () => OnVideoQualityClick?.Invoke(this, torrent.DownloadUrl));
                }

                qualitiesMenuContainer.AddMenu(torrentSource.Language,selected: selected, subMenusContainer: urlsMenuContainer, onClickAction: () => ShowMenus(urlsMenuContainer));
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
            RemoteCommandActionNotifier.Instance.PreventBackButton = true;
            RemoteCommandActionNotifier.Instance.PreventLeftButton = true;
            RemoteCommandActionNotifier.Instance.PreventRightButton = true;
            RemoteCommandActionNotifier.Instance.OnBackButtonPressed += OnBackButtonPressed;

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

            playerMenusContainer.Menus.First().Button.Focus();
            MenuTitle.Text = playerMenusContainer.Title;

            IsVisible = true;
        }

        private void OnBackButtonPressed(object? sender, EventArgs e)
        {
            Hide();
        }

        private void Hide()
        {
            IsVisible = false;
            MenuContainer.Clear();

            RemoteCommandActionNotifier.Instance.PreventBackButton = false;
            RemoteCommandActionNotifier.Instance.PreventLeftButton = false;
            RemoteCommandActionNotifier.Instance.PreventRightButton = false;
            RemoteCommandActionNotifier.Instance.OnBackButtonPressed -= OnBackButtonPressed;
        }
    }
}