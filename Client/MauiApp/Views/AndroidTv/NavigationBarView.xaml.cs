
using Medflix.Services;

namespace Medflix.Views.AndroidTv
{
    public partial class NavigationBarView : ContentView
    {
        public event EventHandler OnHomeButtonClicked;
        public event EventHandler OnSearchButtonClicked;
        public event EventHandler OnWatchHistoryButtonClicked;
        public event EventHandler OnBookmarkButtonButtonClicked;

        public NavigationBarView()
        {
            InitializeComponent();
        }

        private void MoviesButtonClicked(object sender, EventArgs e)
        {
            SeriesButton.Selected = false;

            HomeButton.Selected = true;
            SearchButton.Selected = false;
            WatchHistoryButton.Selected = false;
            BookmarkButton.Selected = false;

            MedflixApiService.Instance.SwitchToMoviesMode();
        }

        private void SeriesButtonClicked(object sender, EventArgs e)
        {
            MoviesButton.Selected = false;

            HomeButton.Selected = true;
            SearchButton.Selected = false;
            WatchHistoryButton.Selected = false;
            BookmarkButton.Selected = false;

            MedflixApiService.Instance.SwitchToSeriesMode();
        }
        private void HomeButtonClicked(object sender, EventArgs e)
        {
            SearchButton.Selected = false;
            WatchHistoryButton.Selected = false;
            BookmarkButton.Selected = false;

            OnHomeButtonClicked?.Invoke(this, e);
        }

        private void SearchButtonClicked(object sender, EventArgs e)
        {
            HomeButton.Selected = false;
            WatchHistoryButton.Selected = false;
            BookmarkButton.Selected = false;

            OnSearchButtonClicked?.Invoke(this, e);
        }

        private void WatchHistoryButtonClicked(object sender, EventArgs e)
        {
            HomeButton.Selected = false;
            SearchButton.Selected = false;
            BookmarkButton.Selected = false;

            OnWatchHistoryButtonClicked?.Invoke(this, e);
        }

        private void BookmarkButtonClicked(object sender, EventArgs e)
        {
            HomeButton.Selected = false;
            SearchButton.Selected = false;
            WatchHistoryButton.Selected = false;

            OnBookmarkButtonButtonClicked?.Invoke(this, e);
        }

    }
}