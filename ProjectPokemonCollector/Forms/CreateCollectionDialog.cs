namespace ProjectMTGCollector.Forms;

public partial class CreateCollectionDialog : Form
{
    public string CollectionName => txtName.Text.Trim();
    public string CollectionDescription => txtDescription.Text.Trim();

    private TextBox txtName;
    private TextBox txtDescription;
    private Label lblName;
    private Label lblDescription;
    private Button btnCreate;
    private Button btnCancel;

    public CreateCollectionDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.lblName = new Label();
        this.txtName = new TextBox();
        this.lblDescription = new Label();
        this.txtDescription = new TextBox();
        this.btnCreate = new Button();
        this.btnCancel = new Button();
        
        this.SuspendLayout();
        
        // lblName
        this.lblName.AutoSize = true;
        this.lblName.Location = new Point(20, 20);
        this.lblName.Text = "Collection Name:";
        this.lblName.Font = new Font("Segoe UI", 10F);
        
        // txtName
        this.txtName.Location = new Point(20, 45);
        this.txtName.Size = new Size(360, 27);
        this.txtName.Font = new Font("Segoe UI", 10F);
        
        // lblDescription
        this.lblDescription.AutoSize = true;
        this.lblDescription.Location = new Point(20, 85);
        this.lblDescription.Text = "Description:";
        this.lblDescription.Font = new Font("Segoe UI", 10F);
        
        // txtDescription
        this.txtDescription.Location = new Point(20, 110);
        this.txtDescription.Size = new Size(360, 80);
        this.txtDescription.Multiline = true;
        this.txtDescription.Font = new Font("Segoe UI", 10F);
        
        // btnCreate
        this.btnCreate.Location = new Point(180, 210);
        this.btnCreate.Size = new Size(100, 35);
        this.btnCreate.Text = "Create";
        this.btnCreate.Font = new Font("Segoe UI", 10F);
        this.btnCreate.Click += BtnCreate_Click;
        
        // btnCancel
        this.btnCancel.Location = new Point(290, 210);
        this.btnCancel.Size = new Size(100, 35);
        this.btnCancel.Text = "Cancel";
        this.btnCancel.Font = new Font("Segoe UI", 10F);
        this.btnCancel.DialogResult = DialogResult.Cancel;
        
        // CreateCollectionDialog
        this.AutoScaleDimensions = new SizeF(8F, 20F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(410, 265);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Create New Collection";
        
        this.Controls.Add(this.lblName);
        this.Controls.Add(this.txtName);
        this.Controls.Add(this.lblDescription);
        this.Controls.Add(this.txtDescription);
        this.Controls.Add(this.btnCreate);
        this.Controls.Add(this.btnCancel);
        
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void BtnCreate_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {
            MessageBox.Show("Please enter a collection name.", "Validation Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtName.Focus();
            return;
        }

        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
