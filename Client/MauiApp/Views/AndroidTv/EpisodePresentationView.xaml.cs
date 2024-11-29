using Medflix.Extensions;
using Medflix.Models.Media;

namespace Medflix.Views.AndroidTv
{

    public partial class EpisodePresentationView : ContentView
    {
        public event EventHandler OnClick;
        public EpisodePresentationView(Episode episode, double progress)
        {
            InitializeComponent();

            if(!string.IsNullOrEmpty(episode.ImagePath))
                ImagePreview.Source = ImageSource.FromUri(new Uri (episode.ImagePath));

            Title.Text = $"Episode {episode.EpisodeNumber}: {episode.Name}";

            if(episode.AirDate.Date <= DateTime.Now.Date)
            {
                ReleaseDate.IsVisible = false;

                Runtime.Text = TimeSpan.FromMinutes(episode.RunTime).ToTimeFormat();
                Synopsis.Text = episode.Overview;
            }
            else
            {
                ReleaseDate.Text =$"Release date :{episode.AirDate.ToString("dd-MM-yyyy")}";
            }

            if (progress > 0)
            {
                WatchProgress.IsVisible = true;
                WatchProgress.ProgressTo(progress, 0, Easing.Default);
            }
            else
                WatchProgress.IsVisible = false;

            HiddenButton.Focused += (s, e) => EpisodePresentationContainer.Stroke = Brush.White.Color;
            HiddenButton.Unfocused += (s, e) => EpisodePresentationContainer.Stroke = Brush.Transparent.Color;
            HiddenButton.Clicked += async (s, e) =>
            {
                EpisodePresentationContainer.Stroke = Brush.Red.Color;
                await Task.Delay(300);
                EpisodePresentationContainer.Stroke = Brush.White.Color;
                OnClick?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}