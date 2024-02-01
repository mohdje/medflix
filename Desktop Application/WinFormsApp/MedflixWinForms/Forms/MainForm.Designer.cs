namespace MedflixWinforms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            WebView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)WebView).BeginInit();
            SuspendLayout();
            // 
            // WebView
            // 
            WebView.AllowExternalDrop = true;
            WebView.CreationProperties = null;
            WebView.DefaultBackgroundColor = Color.White;
            WebView.Dock = DockStyle.Fill;
            WebView.Location = new Point(0, 0);
            WebView.Margin = new Padding(3, 2, 3, 2);
            WebView.Name = "WebView";
            WebView.Size = new Size(1117, 628);
            WebView.TabIndex = 0;
            WebView.Visible = false;
            WebView.ZoomFactor = 1D;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            BackgroundImage = MedflixWinForms.Properties.Resources.logo;
            BackgroundImageLayout = ImageLayout.Center;
            ClientSize = new Size(1117, 628);
            Controls.Add(WebView);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(1133, 667);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Medflix";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)WebView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 WebView;
    }
}