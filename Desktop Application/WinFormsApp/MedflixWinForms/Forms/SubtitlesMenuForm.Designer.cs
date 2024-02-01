namespace WinFormsApp1
{
    partial class SubtitlesMenuForm
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
            OffsetUpDown = new NumericUpDown();
            LanguagesContainer = new Panel();
            OffsetLabel = new Label();
            SubtitlesOptionsContainer = new Panel();
            ((System.ComponentModel.ISupportInitialize)OffsetUpDown).BeginInit();
            SubtitlesOptionsContainer.SuspendLayout();
            SuspendLayout();
            // 
            // OffsetUpDown
            // 
            OffsetUpDown.DecimalPlaces = 2;
            OffsetUpDown.Dock = DockStyle.Top;
            OffsetUpDown.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            OffsetUpDown.Location = new Point(0, 40);
            OffsetUpDown.Minimum = new decimal(new int[] { 100, 0, 0, int.MinValue });
            OffsetUpDown.Name = "OffsetUpDown";
            OffsetUpDown.Size = new Size(148, 27);
            OffsetUpDown.TabIndex = 3;
            OffsetUpDown.TextAlign = HorizontalAlignment.Center;
            OffsetUpDown.Click += OffsetUpDown_Click;
            OffsetUpDown.ValueChanged += OffsetUpDown_ValueChanged;
            // 
            // LanguagesContainer
            // 
            LanguagesContainer.AutoSize = true;
            LanguagesContainer.Dock = DockStyle.Top;
            LanguagesContainer.Location = new Point(0, 0);
            LanguagesContainer.Name = "LanguagesContainer";
            LanguagesContainer.Size = new Size(148, 0);
            LanguagesContainer.TabIndex = 4;
            // 
            // OffsetLabel
            // 
            OffsetLabel.BackColor = Color.Transparent;
            OffsetLabel.Dock = DockStyle.Top;
            OffsetLabel.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            OffsetLabel.ForeColor = Color.White;
            OffsetLabel.Location = new Point(0, 0);
            OffsetLabel.Name = "OffsetLabel";
            OffsetLabel.Padding = new Padding(0, 10, 0, 0);
            OffsetLabel.Size = new Size(148, 40);
            OffsetLabel.TabIndex = 5;
            OffsetLabel.Text = "Offset (sec.)";
            OffsetLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // SubtitlesOptionsContainer
            // 
            SubtitlesOptionsContainer.AutoSize = true;
            SubtitlesOptionsContainer.Controls.Add(OffsetUpDown);
            SubtitlesOptionsContainer.Controls.Add(OffsetLabel);
            SubtitlesOptionsContainer.Controls.Add(LanguagesContainer);
            SubtitlesOptionsContainer.Location = new Point(0, 0);
            SubtitlesOptionsContainer.Name = "SubtitlesOptionsContainer";
            SubtitlesOptionsContainer.Size = new Size(148, 127);
            SubtitlesOptionsContainer.TabIndex = 6;
            // 
            // TestForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(150, 133);
            Controls.Add(SubtitlesOptionsContainer);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "TestForm";
            StartPosition = FormStartPosition.Manual;
            Text = "TestForm";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)OffsetUpDown).EndInit();
            SubtitlesOptionsContainer.ResumeLayout(false);
            SubtitlesOptionsContainer.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion
        private NumericUpDown OffsetUpDown;
        private Panel LanguagesContainer;
        private Label OffsetLabel;
        private Panel SubtitlesOptionsContainer;
    }
}