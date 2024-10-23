
namespace Medflix.Pages.AndroidTv
{

    public partial class MediaTrailerPage : ContentPage
    {
        public MediaTrailerPage(string youtubeVideoUrl)
        {
            InitializeComponent();

            MediaTrailerWebView.Navigated += async (s, e) =>
            {
                await MediaTrailerWebView.EvaluateJavaScriptAsync($"document.body.innerHTML = \"<iframe style='background-color: black;margin: 0!important; margin-block: 0!important; margin-inline: 0 !important; padding: 0!important; border:none; position: fixed;height: 100%; width: 100%' src='{youtubeVideoUrl}' allow='autoplay'></iframe>\"");
            };
        }
    }
}