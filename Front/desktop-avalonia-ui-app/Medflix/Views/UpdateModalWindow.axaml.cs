using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace Medflix.Views
{
    public partial class UpdateModalWindow : Window
    {
        public event EventHandler OnConfirm;
        public event EventHandler OnDecline;
        public UpdateModalWindow()
        {
            InitializeComponent();

            this.YesBtn.Click += (s, e) =>
            {
                this.YesBtn.IsVisible = false;
                this.NoBtn.IsVisible = false;
                OnConfirm?.Invoke(s, e);              
            };
            this.NoBtn.Click += (s, e) => OnDecline?.Invoke(s, e);
        }

        public void NotifyDownloadInProgress()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                this.UpdateMessage.Text = "Download update in progress, please wait...";

            });
        }

        public void NotifyInstallUpdate()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                this.UpdateMessage.Text = "New version downloaded, the application will be closed to install the new version.";
            });
        }

        public void NotifyError()
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                this.UpdateMessage.Text = "An error occured. Update aborted.";
            });
        }
    }
}
