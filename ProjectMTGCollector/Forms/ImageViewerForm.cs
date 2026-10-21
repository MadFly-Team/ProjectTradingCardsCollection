namespace ProjectMTGCollector.Forms;

public partial class ImageViewerForm : Form
{
    public ImageViewerForm(Image image, string cardName)
    {
        InitializeComponent();
        picLargeImage.Image = image;
        this.Text = $"Card Image Viewer - {cardName}";
    }

    private void PicLargeImage_DoubleClick(object? sender, EventArgs e)
    {
        this.Close();
    }

    private void ImageViewerForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
        {
            this.Close();
        }
    }
}
