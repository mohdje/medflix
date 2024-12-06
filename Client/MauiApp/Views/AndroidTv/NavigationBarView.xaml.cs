
using Medflix.Controls.AndroidTv;
using Medflix.Services;

namespace Medflix.Views.AndroidTv
{
    public partial class NavigationBarView : ContentView
    {
        public event EventHandler OnHomeButtonClicked;
        public event EventHandler OnSearchButtonClicked;
        public event EventHandler OnWatchHistoryButtonClicked;
        public event EventHandler OnBookmarkButtonButtonClicked;
        public event EventHandler OnCategoriesButtonButtonClicked;

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
            UpdateNavBarButtonsState(HomeButton);

            MedflixApiService.Instance.SwitchToSeriesMode();
        }
        private void HomeButtonClicked(object sender, EventArgs e)
        {
            UpdateNavBarButtonsState(HomeButton);
            OnHomeButtonClicked?.Invoke(this, e);
        }

        private void SearchButtonClicked(object sender, EventArgs e)
        {
            UpdateNavBarButtonsState(SearchButton);
            OnSearchButtonClicked?.Invoke(this, e);
        }

        private void WatchHistoryButtonClicked(object sender, EventArgs e)
        {
            UpdateNavBarButtonsState(WatchHistoryButton);
            OnWatchHistoryButtonClicked?.Invoke(this, e);
        }

        private void BookmarkButtonClicked(object sender, EventArgs e)
        {
            UpdateNavBarButtonsState(BookmarkButton);
            OnBookmarkButtonButtonClicked?.Invoke(this, e);
        }

        private void CategoriesButtonClicked(object sender, EventArgs e)
        {
            UpdateNavBarButtonsState(CategoriesButton);
            OnCategoriesButtonButtonClicked?.Invoke(this, e);
        }

        private void UpdateNavBarButtonsState(NavigationBarButton selectedButton)
        {
            var buttons = new NavigationBarButton[]
            {
                HomeButton, SearchButton, WatchHistoryButton, BookmarkButton, CategoriesButton
            };

            foreach (var button in buttons)
            {
                if(selectedButton != button)
                    button.Selected = false;
            }
        }
    }
}