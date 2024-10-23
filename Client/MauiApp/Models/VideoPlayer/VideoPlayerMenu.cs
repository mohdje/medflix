namespace Medflix.Models.VideoPlayer
{
    public class VideoPlayerMenu
    {
        private List<VideoPlayerOption> options;
        public VideoPlayerOption[] Menus => options.ToArray();

        public VideoPlayerOption ParentMenu { get; set; }

        public string Title { get; set; }

        public VideoPlayerMenu(string title)
        {
            Title = title;

            options = new List<VideoPlayerOption>();
        }

        public void UpdateSelectedStatuses(VideoPlayerOption selectedMenu)
        {
            foreach (var menu in Menus.Where(menu => menu != selectedMenu))
            {
                menu.Selected = false;
            }

            if (ParentMenu != null && selectedMenu != null)
                ParentMenu.Selected = true;
        }

        public void AddMenu( string text, bool selected = false, Action onClickAction = null, VideoPlayerMenu subMenusContainer = null)
        {
            var option = new VideoPlayerOption(text, this, subMenusContainer);
            if (onClickAction != null)
                option.OnClick = onClickAction;

            option.Selected = selected;

            if (subMenusContainer != null)
                subMenusContainer.ParentMenu = option;

            options.Add(option);
        }
    }
}
