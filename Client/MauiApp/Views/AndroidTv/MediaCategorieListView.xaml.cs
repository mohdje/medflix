using LibVLCSharp.Shared;
using Medflix.Models.Media;

namespace Medflix.Views.AndroidTv;

public partial class MediaCategorieListView : ContentView
{
    public string TitleText
	{
		get
		{
			return ListTitle.Text;
		}
		set
		{
			ListTitle.Text = value;
		}
	}

    public bool ShowSpinner
    {
        get
        {
            return Spinner.IsVisible;
        }
        set
        {
            MediaList.IsVisible = !value;
            Spinner.IsVisible = value;
        }
    }

    public MediaCategorieListView()
	{
		InitializeComponent();
	}

	public void AddMedias(IEnumerable<MediaDetails> medias)
	{
		MediaList.Clear();

        foreach (var media in medias)
		{
			var litePresentation = new MediaLitePresentationView(media);

			MediaList.Add(litePresentation);
		}
	}

    public void AddMedias(IEnumerable<WatchMediaInfo> watchMedias)
    {
        MediaList.Clear();

        foreach (var media in watchMedias)
        {
            var litePresentation = new MediaLitePresentationView(media);

            MediaList.Add(litePresentation);
        }
    }
}