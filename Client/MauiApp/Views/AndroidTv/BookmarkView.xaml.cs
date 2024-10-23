using Medflix.Services;
using System.Runtime.CompilerServices;

namespace Medflix.Views.AndroidTv;

public partial class BookmarkView : ContentView
{
	public BookmarkView()
	{
		InitializeComponent();
	}

    private void OnLoaded(object sender, EventArgs e)
    {
        Spinner.IsVisible = true;

        Task.Run(async () =>
        {
            var medias = await MedflixApiService.Instance.GetBookmarkedMedias();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (medias != null && medias.Any())
                {
                    MediaListView.AddMedias(medias);
                }

                Spinner.IsVisible = false;
            });
        });
    }
}