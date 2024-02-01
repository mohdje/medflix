
using Microsoft.AspNetCore.Server.IIS.Core;
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
    public partial class CastMenuForm : VideoPlayerMenuForm
    {
        public event EventHandler OnStartSearchingDevices;
        public event EventHandler OnStopSearchingDevices;
        public event EventHandler<string> OnCastDeviceSelected;

        private Point startLocation;
        public CastMenuForm(bool isSearchingDevices, Point callerLocation) : base(callerLocation)
        {
            InitializeComponent();

            this.CastToDevice.Checked = isSearchingDevices;

            this.startLocation = callerLocation;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.startLocation = new Point(this.Location.X, this.startLocation.Y - this.CastToDevice.Height);
            UpdateHeightAndLocation();
        }
        public void UpdateCastDevicesList(string[] devicesNames, string selectedDeviceName)
        {
            this.Invoke(() =>
            {
                this.LoadingSpinner.Visible = this.CastToDevice.Checked && !devicesNames.Any();

                this.DevicesListContainer.Controls.Clear();

                if(devicesNames != null && devicesNames.Any())
                {
                    foreach (var deviceName in devicesNames)
                    {
                        var buttonLabel = CreateButtonLabel(deviceName);

                        if (selectedDeviceName == deviceName)
                            buttonLabel.Font = new Font(buttonLabel.Font, FontStyle.Bold);
                        else
                            buttonLabel.Click += (s, e) => this.OnCastDeviceSelected?.Invoke(this, deviceName);

                        buttonLabel.AutoSize = true;
                        buttonLabel.MaximumSize = new Size(this.Width, 0);
                        buttonLabel.Padding = new Padding(10, 5, 0, 5);
                        buttonLabel.TextAlign = ContentAlignment.MiddleLeft;

                        this.DevicesListContainer.Controls.Add(buttonLabel);
                    }
                }

                UpdateHeightAndLocation();
            });
        }

        private void SearchCastDevices_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CastToDevice.Checked)
            {
                this.LoadingSpinner.Visible = true;
                this.DevicesListContainer.Visible = true;
                this.OnStartSearchingDevices?.Invoke(this, null);
            }
            else
            {
                this.LoadingSpinner.Visible = false;
                this.DevicesListContainer.Visible = false;
                this.OnStopSearchingDevices?.Invoke(this, null);
            }

            UpdateHeightAndLocation();
        }

        private void UpdateHeightAndLocation()
        {
            if (this.CastToDevice.Checked)
            {
                if (this.LoadingSpinner.Visible)
                {
                    this.Height = this.CastToDevice.Height + this.LoadingSpinner.Height;
                    this.Location = new Point(this.startLocation.X, this.startLocation.Y - this.LoadingSpinner.Height);
                }
                else
                {
                    this.Height = this.CastToDevice.Height + this.DevicesListContainer.Height;
                    this.Location = new Point(this.startLocation.X, this.startLocation.Y - this.DevicesListContainer.Height);
                }
            }
            else
            {
                this.Height = this.CastToDevice.Height;
                this.Location = this.startLocation;
            }
        }
    }
}
