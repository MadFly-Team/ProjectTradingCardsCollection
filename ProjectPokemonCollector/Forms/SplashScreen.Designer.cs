namespace ProjectMTGCollector.Forms
{
    partial class SplashScreen
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblVersion = new Label();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font( "Segoe UI", 24F, FontStyle.Bold );
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point( 62, 125 );
            lblTitle.Margin = new Padding( 4, 0, 4, 0 );
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size( 625, 62 );
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Pokémon Card Collector";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            lblSubtitle.Font = new Font( "Segoe UI", 14F );
            lblSubtitle.ForeColor = Color.LightGray;
            lblSubtitle.Location = new Point( 62, 233 );
            lblSubtitle.Margin = new Padding( 4, 0, 4, 0 );
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size( 625, 38 );
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Powered by TCGdex API";
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            lblVersion.Font = new Font( "Segoe UI", 10F );
            lblVersion.ForeColor = Color.Gray;
            lblVersion.Location = new Point( 62, 283 );
            lblVersion.Margin = new Padding( 4, 0, 4, 0 );
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size( 625, 31 );
            lblVersion.TabIndex = 2;
            lblVersion.Text = "Version 1.0.0";
            lblVersion.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progressBar
            // 
            progressBar.Location = new Point( 125, 350 );
            progressBar.Margin = new Padding( 4, 4, 4, 4 );
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size( 500, 25 );
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 3;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font( "Segoe UI", 9F );
            lblStatus.ForeColor = Color.LightGray;
            lblStatus.Location = new Point( 62, 400 );
            lblStatus.Margin = new Padding( 4, 0, 4, 0 );
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size( 625, 31 );
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Loading...";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.Font = new Font( "Segoe UI", 14F );
            label1.ForeColor = Color.LightGray;
            label1.Location = new Point( 62, 87 );
            label1.Margin = new Padding( 4, 0, 4, 0 );
            label1.Name = "label1";
            label1.Size = new Size( 625, 38 );
            label1.TabIndex = 5;
            label1.Text = "Beresford Embedded Software Limited";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Click +=  label1_Click ;
            // 
            // SplashScreen
            // 
            AutoScaleDimensions = new SizeF( 10F, 25F );
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb( 20, 20, 30 );
            ClientSize = new Size( 750, 500 );
            Controls.Add( label1 );
            Controls.Add( lblTitle );
            Controls.Add( lblSubtitle );
            Controls.Add( lblVersion );
            Controls.Add( progressBar );
            Controls.Add( lblStatus );
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding( 4, 4, 4, 4 );
            Name = "SplashScreen";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout( false );
        }

        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblVersion;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label label1;
    }
}
