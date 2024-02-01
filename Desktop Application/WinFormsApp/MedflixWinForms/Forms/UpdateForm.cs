using MedflixWinForms.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedflixWinForms.Forms
{
    public partial class UpdateForm : Form
    {
        private readonly AppUpdateService appUpdateService;
        public event EventHandler OnStartExtractUpdate;
        public UpdateForm()
        {
            InitializeComponent();
        }

        public UpdateForm(AppUpdateService appUpdateService) : this()
        {
            this.appUpdateService = appUpdateService;
        }

        private async void YesButton_Click(object sender, EventArgs e)
        {
            this.YesButton.Visible = false;
            this.NoButton.Visible = false;
            this.UpdateMessage.Dock = DockStyle.Fill;

            this.UpdateMessage.Text = "Download of the new version in progress...";

            var success = await this.appUpdateService.DownloadNewReleaseAsync();
            if (success)
            {
                this.UpdateMessage.Text = "The new version has been downloaded with success. The application will be closed to start the update"; 
                await Task.Delay(3000);

                var updateStarted = this.appUpdateService.StartExtractUpdate();
                if (updateStarted)
                    OnStartExtractUpdate?.Invoke(this, null);
            }
            else
            {
                this.UpdateMessage.Text = "There was an error trying to download the new version."; 
                await Task.Delay(3000);
                this.Close();
            }
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
