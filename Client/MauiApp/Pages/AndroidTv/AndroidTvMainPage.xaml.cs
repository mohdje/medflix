using Medflix.Models;
using Medflix.Services;
using Medflix.Views.AndroidTv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Medflix.Pages.AndroidTv
{
    public partial class AndroidTvMainPage : ContentPage
    {
        HomePageView cacheHomePageView;
        HomePageView HomePageView
        {
            get
            {
                if (cacheHomePageView == null)
                {
                    cacheHomePageView = new HomePageView();
                }
                return cacheHomePageView;
            }
            set
            {
                cacheHomePageView.Content = value;
            }
        }

        public AndroidTvMainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (!ActivePage.Children.Any())
                ShowView(HomePageView);

            MedflixApiService.Instance.ContextChanged += (s, e) =>
            {
                cacheHomePageView = null;
                ShowView(HomePageView);
            };

            base.OnAppearing();
        }

        private void ShowView(ContentView contentView)
        {
            ActivePage.Children.Clear();
            contentView.SetValue(Grid.RowProperty, 0);
            contentView.SetValue(Grid.ColumnProperty, 0);
            ActivePage.Children.Add(contentView);
        }

        private void HomeButtonClicked(object sender, EventArgs e)
        {
            ShowView(HomePageView);
        }

        private void SearchButtonClicked(object sender, EventArgs e)
        {
            ShowView(new SearchPageView());
        }

        private void OnWatchHistoryButtonClicked(object sender, EventArgs e)
        {
            ShowView(new WatchHistoryView());
        }

        private void OnBookmarkButtonButtonClicked(object sender, EventArgs e)
        {
            ShowView(new BookmarkView());
        }
    }
}