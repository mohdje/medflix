using MedflixWinForms.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedflixWinForms.Forms
{
    public class VideoPlayerMenuForm : Form
    {
        private Point callerLocation;
        public VideoPlayerMenuForm(Point callerLocation)
        {
            InitializeComponent();
            this.callerLocation = callerLocation;
            this.BackColor = Color.Black;
            this.ShowInTaskbar = false;
            this.TopMost = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Location = new Point(callerLocation.X - this.Width / 4, callerLocation.Y - this.Height);
            base.OnLoad(e);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // VideoPlayerMenuForm
            // 
            BackColor = Color.Black;
            ClientSize = new Size(284, 261);
            Controls.Add(Header);
            FormBorderStyle = FormBorderStyle.None;
            Name = "VideoPlayerMenuForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ResumeLayout(false);
        }

        protected Label CreateButtonLabel(string text)
        {
            var languageButton = new Label();
            languageButton.Text = text;
            languageButton.TextAlign = ContentAlignment.MiddleCenter;
            languageButton.Font = new Font(languageButton.Font.FontFamily, 12);
            languageButton.Height = 30;
            languageButton.ForeColor = Color.White;
            languageButton.BackColor = Color.Transparent;
            languageButton.Dock = DockStyle.Top;
            languageButton.Cursor = Cursors.Hand;
            languageButton.MouseEnter += (s, e) => languageButton.ForeColor = Color.Gray;
            languageButton.MouseLeave += (s, e) => languageButton.ForeColor = Color.White;

            return languageButton;
        }

        private Label Header;
    }
}
