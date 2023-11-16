using Avalonia.Controls;
using Avalonia.Interactivity;
using Medflix.Tools;

namespace Medflix.Views
{
    public partial class SplashScreenView : UserControl
    {
        public SplashScreenView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            this.AppVersion.Text = Consts.AppVersion;
        }
    }
}
