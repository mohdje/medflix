using Medflix.Models;
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
    public partial class QualitiesMenuForm : VideoPlayerMenuForm
    {
        public event EventHandler<QualitySelectedEventArgs> OnQualitySelected;
        private Label selectedQualityLabel;
        public QualitiesMenuForm(VideoOption[] videoOptions, Point callerLocation) : base(callerLocation)
        {
            InitializeComponent();

            SetQualitiesList(videoOptions);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ListContainer.ScrollControlIntoView(selectedQualityLabel);
        }

        private void SetQualitiesList(VideoOption[] videoOptions)
        {
            var qualityIndexes = new Dictionary<string, int>();
            var qualityLabels = new List<Label>();

            foreach (var videoOption in videoOptions.OrderBy(videoOption => videoOption.Quality, StringComparer.OrdinalIgnoreCase))
            {
                Label qualityButtonLabel;

                if (qualityIndexes.ContainsKey(videoOption.Quality))
                {
                    qualityIndexes[videoOption.Quality]++;
                    qualityButtonLabel = CreateButtonLabel($"{videoOption.Quality} ({qualityIndexes[videoOption.Quality]})");
                }
                else
                {
                    qualityIndexes.Add(videoOption.Quality, 0);
                    qualityButtonLabel = CreateButtonLabel($"{videoOption.Quality}");
                }

                if (videoOption.Selected)
                {
                    selectedQualityLabel = qualityButtonLabel;
                    qualityButtonLabel.Font = new Font(qualityButtonLabel.Font, FontStyle.Bold);
                }

                qualityButtonLabel.Click += (s, e) => this.OnQualitySelected?.Invoke(this, new QualitySelectedEventArgs { VideoQualityOption = videoOption });
                qualityLabels.Add(qualityButtonLabel);
            }

            qualityLabels.Reverse();
            ListContainer.Controls.AddRange(qualityLabels.ToArray());
        }
    }

    public class QualitySelectedEventArgs
    {
        public VideoOption VideoQualityOption { get; set; }
    }
}
