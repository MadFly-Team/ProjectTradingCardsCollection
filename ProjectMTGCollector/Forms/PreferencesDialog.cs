using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Forms;

public partial class PreferencesDialog : Form
{
    private List<ColorTheme> _themes;
    private ColorTheme? _selectedTheme;
    public ColorTheme? SelectedTheme => _selectedTheme;

    public PreferencesDialog(ColorTheme currentTheme)
    {
        InitializeComponent();
        _themes = ColorTheme.GetAllThemes();
        
        // Populate combo box
        foreach (var theme in _themes)
        {
            cmbThemes.Items.Add(theme.Name);
        }
        
        // Select current theme
        var currentIndex = _themes.FindIndex(t => t.Name == currentTheme.Name);
        if (currentIndex >= 0)
        {
            cmbThemes.SelectedIndex = currentIndex;
        }
        else
        {
            cmbThemes.SelectedIndex = 0;
        }
    }

    private void CmbThemes_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cmbThemes.SelectedIndex >= 0)
        {
            var theme = _themes[cmbThemes.SelectedIndex];
            UpdatePreview(theme);
        }
    }

    private void UpdatePreview(ColorTheme theme)
    {
        // Update preview panel
        previewPanel.BackColor = theme.PanelBackColor;
        previewLabel.ForeColor = theme.ForeColor;
        previewButton.BackColor = theme.ButtonBackColor;
        previewButton.ForeColor = theme.ButtonForeColor;
        
        // Update dialog colors
        this.BackColor = theme.BackColor;
        lblTheme.ForeColor = theme.ForeColor;
        lblDescription.ForeColor = Color.FromArgb(
            Math.Min(255, theme.ForeColor.R + 50),
            Math.Min(255, theme.ForeColor.G + 50),
            Math.Min(255, theme.ForeColor.B + 50)
        );
        lblPreview.ForeColor = theme.ForeColor;
    }

    private void BtnApply_Click(object? sender, EventArgs e)
    {
        if (cmbThemes.SelectedIndex >= 0)
        {
            _selectedTheme = _themes[cmbThemes.SelectedIndex];
            DialogResult = DialogResult.None; // Don't close the dialog
            
            // Notify that theme should be applied
            if (Owner is MainForm mainForm)
            {
                mainForm.ApplyTheme(_selectedTheme);
            }
        }
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        if (cmbThemes.SelectedIndex >= 0)
        {
            _selectedTheme = _themes[cmbThemes.SelectedIndex];
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
