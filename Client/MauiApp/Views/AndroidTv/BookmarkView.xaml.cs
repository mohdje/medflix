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
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            MediaListView.ShowSpinner = true;

            await Task.Delay(500);

            var medias = await MedflixApiService.Instance.GetBookmarkedMedias();

            if (medias != null && medias.Any())
            {
                MediaListView.AddMedias(medias);
            }

            MediaListView.ShowSpinner = false;
        });
    }
}