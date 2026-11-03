namespace ProjectMTGCollector.Forms;

public partial class MainForm
{
    private void InitializeCollectionFileControls()
    {
        var exportButton = new Button
        {
            Text = "Export All...",
            Location = new Point(740, 20),
            Size = new Size(120, 35),
            Font = btnDeleteCollection.Font
        };
        var importButton = new Button
        {
            Text = "Import...",
            Location = new Point(870, 20),
            Size = new Size(120, 35),
            Font = btnDeleteCollection.Font
        };
        exportButton.Click += BtnExportCollections_Click;
        importButton.Click += BtnImportCollections_Click;
        tabCollections.Controls.Add(exportButton);
        tabCollections.Controls.Add(importButton);

        listViewCollectionCards.Scrollable = true;
        tabCollections.AutoScroll = true;
        tabCollections.AutoScrollMinSize = new Size(1100, 500);
        foreach (var control in new Control[] { listViewCollections, treeViewCollectionFolders,
            panelCollectionDetails, listViewCollectionCards })
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        tabCollections.Resize += TabCollections_Resize;
        TabCollections_Resize(this, EventArgs.Empty);
        InitializeCollectionThumbnails();
    }

    private void TabCollections_Resize(object? sender, EventArgs e)
    {
        var width = Math.Max(1100, tabCollections.ClientSize.Width);
        var height = Math.Max(500, tabCollections.ClientSize.Height);
        listViewCollections.Height = height - 40;
        treeViewCollectionFolders.Height = height - 200;
        panelCollectionDetails.Width = width - 350;
        listViewCollectionCards.Size = new Size(width - 790, height - 200);
    }

    private async void BtnExportCollections_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = "Export all collections",
            Filter = "Collection export (*.json)|*.json",
            DefaultExt = "json",
            FileName = "collection-export.json",
            AddExtension = true,
            OverwritePrompt = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        tabCollections.Enabled = false;
        try
        {
            await _collectionService.ExportCollectionsAsync(dialog.FileName);
            MessageBox.Show(this, "All collections were exported successfully.", "Export Collections",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Error exporting collections: {ex.Message}", "Export Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            tabCollections.Enabled = true;
        }
    }

    private async void BtnImportCollections_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Import collections",
            Filter = "Collection export (*.json)|*.json",
            CheckFileExists = true,
            Multiselect = false
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;
        if (MessageBox.Show(this,
            "Import collections as new copies? Existing collections will not be replaced. Importing the same file again will add duplicate collections.",
            "Confirm Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        var pollingEnabled = _collectionsPollTimer.Enabled;
        _collectionsPollTimer.Stop();
        tabCollections.Enabled = false;
        try
        {
            var count = await _collectionService.ImportCollectionsAsync(dialog.FileName);
            RefreshCollectionsTab();
            MessageBox.Show(this, $"Imported {count} collection(s). Existing collections were preserved.",
                "Import Collections", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Error importing collections: {ex.Message}", "Import Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            tabCollections.Enabled = true;
            if (pollingEnabled)
                _collectionsPollTimer.Start();
        }
    }
}
