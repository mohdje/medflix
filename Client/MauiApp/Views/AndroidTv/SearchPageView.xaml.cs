
using Medflix.Controls.AndroidTv;

using Medflix.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Medflix.Views.AndroidTv
{

	public partial class SearchPageView : ContentView
	{
		public SearchPageView()
		{
			InitializeComponent();
		}

        private void SearchCompleted(object sender, EventArgs e)
        {
            var text = SearchInput.Text;
            NoResultMessage.IsVisible = false;

            if (!string.IsNullOrEmpty(text))
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    SearchResultContainer.Clear();

                    SearchSpinner.IsVisible = true;

                    var result = await MedflixApiService.Instance.SearchMedia(text);

                    SearchSpinner.IsVisible = false;

                    if (result != null && result.Any())
                        SearchResultContainer.Children.Add(new MediaLitePresentationListView($"{result.Count()} result(s)", result.ToArray()));
                    else
                        NoResultMessage.IsVisible = true;
                });
            }

        }
    }
}