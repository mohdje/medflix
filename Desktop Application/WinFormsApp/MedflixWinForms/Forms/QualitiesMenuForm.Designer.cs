namespace MedflixWinForms.Forms
{
    partial class QualitiesMenuForm
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
            ListContainer = new Panel();
            SuspendLayout();
            // 
            // ListContainer
            // 
            ListContainer.AutoScroll = true;
            ListContainer.BackColor = Color.Transparent;
            ListContainer.BackgroundImageLayout = ImageLayout.Center;
            ListContainer.Dock = DockStyle.Fill;
            ListContainer.Location = new Point(0, 0);
            ListContainer.Name = "ListContainer";
            ListContainer.Size = new Size(70, 120);
            ListContainer.TabIndex = 0;
            // 
            // QualitiesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(70, 120);
            Controls.Add(ListContainer);
            FormBorderStyle = FormBorderStyle.None;
            Name = "QualitiesForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "QualitiesForm";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private Panel ListContainer;
    }
}