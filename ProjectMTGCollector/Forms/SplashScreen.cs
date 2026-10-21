namespace ProjectMTGCollector.Forms;

public partial class SplashScreen : Form
{
    public SplashScreen()
    {
        InitializeComponent();
    }

    public void UpdateStatus( string status )
    {
        if ( lblStatus.InvokeRequired )
        {
            lblStatus.Invoke( new Action( () => lblStatus.Text = status ) );
        }
        else
        {
            lblStatus.Text = status;
        }
    }

    private void label1_Click( object sender, EventArgs e )
    {

    }
}
