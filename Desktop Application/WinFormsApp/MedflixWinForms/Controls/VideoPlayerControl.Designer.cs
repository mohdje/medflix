namespace MedflixWinforms.Controls
{
    partial class VideoPlayerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            VideoView = new LibVLCSharp.WinForms.VideoView();
            ControlsContainer = new Panel();
            TimeControlsContainer = new Panel();
            ProgressTimeBarContainer = new Panel();
            TimebarBackground = new Panel();
            TimebarForeground = new Panel();
            RemainingTimeLabel = new Label();
            CurrentTimeLabel = new Label();
            PlaybackControlsContainer = new Panel();
            VolumeBarContainer = new Panel();
            VolumeBarBackground = new Panel();
            VolumeBarForeground = new Panel();
            SoundButton = new PictureBox();
            CastButton = new PictureBox();
            SubtitlesButton = new PictureBox();
            QualitiesButton = new PictureBox();
            FullScreenButton = new PictureBox();
            ForwardButton = new PictureBox();
            BackwardButton = new PictureBox();
            StopButton = new PictureBox();
            PlayPauseButton = new PictureBox();
            LoadingPanel = new Panel();
            LoadingSpinner = new PictureBox();
            LoadingMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)VideoView).BeginInit();
            ControlsContainer.SuspendLayout();
            TimeControlsContainer.SuspendLayout();
            ProgressTimeBarContainer.SuspendLayout();
            TimebarBackground.SuspendLayout();
            PlaybackControlsContainer.SuspendLayout();
            VolumeBarContainer.SuspendLayout();
            VolumeBarBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SoundButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CastButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SubtitlesButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)QualitiesButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)FullScreenButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ForwardButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BackwardButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StopButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PlayPauseButton).BeginInit();
            LoadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LoadingSpinner).BeginInit();
            SuspendLayout();
            // 
            // VideoView
            // 
            VideoView.BackColor = Color.Black;
            VideoView.Dock = DockStyle.Fill;
            VideoView.Location = new Point(0, 0);
            VideoView.Margin = new Padding(3, 2, 3, 2);
            VideoView.MediaPlayer = null;
            VideoView.Name = "VideoView";
            VideoView.Size = new Size(774, 440);
            VideoView.TabIndex = 0;
            VideoView.Text = "videoView1";
            // 
            // ControlsContainer
            // 
            ControlsContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ControlsContainer.BackColor = Color.Transparent;
            ControlsContainer.BackgroundImage = MedflixWinForms.Properties.Resources.gradient_background;
            ControlsContainer.BackgroundImageLayout = ImageLayout.Stretch;
            ControlsContainer.Controls.Add(TimeControlsContainer);
            ControlsContainer.Controls.Add(PlaybackControlsContainer);
            ControlsContainer.Dock = DockStyle.Bottom;
            ControlsContainer.Location = new Point(0, 327);
            ControlsContainer.Margin = new Padding(3, 2, 3, 2);
            ControlsContainer.Name = "ControlsContainer";
            ControlsContainer.Size = new Size(774, 113);
            ControlsContainer.TabIndex = 2;
            // 
            // TimeControlsContainer
            // 
            TimeControlsContainer.BackColor = Color.Transparent;
            TimeControlsContainer.Controls.Add(ProgressTimeBarContainer);
            TimeControlsContainer.Controls.Add(RemainingTimeLabel);
            TimeControlsContainer.Controls.Add(CurrentTimeLabel);
            TimeControlsContainer.Dock = DockStyle.Bottom;
            TimeControlsContainer.Location = new Point(0, 3);
            TimeControlsContainer.Margin = new Padding(3, 2, 3, 2);
            TimeControlsContainer.Name = "TimeControlsContainer";
            TimeControlsContainer.Size = new Size(774, 50);
            TimeControlsContainer.TabIndex = 5;
            // 
            // ProgressTimeBarContainer
            // 
            ProgressTimeBarContainer.BackColor = Color.Transparent;
            ProgressTimeBarContainer.Controls.Add(TimebarBackground);
            ProgressTimeBarContainer.Dock = DockStyle.Fill;
            ProgressTimeBarContainer.Location = new Point(80, 0);
            ProgressTimeBarContainer.Margin = new Padding(3, 2, 3, 38);
            ProgressTimeBarContainer.Name = "ProgressTimeBarContainer";
            ProgressTimeBarContainer.Size = new Size(614, 50);
            ProgressTimeBarContainer.TabIndex = 2;
            ProgressTimeBarContainer.Paint += ProgressTimeBarContainer_Paint;
            // 
            // TimebarBackground
            // 
            TimebarBackground.BackColor = Color.FromArgb(64, 64, 64);
            TimebarBackground.Controls.Add(TimebarForeground);
            TimebarBackground.Cursor = Cursors.Hand;
            TimebarBackground.Location = new Point(6, 22);
            TimebarBackground.Margin = new Padding(3, 2, 3, 2);
            TimebarBackground.Name = "TimebarBackground";
            TimebarBackground.Size = new Size(440, 10);
            TimebarBackground.TabIndex = 0;
            TimebarBackground.Tag = "user_action";
            TimebarBackground.Click += TimebarClick;
            // 
            // TimebarForeground
            // 
            TimebarForeground.BackColor = Color.Red;
            TimebarForeground.Dock = DockStyle.Left;
            TimebarForeground.Location = new Point(0, 0);
            TimebarForeground.Margin = new Padding(3, 2, 3, 2);
            TimebarForeground.Name = "TimebarForeground";
            TimebarForeground.Size = new Size(0, 10);
            TimebarForeground.TabIndex = 0;
            TimebarForeground.Tag = "user_action";
            TimebarForeground.Click += TimebarClick;
            // 
            // RemainingTimeLabel
            // 
            RemainingTimeLabel.BackColor = Color.Transparent;
            RemainingTimeLabel.Dock = DockStyle.Right;
            RemainingTimeLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            RemainingTimeLabel.ForeColor = Color.White;
            RemainingTimeLabel.Location = new Point(694, 0);
            RemainingTimeLabel.Name = "RemainingTimeLabel";
            RemainingTimeLabel.Size = new Size(80, 50);
            RemainingTimeLabel.TabIndex = 1;
            RemainingTimeLabel.Text = "-00:00:00";
            RemainingTimeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CurrentTimeLabel
            // 
            CurrentTimeLabel.BackColor = Color.Transparent;
            CurrentTimeLabel.Dock = DockStyle.Left;
            CurrentTimeLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            CurrentTimeLabel.ForeColor = Color.White;
            CurrentTimeLabel.Location = new Point(0, 0);
            CurrentTimeLabel.Name = "CurrentTimeLabel";
            CurrentTimeLabel.Size = new Size(80, 50);
            CurrentTimeLabel.TabIndex = 0;
            CurrentTimeLabel.Text = "00:00:00";
            CurrentTimeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PlaybackControlsContainer
            // 
            PlaybackControlsContainer.BackColor = Color.Transparent;
            PlaybackControlsContainer.Controls.Add(VolumeBarContainer);
            PlaybackControlsContainer.Controls.Add(SoundButton);
            PlaybackControlsContainer.Controls.Add(CastButton);
            PlaybackControlsContainer.Controls.Add(SubtitlesButton);
            PlaybackControlsContainer.Controls.Add(QualitiesButton);
            PlaybackControlsContainer.Controls.Add(FullScreenButton);
            PlaybackControlsContainer.Controls.Add(ForwardButton);
            PlaybackControlsContainer.Controls.Add(BackwardButton);
            PlaybackControlsContainer.Controls.Add(StopButton);
            PlaybackControlsContainer.Controls.Add(PlayPauseButton);
            PlaybackControlsContainer.Dock = DockStyle.Bottom;
            PlaybackControlsContainer.Location = new Point(0, 53);
            PlaybackControlsContainer.Margin = new Padding(3, 2, 3, 2);
            PlaybackControlsContainer.Name = "PlaybackControlsContainer";
            PlaybackControlsContainer.Size = new Size(774, 60);
            PlaybackControlsContainer.TabIndex = 4;
            // 
            // VolumeBarContainer
            // 
            VolumeBarContainer.BackColor = Color.Transparent;
            VolumeBarContainer.Controls.Add(VolumeBarBackground);
            VolumeBarContainer.Dock = DockStyle.Left;
            VolumeBarContainer.Location = new Point(270, 0);
            VolumeBarContainer.Name = "VolumeBarContainer";
            VolumeBarContainer.Size = new Size(115, 60);
            VolumeBarContainer.TabIndex = 11;
            VolumeBarContainer.Paint += VolumeBarContainer_Paint;
            // 
            // VolumeBarBackground
            // 
            VolumeBarBackground.BackColor = SystemColors.ControlDarkDark;
            VolumeBarBackground.Controls.Add(VolumeBarForeground);
            VolumeBarBackground.Cursor = Cursors.Hand;
            VolumeBarBackground.Location = new Point(6, 26);
            VolumeBarBackground.Name = "VolumeBarBackground";
            VolumeBarBackground.Size = new Size(81, 7);
            VolumeBarBackground.TabIndex = 0;
            VolumeBarBackground.Tag = "user_action";
            VolumeBarBackground.Click += VolumeBarClick;
            // 
            // VolumeBarForeground
            // 
            VolumeBarForeground.BackColor = Color.White;
            VolumeBarForeground.Location = new Point(0, 0);
            VolumeBarForeground.Name = "VolumeBarForeground";
            VolumeBarForeground.Size = new Size(44, 7);
            VolumeBarForeground.TabIndex = 0;
            VolumeBarForeground.Tag = "user_action";
            VolumeBarForeground.Click += VolumeBarClick;
            // 
            // SoundButton
            // 
            SoundButton.BackColor = Color.Transparent;
            SoundButton.BackgroundImage = MedflixWinForms.Properties.Resources.sound_loud;
            SoundButton.BackgroundImageLayout = ImageLayout.Center;
            SoundButton.Cursor = Cursors.Hand;
            SoundButton.Dock = DockStyle.Left;
            SoundButton.Location = new Point(220, 0);
            SoundButton.Margin = new Padding(3, 2, 3, 2);
            SoundButton.Name = "SoundButton";
            SoundButton.Size = new Size(50, 60);
            SoundButton.TabIndex = 5;
            SoundButton.TabStop = false;
            SoundButton.Tag = "user_action";
            SoundButton.Click += SoundButton_Click;
            // 
            // CastButton
            // 
            CastButton.BackColor = Color.Transparent;
            CastButton.BackgroundImage = MedflixWinForms.Properties.Resources.cast;
            CastButton.BackgroundImageLayout = ImageLayout.Center;
            CastButton.Cursor = Cursors.Hand;
            CastButton.Dock = DockStyle.Right;
            CastButton.Location = new Point(524, 0);
            CastButton.Margin = new Padding(3, 2, 3, 2);
            CastButton.Name = "CastButton";
            CastButton.Size = new Size(60, 60);
            CastButton.TabIndex = 9;
            CastButton.TabStop = false;
            CastButton.Tag = "user_action";
            CastButton.Click += CastButton_Click;
            // 
            // SubtitlesButton
            // 
            SubtitlesButton.BackColor = Color.Transparent;
            SubtitlesButton.BackgroundImage = MedflixWinForms.Properties.Resources.subtitles;
            SubtitlesButton.BackgroundImageLayout = ImageLayout.Center;
            SubtitlesButton.Cursor = Cursors.Hand;
            SubtitlesButton.Dock = DockStyle.Right;
            SubtitlesButton.Location = new Point(584, 0);
            SubtitlesButton.Margin = new Padding(3, 2, 3, 2);
            SubtitlesButton.Name = "SubtitlesButton";
            SubtitlesButton.Size = new Size(60, 60);
            SubtitlesButton.TabIndex = 8;
            SubtitlesButton.TabStop = false;
            SubtitlesButton.Tag = "user_action";
            SubtitlesButton.Click += SubtitlesButton_Click;
            // 
            // QualitiesButton
            // 
            QualitiesButton.BackColor = Color.Transparent;
            QualitiesButton.BackgroundImage = MedflixWinForms.Properties.Resources.settings;
            QualitiesButton.BackgroundImageLayout = ImageLayout.Center;
            QualitiesButton.Cursor = Cursors.Hand;
            QualitiesButton.Dock = DockStyle.Right;
            QualitiesButton.Location = new Point(644, 0);
            QualitiesButton.Margin = new Padding(3, 2, 3, 2);
            QualitiesButton.Name = "QualitiesButton";
            QualitiesButton.Size = new Size(60, 60);
            QualitiesButton.TabIndex = 7;
            QualitiesButton.TabStop = false;
            QualitiesButton.Tag = "user_action";
            QualitiesButton.Click += QualitiesButton_Click;
            // 
            // FullScreenButton
            // 
            FullScreenButton.BackColor = Color.Transparent;
            FullScreenButton.BackgroundImage = MedflixWinForms.Properties.Resources.full_screen;
            FullScreenButton.BackgroundImageLayout = ImageLayout.Center;
            FullScreenButton.Cursor = Cursors.Hand;
            FullScreenButton.Dock = DockStyle.Right;
            FullScreenButton.Location = new Point(704, 0);
            FullScreenButton.Margin = new Padding(3, 2, 3, 2);
            FullScreenButton.Name = "FullScreenButton";
            FullScreenButton.Size = new Size(70, 60);
            FullScreenButton.TabIndex = 6;
            FullScreenButton.TabStop = false;
            FullScreenButton.Tag = "user_action";
            FullScreenButton.Click += FullScreenButton_Click;
            // 
            // ForwardButton
            // 
            ForwardButton.BackColor = Color.Transparent;
            ForwardButton.BackgroundImage = MedflixWinForms.Properties.Resources.forward_10;
            ForwardButton.BackgroundImageLayout = ImageLayout.Center;
            ForwardButton.Cursor = Cursors.Hand;
            ForwardButton.Dock = DockStyle.Left;
            ForwardButton.Location = new Point(170, 0);
            ForwardButton.Margin = new Padding(3, 2, 3, 2);
            ForwardButton.Name = "ForwardButton";
            ForwardButton.Size = new Size(50, 60);
            ForwardButton.TabIndex = 4;
            ForwardButton.TabStop = false;
            ForwardButton.Tag = "user_action";
            ForwardButton.Click += ForwardButton_Click;
            // 
            // BackwardButton
            // 
            BackwardButton.BackColor = Color.Transparent;
            BackwardButton.BackgroundImage = MedflixWinForms.Properties.Resources.backward_10;
            BackwardButton.BackgroundImageLayout = ImageLayout.Center;
            BackwardButton.Cursor = Cursors.Hand;
            BackwardButton.Dock = DockStyle.Left;
            BackwardButton.Location = new Point(120, 0);
            BackwardButton.Margin = new Padding(3, 2, 3, 2);
            BackwardButton.Name = "BackwardButton";
            BackwardButton.Size = new Size(50, 60);
            BackwardButton.TabIndex = 3;
            BackwardButton.TabStop = false;
            BackwardButton.Tag = "user_action";
            BackwardButton.Click += BackwardButton_Click;
            // 
            // StopButton
            // 
            StopButton.BackColor = Color.Transparent;
            StopButton.BackgroundImage = MedflixWinForms.Properties.Resources.stop;
            StopButton.BackgroundImageLayout = ImageLayout.Center;
            StopButton.Cursor = Cursors.Hand;
            StopButton.Dock = DockStyle.Left;
            StopButton.Location = new Point(70, 0);
            StopButton.Margin = new Padding(3, 2, 3, 2);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(50, 60);
            StopButton.TabIndex = 2;
            StopButton.TabStop = false;
            StopButton.Tag = "user_action";
            StopButton.Click += StopButton_Click;
            // 
            // PlayPauseButton
            // 
            PlayPauseButton.BackColor = Color.Transparent;
            PlayPauseButton.BackgroundImage = MedflixWinForms.Properties.Resources.play;
            PlayPauseButton.BackgroundImageLayout = ImageLayout.Center;
            PlayPauseButton.Cursor = Cursors.Hand;
            PlayPauseButton.Dock = DockStyle.Left;
            PlayPauseButton.Location = new Point(0, 0);
            PlayPauseButton.Margin = new Padding(3, 2, 3, 2);
            PlayPauseButton.Name = "PlayPauseButton";
            PlayPauseButton.Size = new Size(70, 60);
            PlayPauseButton.TabIndex = 1;
            PlayPauseButton.TabStop = false;
            PlayPauseButton.Tag = "user_action";
            PlayPauseButton.Click += PlayPauseButton_Click;
            // 
            // LoadingPanel
            // 
            LoadingPanel.AutoSize = true;
            LoadingPanel.BackColor = Color.DimGray;
            LoadingPanel.BackgroundImage = MedflixWinForms.Properties.Resources.gradient_background;
            LoadingPanel.BackgroundImageLayout = ImageLayout.Stretch;
            LoadingPanel.Controls.Add(LoadingSpinner);
            LoadingPanel.Controls.Add(LoadingMessage);
            LoadingPanel.Location = new Point(200, 0);
            LoadingPanel.Name = "LoadingPanel";
            LoadingPanel.Size = new Size(249, 48);
            LoadingPanel.TabIndex = 1;
            LoadingPanel.Visible = false;
            // 
            // LoadingSpinner
            // 
            LoadingSpinner.BackColor = Color.Transparent;
            LoadingSpinner.Dock = DockStyle.Left;
            LoadingSpinner.Image = MedflixWinForms.Properties.Resources.spinner_loading;
            LoadingSpinner.Location = new Point(140, 0);
            LoadingSpinner.Name = "LoadingSpinner";
            LoadingSpinner.Size = new Size(106, 48);
            LoadingSpinner.SizeMode = PictureBoxSizeMode.Zoom;
            LoadingSpinner.TabIndex = 1;
            LoadingSpinner.TabStop = false;
            // 
            // LoadingMessage
            // 
            LoadingMessage.AutoSize = true;
            LoadingMessage.BackColor = Color.Transparent;
            LoadingMessage.Dock = DockStyle.Left;
            LoadingMessage.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
            LoadingMessage.ForeColor = Color.White;
            LoadingMessage.Location = new Point(0, 0);
            LoadingMessage.Name = "LoadingMessage";
            LoadingMessage.Padding = new Padding(30, 10, 30, 10);
            LoadingMessage.Size = new Size(140, 45);
            LoadingMessage.TabIndex = 0;
            LoadingMessage.Text = "Loading";
            LoadingMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // VideoPlayerControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(LoadingPanel);
            Controls.Add(ControlsContainer);
            Controls.Add(VideoView);
            Margin = new Padding(3, 2, 3, 2);
            Name = "VideoPlayerControl";
            Size = new Size(774, 440);
            Paint += VideoPlayerControl_Paint;
            ((System.ComponentModel.ISupportInitialize)VideoView).EndInit();
            ControlsContainer.ResumeLayout(false);
            TimeControlsContainer.ResumeLayout(false);
            ProgressTimeBarContainer.ResumeLayout(false);
            TimebarBackground.ResumeLayout(false);
            PlaybackControlsContainer.ResumeLayout(false);
            VolumeBarContainer.ResumeLayout(false);
            VolumeBarBackground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SoundButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)CastButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)SubtitlesButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)QualitiesButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)FullScreenButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)ForwardButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)BackwardButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)StopButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)PlayPauseButton).EndInit();
            LoadingPanel.ResumeLayout(false);
            LoadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LoadingSpinner).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView VideoView;
        private Panel ControlsContainer;
        private PictureBox PlayPauseButton;
        private Panel PlaybackControlsContainer;
        private Panel TimeControlsContainer;
        private Label RemainingTimeLabel;
        private Label CurrentTimeLabel;
        private Panel ProgressTimeBarContainer;
        private Panel TimebarBackground;
        private Panel TimebarForeground;
        private PictureBox StopButton;
        private PictureBox ForwardButton;
        private PictureBox BackwardButton;
        private PictureBox FullScreenButton;
        private PictureBox CastButton;
        private PictureBox SubtitlesButton;
        private PictureBox QualitiesButton;
        private PictureBox SoundButton;
        private Panel VolumeBarContainer;
        private Panel VolumeBarBackground;
        private Panel VolumeBarForeground;
        private Panel LoadingPanel;
        private Label LoadingMessage;
        private PictureBox LoadingSpinner;
    }
}
