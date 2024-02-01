namespace MedflixWinForms.Forms
{
    partial class UpdateForm
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
            YesButton = new Button();
            UpdateMessage = new Label();
            NoButton = new Button();
            SuspendLayout();
            // 
            // YesButton
            // 
            YesButton.Location = new Point(153, 75);
            YesButton.Name = "YesButton";
            YesButton.Size = new Size(95, 23);
            YesButton.TabIndex = 0;
            YesButton.Text = "Yes";
            YesButton.UseVisualStyleBackColor = true;
            YesButton.Click += YesButton_Click;
            // 
            // UpdateMessage
            // 
            UpdateMessage.BackColor = Color.Transparent;
            UpdateMessage.Dock = DockStyle.Top;
            UpdateMessage.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            UpdateMessage.ForeColor = Color.White;
            UpdateMessage.Location = new Point(0, 0);
            UpdateMessage.Name = "UpdateMessage";
            UpdateMessage.Padding = new Padding(10);
            UpdateMessage.Size = new Size(507, 72);
            UpdateMessage.TabIndex = 1;
            UpdateMessage.Text = "A new version of Medflix is available. Do you want to install it ?";
            UpdateMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // NoButton
            // 
            NoButton.Location = new Point(268, 75);
            NoButton.Name = "NoButton";
            NoButton.Size = new Size(95, 23);
            NoButton.TabIndex = 2;
            NoButton.Text = "No";
            NoButton.UseVisualStyleBackColor = true;
            NoButton.Click += NoButton_Click;
            // 
            // UpdateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.gradient_background;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(507, 114);
            Controls.Add(NoButton);
            Controls.Add(UpdateMessage);
            Controls.Add(YesButton);
            FormBorderStyle = FormBorderStyle.None;
            Name = "UpdateForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "UpdateForm";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private Button YesButton;
        private Label UpdateMessage;
        private Button NoButton;
    }
}