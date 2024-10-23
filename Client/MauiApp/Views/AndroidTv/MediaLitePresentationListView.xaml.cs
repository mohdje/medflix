using LibVLCSharp.Shared;
using Medflix.Models.Media;
using Medflix.Models.VideoPlayer;
using System.Runtime.CompilerServices;

namespace Medflix.Views.AndroidTv
{

    public partial class MediaLitePresentationListView : ContentView
    {
        IEnumerable<MediaDetails> medias;
        public MediaLitePresentationListView(string title, IEnumerable<MediaDetails> medias)
        {
            InitializeComponent();

            Title.Text = title;

            this.medias = medias;

            DisplayNextMedias(false);

            PlusButton.Clicked += PlusButtonClicked;
        }

        private void DisplayNextMedias(bool setFocus)
        {
            var amountToDisplay = 6;
            var startIndex = MediaList.Children.Count;

            foreach (var media in medias.Take(new Range(startIndex, startIndex + amountToDisplay)))
                MediaList.Children.Add(new MediaLitePresentationView(media));

            if (setFocus)
                MediaList.Children[startIndex].Focus();

            PlusButton.IsVisible = MediaList.Children.Count < medias.Count();
        }

        private async void PlusButtonClicked(object? sender, EventArgs e)
        {
            MediaList.Children.Last().Focus();

            Spinner.IsVisible = true;
            PlusButton.IsVisible = false;

            await Task.Delay(300);

            DisplayNextMedias(true);

            Spinner.IsVisible = false;
        }

        private void PlusButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}