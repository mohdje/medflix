using Medflix.Controls.VideoPlayer;

namespace Medflix.Models.VideoPlayer
{
    public class VideoPlayerOption
    {
        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                Button.Selected = value;
                if (Parent != null && value)
                    Parent.UpdateSelectedStatuses(this);

                if (Child != null && !value)
                    Child.UpdateSelectedStatuses(null);
            }
        }

        public VideoPlayerMenuButton Button { get; set; }

        public Action OnClick { get; set; }

        public VideoPlayerMenu Parent { get; private set; }
        public VideoPlayerMenu Child { get; private set; }

        public VideoPlayerOption(string text, VideoPlayerMenu parent, VideoPlayerMenu child = null)
        {
            Parent = parent;
            Child = child;

            Button = new VideoPlayerMenuButton(text);
            Button.Clicked += (s, e) =>
            {
                if(Child == null)
                    Selected = true;

                OnClick?.Invoke();
            };
        }
    }
}
