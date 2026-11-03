using ProjectMTGCollector.Models;
using ProjectMTGCollector.Services;

namespace ProjectMTGCollector.Forms;

// This partial class contains collection management methods for MainForm
public partial class MainForm
{
    private Collection? _currentCollection;
    private CollectionFolder? _currentFolder;

    // Add to Collection button handlers
    private void BtnAddToCollection_Click(object? sender, EventArgs e)
    {
        if (_currentCard != null)
        {
            ShowAddToCollectionDialog(_currentCard);
            btnAddToCollection.Enabled = true;
        }
    }

    private void BtnAddSelectedToCollection_Click(object? sender, EventArgs e)
    {
        if (_currentSelectedSetCard != null)
        {
            ShowAddToCollectionDialog(_currentSelectedSetCard);
            btnAddSelectedToCollection.Enabled = true;
        }
    }

    // Collections Tab Event Handlers
    private void ListViewCollections_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (listViewCollections.SelectedItems.Count > 0)
        {
            var collection = (Collection)listViewCollections.SelectedItems[0].Tag;
            _currentCollection = collection;
            DisplayCollectionDetails(collection);
            btnEditCollection.Enabled = true;
            btnDeleteCollection.Enabled = true;
            btnNewCollectionFolder.Enabled = true;
        }
        else
        {
            _currentCollection = null;
            _currentFolder = null;
            treeViewCollectionFolders.Nodes.Clear();
            listViewCollectionCards.Items.Clear();
            btnEditCollection.Enabled = false;
            btnDeleteCollection.Enabled = false;
            btnNewCollectionFolder.Enabled = false;
            btnEditFolder.Enabled = false;
            btnDeleteFolder.Enabled = false;
            btnRemoveCard.Enabled = false;
        }
    }

    private void TreeViewCollectionFolders_AfterSelect(object? sender, TreeViewEventArgs e)
    {
        if (e.Node?.Tag is CollectionFolder folder)
        {
            _currentFolder = folder;
            DisplayFolderCards(folder);
            btnEditFolder.Enabled = true;
            btnDeleteFolder.Enabled = true;
        }
        else
        {
            _currentFolder = null;
            listViewCollectionCards.Items.Clear();
            btnEditFolder.Enabled = false;
            btnDeleteFolder.Enabled = false;
        }

        btnRemoveCard.Enabled = false;
    }

    private void ListViewCollectionCards_SelectedIndexChanged(object? sender, EventArgs e)
    {
        btnRemoveCard.Enabled = _currentCollection != null && _currentFolder != null &&
            listViewCollectionCards.SelectedItems.Count > 0;
    }

    private async void BtnNewCollection_Click(object? sender, EventArgs e)
    {
        using var dialog = new CreateCollectionDialog();
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                await _collectionService.CreateCollectionAsync(
                    dialog.CollectionName,
                    dialog.CollectionDescription
                );
                
                RefreshCollectionsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating collection: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnEditCollection_Click(object? sender, EventArgs e)
    {
        if (_currentCollection == null)
            return;

        using var dialog = new CreateCollectionDialog(_currentCollection.Name, _currentCollection.Description);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                _currentCollection.Name = dialog.CollectionName;
                _currentCollection.Description = dialog.CollectionDescription;
                await _collectionService.UpdateCollectionAsync(_currentCollection);
                
                RefreshCollectionsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating collection: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnDeleteCollection_Click(object? sender, EventArgs e)
    {
        if (_currentCollection == null)
            return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{_currentCollection.Name}'? This will remove all folders and cards in this collection.",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            try
            {
                await _collectionService.DeleteCollectionAsync(_currentCollection.Id);
                RefreshCollectionsTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting collection: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnNewCollectionFolder_Click(object? sender, EventArgs e)
    {
        if (_currentCollection == null)
            return;

        using var dialog = new CreateFolderDialog();
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                await _collectionService.AddFolderAsync(
                    _currentCollection.Id,
                    dialog.FolderName,
                    dialog.FolderDescription
                );
                
                LoadCollectionFolders(_currentCollection);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating folder: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnEditFolder_Click(object? sender, EventArgs e)
    {
        if (_currentCollection == null || _currentFolder == null)
            return;

        var collection = _currentCollection;
        var folder = _currentFolder;
        using var dialog = new CreateFolderDialog(folder.Name, folder.Description);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                folder.Name = dialog.FolderName;
                folder.Description = dialog.FolderDescription;
                await _collectionService.UpdateCollectionAsync(collection);
                LoadCollectionFolders(collection);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating folder: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnDeleteFolder_Click(object? sender, EventArgs e)
    {
        if (_currentCollection == null || _currentFolder == null)
            return;

        var result = MessageBox.Show(
            $"Are you sure you want to delete folder '{_currentFolder.Name}'? This will remove all cards in this folder.",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result == DialogResult.Yes)
        {
            try
            {
                await _collectionService.DeleteFolderAsync(_currentCollection.Id, _currentFolder.Id);
                LoadCollectionFolders(_currentCollection);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting folder: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void BtnRemoveCard_Click(object? sender, EventArgs e)
    {
        if (_currentCollection == null || _currentFolder == null || listViewCollectionCards.SelectedItems.Count == 0)
            return;

        var selectedCard = (CollectionCard)listViewCollectionCards.SelectedItems[0].Tag;

        var result = MessageBox.Show(
            $"Remove {selectedCard.Quantity}x {selectedCard.CardName} ({selectedCard.VariantDescription}) from this folder?",
            "Confirm Remove",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            try
            {
                await _collectionService.RemoveCardFromFolderAsync(
                    _currentCollection.Id,
                    _currentFolder.Id,
                    selectedCard.CardId,
                    selectedCard.IsFoil,
                    selectedCard.Variant?.Id
                );
                
                DisplayFolderCards(_currentFolder);
                DisplayCollectionDetails(_currentCollection);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing card: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // Display methods for Collections Tab
    private void RefreshCollectionsTab()
    {
        var collectionId = _currentCollection?.Id;
        var folderId = _currentFolder?.Id;
        listViewCollections.Items.Clear();
        var collections = _collectionService.GetAllCollections();
        ListViewItem? selectedItem = null;

        foreach (var collection in collections)
        {
            var item = new ListViewItem(collection.Name);
            item.SubItems.Add(collection.GetTotalCardCount().ToString());
            item.Tag = collection;
            listViewCollections.Items.Add(item);
            if (collection.Id == collectionId)
            {
                selectedItem = item;
                _currentFolder = collection.Folders.FirstOrDefault(f => f.Id == folderId);
            }
        }

        if (selectedItem != null)
        {
            selectedItem.Selected = true;
        }
        else
        {
            _currentCollection = null;
            _currentFolder = null;
            treeViewCollectionFolders.Nodes.Clear();
            listViewCollectionCards.Items.Clear();
            btnEditCollection.Enabled = false;
            btnDeleteCollection.Enabled = false;
            btnNewCollectionFolder.Enabled = false;
            btnEditFolder.Enabled = false;
            btnDeleteFolder.Enabled = false;
            btnRemoveCard.Enabled = false;
            lblCollectionName.Text = "Select a collection";
            lblCollectionDescription.Text = "";
            lblCollectionStats.Text = "";
        }
    }

    private void DisplayCollectionDetails(Collection collection)
    {
        lblCollectionName.Text = collection.Name;
        lblCollectionDescription.Text = collection.Description;
        
        UpdateCollectionValue(collection);
        LoadCollectionFolders(collection);
    }

    private void UpdateCollectionValue(Collection collection)
    {
        var totalCards = collection.GetTotalCardCount();
        var cards = collection.Folders.SelectMany(f => f.Cards);
        lblCollectionStats.Text = $"{totalCards} cards | {_gbpPrices.FormatCollectionTotal(cards)}";
    }

    private void LoadCollectionFolders(Collection collection)
    {
        var folderId = _currentFolder?.Id;
        treeViewCollectionFolders.Nodes.Clear();
        _currentFolder = null;
        listViewCollectionCards.Items.Clear();
        btnEditFolder.Enabled = false;
        btnDeleteFolder.Enabled = false;
        btnRemoveCard.Enabled = false;

        var rootNode = new TreeNode(collection.Name)
        {
            Tag = collection,
            NodeFont = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        TreeNode? selectedNode = null;

        foreach (var folder in collection.Folders)
        {
            var folderNode = new TreeNode($"{folder.Name} ({folder.GetTotalCardCount()} cards)")
            {
                Tag = folder
            };
            rootNode.Nodes.Add(folderNode);
            if (folder.Id == folderId)
                selectedNode = folderNode;
        }

        treeViewCollectionFolders.Nodes.Add(rootNode);
        rootNode.Expand();
        treeViewCollectionFolders.SelectedNode = selectedNode ?? rootNode;
    }

    private void DisplayFolderCards(CollectionFolder folder)
    {
        listViewCollectionCards.Items.Clear();

        foreach (var card in folder.Cards)
        {
            var item = new ListViewItem(card.CardName);
            item.SubItems.Add($"{card.SetName} ({card.SetCode.ToUpper()})");
            item.SubItems.Add(card.CollectorNumber);
            item.SubItems.Add(card.Quantity.ToString());
            item.SubItems.Add(card.VariantDescription);
            item.SubItems.Add(card.Condition);
            item.SubItems.Add(_gbpPrices.FormatCollectionPrice(card));
            item.Tag = card;
            
            listViewCollectionCards.Items.Add(item);
        }

        btnRemoveCard.Enabled = listViewCollectionCards.SelectedItems.Count > 0;

        // Attach double-click handler to load card into main view
        listViewCollectionCards.DoubleClick -= ListViewCollectionCards_DoubleClick;
        listViewCollectionCards.DoubleClick += ListViewCollectionCards_DoubleClick;
        _ = RefreshCollectionThumbnailsAsync();
    }

    private async void ListViewCollectionCards_DoubleClick(object? sender, EventArgs e)
    {
        if (listViewCollectionCards.SelectedItems.Count == 0)
            return;

        var collCard = (CollectionCard)listViewCollectionCards.SelectedItems[0].Tag;
        if (collCard == null)
            return;

        // Look up the exact Pokémon card using its set and collector number.
        try
        {
            var setCode = collCard.SetCode;
            var collector = collCard.CollectorNumber;

            var card = await _pokemonCardService.GetCardBySetAndCollectorAsync(setCode, collector);
            if (card != null)
            {
                if (collCard.Variant != null)
                {
                    var variant = card.Variants.FirstOrDefault(v => v.Id == collCard.Variant.Id);
                    if (variant == null)
                    {
                        MessageBox.Show(this, "The saved variant is no longer listed by TCGdex. Showing the card without selecting a different variant.",
                            "Variant unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        card = card.WithVariant(variant);
                }
                _currentCard = card;
                _currentCardVariants = new List<Card> { card };
                DisplayVariants(_currentCardVariants);
                await DisplayCard(card);
                tabControl.SelectedTab = tabSearchCard;
            }
            else
            {
                MessageBox.Show("Unable to load Pokémon card from TCGdex.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading card: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
