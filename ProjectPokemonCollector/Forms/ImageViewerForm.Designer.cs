namespace ProjectMTGCollector.Forms
{
    partial class ImageViewerForm
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
            this.picLargeImage = new PictureBox();
            
            ((System.ComponentModel.ISupportInitialize)(this.picLargeImage)).BeginInit();
            this.SuspendLayout();
            
            // picLargeImage
            this.picLargeImage.Dock = DockStyle.Fill;
            this.picLargeImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.picLargeImage.BackColor = Color.Black;
            this.picLargeImage.Cursor = Cursors.Hand;
            this.picLargeImage.DoubleClick += PicLargeImage_DoubleClick;
            
            // ImageViewerForm
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 1000);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Card Image Viewer";
            this.BackColor = Color.Black;
            this.KeyPreview = true;
            this.KeyDown += ImageViewerForm_KeyDown;
            
            this.Controls.Add(this.picLargeImage);
            
            ((System.ComponentModel.ISupportInitialize)(this.picLargeImage)).EndInit();
            this.ResumeLayout(false);
        }

        private PictureBox picLargeImage;
    }
}
