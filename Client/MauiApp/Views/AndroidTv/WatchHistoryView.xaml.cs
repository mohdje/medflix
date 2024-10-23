using Medflix.Services;
using System.Runtime.CompilerServices;

namespace Medflix.Views.AndroidTv;

public partial class WatchHistoryView : ContentView
{
	public WatchHistoryView()
	{
		InitializeComponent();
	}

    private void OnLoaded(object sender, EventArgs e)
    {
		Spinner.IsVisible = true;

		Task.Run(async () =>
		{
            var watchMediaInfos = await MedflixApiService.Instance.GetWatchMediaHistory();

            MainThread.BeginInvokeOnMainThread(() => 
            {
                if (watchMediaInfos != null && watchMediaInfos.Any())
                {
                    MediaListView.AddMedias(watchMediaInfos);
                }

                Spinner.IsVisible = false;
            });
        });
    }
}