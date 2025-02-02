using Medflix.Services;

namespace Medflix.Views.AndroidTv;

public partial class WatchHistoryView : ContentView
{
	public WatchHistoryView()
	{
		InitializeComponent();
	}

    private void OnLoaded(object sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            MediaListView.ShowSpinner = true;

            await Task.Delay(500);

            var medias = await MedflixApiService.Instance.GetWatchMediaHistory();

            if (medias != null && medias.Any())
            {
                MediaListView.AddMedias(medias);
            }

            MediaListView.ShowSpinner = false;
        });
    }
}