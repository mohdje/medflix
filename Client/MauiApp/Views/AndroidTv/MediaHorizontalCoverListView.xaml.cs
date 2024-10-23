using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;

namespace Medflix.Views.AndroidTv
{
    public partial class MediaHorizontalCoverListView : ContentView
    {
        public MediaHorizontalCoverListView(IEnumerable<MediaDetails> medias)
        {
            InitializeComponent();

            foreach (var media in medias)
                MediaList.Children.Add(new MediaHorizontalCoverView(media));
        }
    }
}