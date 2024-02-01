namespace MedflixWinForms.Forms
{
    partial class CastMenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LoadingSpinner = new PictureBox();
            CastToDevice = new CheckBox();
            DevicesListContainer = new Panel();
            ((System.ComponentModel.ISupportInitialize)LoadingSpinner).BeginInit();
            SuspendLayout();
            // 
            // LoadingSpinner
            // 
            LoadingSpinner.Dock = DockStyle.Top;
            LoadingSpinner.Image = Properties.Resources.spinner;
            LoadingSpinner.Location = new Point(0, 69);
            LoadingSpinner.Name = "LoadingSpinner";
            LoadingSpinner.Size = new Size(208, 48);
            LoadingSpinner.SizeMode = PictureBoxSizeMode.Zoom;
            LoadingSpinner.TabIndex = 0;
            LoadingSpinner.TabStop = false;
            LoadingSpinner.Visible = false;
            LoadingSpinner.Padding = new Padding(10);
            // 
            // SearchCastDevices
            // 
            CastToDevice.AutoSize = true;
            CastToDevice.BackColor = Color.Transparent;
            CastToDevice.CheckAlign = ContentAlignment.BottomCenter;
            CastToDevice.Dock = DockStyle.Top;
            CastToDevice.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            CastToDevice.ForeColor = Color.White;
            CastToDevice.Location = new Point(0, 0);
            CastToDevice.Name = "SearchCastDevices";
            CastToDevice.Padding = new Padding(0, 10, 0, 20);
            CastToDevice.Size = new Size(208, 69);
            CastToDevice.TabIndex = 1;
            CastToDevice.Text = "Cast to device";
            CastToDevice.TextAlign = ContentAlignment.MiddleCenter;
            CastToDevice.UseVisualStyleBackColor = false;
            CastToDevice.CheckedChanged += SearchCastDevices_CheckedChanged;
            // 
            // DevicesListContainer
            // 
            DevicesListContainer.AutoScroll = true;
            DevicesListContainer.AutoSize = true;
            DevicesListContainer.Dock = DockStyle.Top;
            DevicesListContainer.Location = new Point(0, 117);
            DevicesListContainer.MaximumSize = new Size(0, 200);
            DevicesListContainer.Name = "DevicesListContainer";
            DevicesListContainer.Size = new Size(208, 0);
            DevicesListContainer.TabIndex = 2;
            // 
            // CastMenuForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.Black;
            ClientSize = new Size(208, 120);
            Controls.Add(DevicesListContainer);
            Controls.Add(LoadingSpinner);
            Controls.Add(CastToDevice);
            Name = "CastMenuForm";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)LoadingSpinner).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox LoadingSpinner;
        private CheckBox CastToDevice;
        private Panel DevicesListContainer;
    }
}