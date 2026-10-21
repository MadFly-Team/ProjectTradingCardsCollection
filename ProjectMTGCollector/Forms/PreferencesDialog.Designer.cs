using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Forms
{
    partial class PreferencesDialog
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
            this.lblTheme = new Label();
            this.cmbThemes = new ComboBox();
            this.previewPanel = new Panel();
            this.lblPreview = new Label();
            this.previewButton = new Button();
            this.previewLabel = new Label();
            this.btnApply = new Button();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.lblDescription = new Label();
            
            this.SuspendLayout();
            
            // lblTheme
            this.lblTheme.AutoSize = true;
            this.lblTheme.Location = new Point(30, 30);
            this.lblTheme.Size = new Size(100, 20);
            this.lblTheme.Text = "Color Theme:";
            this.lblTheme.Font = new Font("Segoe UI", 10F);
            
            // cmbThemes
            this.cmbThemes.Location = new Point(140, 27);
            this.cmbThemes.Size = new Size(300, 28);
            this.cmbThemes.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbThemes.Font = new Font("Segoe UI", 10F);
            this.cmbThemes.SelectedIndexChanged += CmbThemes_SelectedIndexChanged;
            
            // lblDescription
            this.lblDescription.Location = new Point(30, 70);
            this.lblDescription.Size = new Size(550, 40);
            this.lblDescription.Font = new Font("Segoe UI", 9F);
            this.lblDescription.ForeColor = Color.Gray;
            this.lblDescription.Text = "Select a color theme to customize the appearance of the application. Changes can be previewed before applying.";
            
            // lblPreview
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new Point(30, 120);
            this.lblPreview.Size = new Size(100, 20);
            this.lblPreview.Text = "Preview:";
            this.lblPreview.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            
            // previewPanel
            this.previewPanel.Location = new Point(30, 150);
            this.previewPanel.Size = new Size(550, 200);
            this.previewPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // previewLabel
            this.previewLabel.Location = new Point(20, 20);
            this.previewLabel.Size = new Size(510, 60);
            this.previewLabel.Text = "This is how text will appear in the application with the selected theme.";
            this.previewLabel.Font = new Font("Segoe UI", 10F);
            
            // previewButton
            this.previewButton.Location = new Point(20, 100);
            this.previewButton.Size = new Size(150, 40);
            this.previewButton.Text = "Sample Button";
            this.previewButton.Font = new Font("Segoe UI", 10F);
            
            this.previewPanel.Controls.Add(this.previewLabel);
            this.previewPanel.Controls.Add(this.previewButton);
            
            // btnApply
            this.btnApply.Location = new Point(280, 380);
            this.btnApply.Size = new Size(100, 35);
            this.btnApply.Text = "Apply";
            this.btnApply.Font = new Font("Segoe UI", 10F);
            this.btnApply.Click += BtnApply_Click;
            
            // btnOK
            this.btnOK.Location = new Point(390, 380);
            this.btnOK.Size = new Size(100, 35);
            this.btnOK.Text = "OK";
            this.btnOK.Font = new Font("Segoe UI", 10F);
            this.btnOK.Click += BtnOK_Click;
            
            // btnCancel
            this.btnCancel.Location = new Point(500, 380);
            this.btnCancel.Size = new Size(100, 35);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Font = new Font("Segoe UI", 10F);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            
            // PreferencesDialog
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(620, 440);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.BackColor = Color.White;
            
            this.Controls.Add(this.lblTheme);
            this.Controls.Add(this.cmbThemes);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.previewPanel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTheme;
        private ComboBox cmbThemes;
        private Label lblDescription;
        private Label lblPreview;
        private Panel previewPanel;
        private Label previewLabel;
        private Button previewButton;
        private Button btnApply;
        private Button btnOK;
        private Button btnCancel;
    }
}
