using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Forms
{
    partial class AddToCollectionDialog
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
            this.lblCardName = new Label();
            this.lblSelectCollection = new Label();
            this.treeViewCollections = new TreeView();
            this.lblQuantity = new Label();
            this.numQuantity = new NumericUpDown();
            this.cmbVariant = new ComboBox();
            this.lblVariant = new Label();
            this.lblVariantPrice = new Label();
            this.lblCondition = new Label();
            this.cmbCondition = new ComboBox();
            this.lblNotes = new Label();
            this.txtNotes = new TextBox();
            this.btnAdd = new Button();
            this.btnCancel = new Button();
            this.btnNewCollection = new Button();
            this.btnNewFolder = new Button();
            
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            this.SuspendLayout();
            
            // lblCardName
            this.lblCardName.AutoSize = false;
            this.lblCardName.Location = new Point(20, 20);
            this.lblCardName.Size = new Size(560, 30);
            this.lblCardName.Text = "Add Card: ";
            this.lblCardName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            
            // lblSelectCollection
            this.lblSelectCollection.AutoSize = true;
            this.lblSelectCollection.Location = new Point(20, 60);
            this.lblSelectCollection.Text = "Select Collection and Folder:";
            this.lblSelectCollection.Font = new Font("Segoe UI", 10F);
            
            // treeViewCollections
            this.treeViewCollections.Location = new Point(20, 90);
            this.treeViewCollections.Size = new Size(560, 200);
            this.treeViewCollections.Font = new Font("Segoe UI", 9F);
            this.treeViewCollections.HideSelection = false;
            
            // btnNewCollection
            this.btnNewCollection.Location = new Point(20, 300);
            this.btnNewCollection.Size = new Size(140, 30);
            this.btnNewCollection.Text = "New Collection";
            this.btnNewCollection.Font = new Font("Segoe UI", 9F);
            this.btnNewCollection.Click += BtnNewCollection_Click;
            
            // btnNewFolder
            this.btnNewFolder.Location = new Point(170, 300);
            this.btnNewFolder.Size = new Size(120, 30);
            this.btnNewFolder.Text = "New Folder";
            this.btnNewFolder.Font = new Font("Segoe UI", 9F);
            this.btnNewFolder.Click += BtnNewFolder_Click;
            
            // lblQuantity
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new Point(20, 350);
            this.lblQuantity.Text = "Quantity:";
            this.lblQuantity.Font = new Font("Segoe UI", 9F);
            
            // numQuantity
            this.numQuantity.Location = new Point(100, 347);
            this.numQuantity.Size = new Size(80, 27);
            this.numQuantity.Minimum = 1;
            this.numQuantity.Maximum = 999;
            this.numQuantity.Value = 1;
            this.numQuantity.Font = new Font("Segoe UI", 9F);
            
            this.lblVariant.Location = new Point(20, 390);
            this.lblVariant.Size = new Size(80, 25);
            this.lblVariant.Text = "Variant:";
            this.cmbVariant.Location = new Point(100, 387);
            this.cmbVariant.Size = new Size(480, 28);
            this.cmbVariant.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbVariant.DropDownWidth = 800;
            this.cmbVariant.DisplayMember = nameof(CardVariant.Description);
            this.cmbVariant.SelectedIndexChanged += CmbVariant_SelectedIndexChanged;
            this.lblVariantPrice.Location = new Point(100, 420);
            this.lblVariantPrice.Size = new Size(480, 30);
            
            // lblCondition
            this.lblCondition.AutoSize = true;
            this.lblCondition.Location = new Point(300, 350);
            this.lblCondition.Text = "Condition:";
            this.lblCondition.Font = new Font("Segoe UI", 9F);
            
            // cmbCondition
            this.cmbCondition.Location = new Point(380, 347);
            this.cmbCondition.Size = new Size(200, 28);
            this.cmbCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCondition.Font = new Font("Segoe UI", 9F);
            this.cmbCondition.Items.AddRange(new object[] {
                "Mint",
                "Near Mint",
                "Excellent",
                "Good",
                "Light Played",
                "Played",
                "Poor"
            });
            this.cmbCondition.SelectedIndex = 1; // Near Mint
            
            // lblNotes
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new Point(20, 460);
            this.lblNotes.Text = "Notes (optional):";
            this.lblNotes.Font = new Font("Segoe UI", 9F);
            
            // txtNotes
            this.txtNotes.Location = new Point(20, 485);
            this.txtNotes.Size = new Size(560, 60);
            this.txtNotes.Multiline = true;
            this.txtNotes.Font = new Font("Segoe UI", 9F);
            
            // btnAdd
            this.btnAdd.Location = new Point(380, 560);
            this.btnAdd.Size = new Size(100, 35);
            this.btnAdd.Text = "Add";
            this.btnAdd.Font = new Font("Segoe UI", 10F);
            this.btnAdd.Click += BtnAdd_Click;
            
            // btnCancel
            this.btnCancel.Location = new Point(490, 560);
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Font = new Font("Segoe UI", 10F);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            
            // AddToCollectionDialog
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(610, 615);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Add to Collection";
            
            this.Controls.Add(this.lblCardName);
            this.Controls.Add(this.lblSelectCollection);
            this.Controls.Add(this.treeViewCollections);
            this.Controls.Add(this.btnNewCollection);
            this.Controls.Add(this.btnNewFolder);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.lblVariant);
            this.Controls.Add(this.cmbVariant);
            this.Controls.Add(this.lblVariantPrice);
            this.Controls.Add(this.lblCondition);
            this.Controls.Add(this.cmbCondition);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblCardName;
        private Label lblSelectCollection;
        private TreeView treeViewCollections;
        private Label lblQuantity;
        private NumericUpDown numQuantity;
        private ComboBox cmbVariant;
        private Label lblVariant;
        private Label lblVariantPrice;
        private Label lblCondition;
        private ComboBox cmbCondition;
        private Label lblNotes;
        private TextBox txtNotes;
        private Button btnAdd;
        private Button btnCancel;
        private Button btnNewCollection;
        private Button btnNewFolder;
    }
}
