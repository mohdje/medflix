using Medflix.Models;
using MedflixWinForms.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class SubtitlesMenuForm : VideoPlayerMenuForm
    {
        public event EventHandler<string> OnSubtitlesSelected;
        public event EventHandler<double> OnOffsetChanged;
        private Panel subListPanel;

        public SubtitlesMenuForm(SubtitleOption[] subtitlesOptions,string selectedSourceUrl, double offset, Point callerLocation) : base(callerLocation)
        {
            InitializeComponent();
          
            BuildMenu(subtitlesOptions, selectedSourceUrl);
            this.OffsetUpDown.Value = (decimal)offset;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Size = this.SubtitlesOptionsContainer.Size;
            base.OnLoad(e);
        }
        private void BuildMenu(SubtitleOption[] subtitlesOptions, string selectedSourceUrl)
        {
            var noSubtitlesButton = CreateButtonLabel("No subtitles");
            if (string.IsNullOrEmpty(selectedSourceUrl))
                noSubtitlesButton.Font = new Font(noSubtitlesButton.Font, FontStyle.Bold);

            noSubtitlesButton.Click += (s, e) => this.OnSubtitlesSelected?.Invoke(this, string.Empty);

            this.LanguagesContainer.Controls.Add(noSubtitlesButton);

            foreach (var subtitleOption in subtitlesOptions)
            {
                var languageButton = CreateButtonLabel(subtitleOption.Language);
                if (subtitleOption.SubtitlesSourceUrls.Contains(selectedSourceUrl))
                    languageButton.Font = new Font(languageButton.Font, FontStyle.Bold);
                
                languageButton.Click += (s, e) =>
                {
                    CloseSubList();

                    subListPanel = new Panel();
                    subListPanel.Location = new Point(this.SubtitlesOptionsContainer.Width, 0);
                    subListPanel.BackColor = ColorTranslator.FromHtml("#242322");
                    subListPanel.AutoScroll = true;

                    var i = subtitleOption.SubtitlesSourceUrls.Length + 1;
                    foreach (var sourceUrl in subtitleOption.SubtitlesSourceUrls)
                    {
                        i--;
                        var lbl = CreateButtonLabel($"{subtitleOption.Language} {i}");
                        lbl.Click += (s, e) => this.OnSubtitlesSelected?.Invoke(this, sourceUrl);
                        if (selectedSourceUrl == sourceUrl)
                            lbl.Font = new Font(lbl.Font, FontStyle.Bold);
                        
                        subListPanel.Controls.Add(lbl);
                    }

                    subListPanel.Height = this.Height;
                    subListPanel.Width = this.Width;
                    this.Width = 2 * this.Width;

                    this.Controls.Add(subListPanel);
                };

                this.LanguagesContainer.Controls.Add(languageButton);
            }

        }

        private void OffsetUpDown_Click(object sender, EventArgs e)
        {
            CloseSubList();
        }

        private void OffsetUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.OnOffsetChanged?.Invoke(this, (double)this.OffsetUpDown.Value);
        }

        private void CloseSubList()
        {
            if (subListPanel != null)
            {
                this.Controls.Remove(subListPanel);
                this.Width = this.Width / 2;
                subListPanel.Dispose();
                subListPanel = null;
            }
        }
    }
}
