using System.Diagnostics;
using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Forms;

public partial class MainForm
{
    private ComboBox _collectionViewMode = null!;
    private ComboBox _collectionSortBy = null!;
    private ComboBox _collectionSortOrder = null!;
    private ImageList _collectionThumbnails = null!;
    private int _collectionThumbnailRequest;
    private bool _collectionThumbnailsClosed;

    private void InitializeCollectionThumbnails()
    {
        _collectionViewMode = new ComboBox
        {
            Location = new Point(640, 370),
            Size = new Size(120, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            AccessibleName = "Collection card view"
        };
        _collectionViewMode.Items.AddRange(new object[] { "List", "Thumbnails" });
        _collectionViewMode.SelectedIndex = 0;
        _collectionViewMode.SelectedIndexChanged += CollectionViewMode_SelectedIndexChanged;
        tabCollections.Controls.Add(new Label
        {
            Text = "Card view:",
            Location = new Point(640, 345),
            AutoSize = true
        });
        tabCollections.Controls.Add(_collectionViewMode);
        InitializeCollectionSorting();
        _collectionThumbnails = new ImageList
        {
            ImageSize = new Size(140, 196),
            ColorDepth = ColorDepth.Depth32Bit
        };
        listViewCollectionCards.LargeImageList = _collectionThumbnails;
        _ = _collectionThumbnails.Handle;
        listViewCollectionCards.ShowItemToolTips = true;
        FormClosing += (_, _) =>
        {
            _collectionThumbnailsClosed = true;
            _collectionThumbnailRequest++;
        };
        Disposed += (_, _) => _collectionThumbnails.Dispose();
    }

    private void InitializeCollectionSorting()
    {
        tabCollections.Controls.Add(new Label
        {
            Text = "Sort by:",
            Location = new Point(640, 405),
            AutoSize = true
        });
        _collectionSortBy = new ComboBox
        {
            Location = new Point(640, 428),
            Size = new Size(120, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            AccessibleName = "Collection sort field"
        };
        _collectionSortBy.Items.AddRange(new object[]
        {
            "Name", "Price (unit)", "Quantity", "Set", "Collector #", "Variant / Foil", "Condition"
        });
        _collectionSortBy.SelectedIndex = 0;
        _collectionSortOrder = new ComboBox
        {
            Location = new Point(640, 462),
            Size = new Size(120, 28),
            DropDownStyle = ComboBoxStyle.DropDownList,
            AccessibleName = "Collection sort direction"
        };
        _collectionSortOrder.Items.AddRange(new object[] { "Ascending", "Descending" });
        _collectionSortOrder.SelectedIndex = 0;
        _collectionSortBy.SelectedIndexChanged += (_, _) => ApplyCollectionSorting();
        _collectionSortOrder.SelectedIndexChanged += (_, _) => ApplyCollectionSorting();
        tabCollections.Controls.Add(_collectionSortBy);
        tabCollections.Controls.Add(_collectionSortOrder);
        listViewCollectionCards.ListViewItemSorter = new CollectionItemComparer(CompareCollectionItems);
    }

    private void ApplyCollectionSorting()
    {
        listViewCollectionCards.Sort();
        if (listViewCollectionCards.SelectedItems.Count > 0)
            listViewCollectionCards.SelectedItems[0].EnsureVisible();
    }

    private int CompareCollectionItems(ListViewItem leftItem, ListViewItem rightItem)
    {
        var left = (CollectionCard)leftItem.Tag!;
        var right = (CollectionCard)rightItem.Tag!;
        var direction = _collectionSortOrder.SelectedIndex == 1 ? -1 : 1;
        int result;
        if (_collectionSortBy.SelectedIndex == 1)
        {
            // Never compare amounts from different currencies as though they were equivalent.
            if (left.CurrentPrice.HasValue != right.CurrentPrice.HasValue)
                return left.CurrentPrice.HasValue ? -1 : 1;
            var leftPrice = left.CurrentPrice.HasValue
                ? _gbpPrices.ConvertToGbp(left.CurrentPrice.Value, left.PriceCurrency) : null;
            var rightPrice = right.CurrentPrice.HasValue
                ? _gbpPrices.ConvertToGbp(right.CurrentPrice.Value, right.PriceCurrency) : null;
            var leftCurrency = leftPrice.HasValue ? "GBP" : left.PriceCurrency ?? "(currency unknown)";
            var rightCurrency = rightPrice.HasValue ? "GBP" : right.PriceCurrency ?? "(currency unknown)";
            if (left.CurrentPrice.HasValue)
            {
                result = StringComparer.OrdinalIgnoreCase.Compare(leftCurrency, rightCurrency);
                if (result != 0)
                    return result;
            }
            result = Nullable.Compare(leftPrice ?? left.CurrentPrice, rightPrice ?? right.CurrentPrice);
        }
        else
        {
            result = _collectionSortBy.SelectedIndex switch
            {
                2 => left.Quantity.CompareTo(right.Quantity),
                3 => StringComparer.CurrentCultureIgnoreCase.Compare(left.SetName, right.SetName),
                4 => CompareCollectionCollectorNumbers(left.CollectorNumber, right.CollectorNumber),
                5 => StringComparer.CurrentCultureIgnoreCase.Compare(leftItem.SubItems[4].Text, rightItem.SubItems[4].Text),
                6 => StringComparer.CurrentCultureIgnoreCase.Compare(left.Condition, right.Condition),
                _ => StringComparer.CurrentCultureIgnoreCase.Compare(left.CardName, right.CardName)
            };
        }
        if (result == 0)
            result = StringComparer.CurrentCultureIgnoreCase.Compare(left.CardName, right.CardName);
        if (result == 0)
            result = StringComparer.OrdinalIgnoreCase.Compare(left.SetCode, right.SetCode);
        if (result == 0)
            result = CompareCollectionCollectorNumbers(left.CollectorNumber, right.CollectorNumber);
        return Math.Sign(result) * direction;
    }

    private int CompareCollectionCollectorNumbers(string left, string right)
    {
        var result = GetNumericCollectorNumber(left).CompareTo(GetNumericCollectorNumber(right));
        return result != 0 ? result : StringComparer.OrdinalIgnoreCase.Compare(left, right);
    }

    private sealed class CollectionItemComparer(Func<ListViewItem, ListViewItem, int> compare) : System.Collections.IComparer
    {
        public int Compare(object? left, object? right)
        {
            if (left is not ListViewItem leftItem || right is not ListViewItem rightItem)
                throw new ArgumentException("Collection sorting requires card list items.");
            return compare(leftItem, rightItem);
        }
    }

    private async void CollectionViewMode_SelectedIndexChanged(object? sender, EventArgs e)
    {
        await RefreshCollectionThumbnailsAsync();
    }

    private async Task RefreshCollectionThumbnailsAsync()
    {
        ApplyCollectionSorting();
        var request = ++_collectionThumbnailRequest;
        var thumbnails = _collectionViewMode.SelectedIndex == 1;
        var items = listViewCollectionCards.Items.Cast<ListViewItem>().ToArray();
        listViewCollectionCards.BeginUpdate();
        try
        {
            listViewCollectionCards.View = thumbnails ? View.LargeIcon : View.Details;
            _collectionThumbnails.Images.Clear();
            if (thumbnails)
                _collectionThumbnails.Images.Add(_imageCacheService.GetPlaceholderImage());
            foreach (var item in items)
            {
                var card = (CollectionCard)item.Tag!;
                var details = $"{card.Quantity}x | {listViewCollectionCards.Columns[4].Text}: {item.SubItems[4].Text} | {item.SubItems[6].Text}";
                item.Text = thumbnails ? $"{card.CardName}\n{details}" : card.CardName;
                item.ToolTipText = $"{card.CardName}\n{item.SubItems[1].Text} #{card.CollectorNumber}\n{details}";
                item.ImageIndex = thumbnails ? 0 : -1;
            }
        }
        finally
        {
            listViewCollectionCards.EndUpdate();
        }
        if (!thumbnails)
            return;

        // A small worker pool avoids queuing a download for every card in a large folder.
        var next = 0;
        async Task LoadImagesAsync()
        {
            while (!_collectionThumbnailsClosed && !IsDisposed && request == _collectionThumbnailRequest && next < items.Length)
            {
                var item = items[next++];
                if (item.ListView != listViewCollectionCards)
                    continue;
                var card = (CollectionCard)item.Tag!;
                Image? image = null;
                if (!string.IsNullOrWhiteSpace(card.ImageUrl))
                {
                    try
                    {
                        image = await _imageCacheService.GetImageAsync(card.ImageUrl);
                    }
                    catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or ArgumentException or IOException)
                    {
                        Trace.TraceWarning($"Collection image failed for '{card.CardName}': {ex.Message}");
                    }
                }
                if (_collectionThumbnailsClosed || IsDisposed || request != _collectionThumbnailRequest)
                    return;
                if (item.ListView != listViewCollectionCards)
                    continue;
                if (image == null)
                {
                    item.ToolTipText += "\nImage unavailable";
                    Trace.TraceWarning($"Collection image unavailable for '{card.CardName}'.");
                    continue;
                }
                using var thumbnail = CreateCollectionThumbnail(image);
                var index = _collectionThumbnails.Images.Count;
                _collectionThumbnails.Images.Add(thumbnail);
                item.ImageIndex = index;
                await Task.Yield();
            }
        }
        try
        {
            await Task.WhenAll(LoadImagesAsync(), LoadImagesAsync(), LoadImagesAsync());
        }
        catch (Exception ex)
        {
            if (!_collectionThumbnailsClosed && !IsDisposed)
                MessageBox.Show(this, $"Error loading collection thumbnails: {ex.Message}", "Thumbnail Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Bitmap CreateCollectionThumbnail(Image image)
    {
        var size = _collectionThumbnails.ImageSize;
        var bitmap = new Bitmap(size.Width, size.Height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(listViewCollectionCards.BackColor);
        var scale = Math.Min((float)size.Width / image.Width, (float)size.Height / image.Height);
        var width = (int)(image.Width * scale);
        var height = (int)(image.Height * scale);
        graphics.DrawImage(image, (size.Width - width) / 2, (size.Height - height) / 2, width, height);
        return bitmap;
    }
}
