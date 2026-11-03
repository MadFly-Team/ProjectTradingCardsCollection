using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using ProjectMTGCollector.Services;

namespace ProjectMTGCollector.Forms;

public sealed class GradedPricesControl : UserControl
{
    private static readonly Dictionary<string, (PriceChartingProduct Product, DateTime Loaded)> Cache = new();
    private static event Action? PricesChanged;
    private readonly PriceChartingService _service = new();
    private readonly Button _load = new() { Text = "Load / change graded values...", Dock = DockStyle.Top, Height = 30, Enabled = false };
    private readonly ListView _table = new() { Dock = DockStyle.Fill, View = View.Details, GridLines = true, FullRowSelect = true };
    private readonly LinkLabel _source = new() { Dock = DockStyle.Bottom, Height = 52, AutoEllipsis = true };
    private GbpPriceService? _gbp;
    private string? _key;
    private string _query = "";
    private string _cardDescription = "";
    private bool _loading;

    public GradedPricesControl()
    {
        Height = 245;
        MinimumSize = new Size(320, 245);
        _table.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        _table.Columns.Add("Grade", 55);
        _table.Columns.Add("General (all graders)", 205);
        _table.Columns.Add("PSA", 190);
        _table.Columns.Add("CGC", 190);
        _table.Columns.Add("TAG", 190);
        Controls.Add(_table);
        Controls.Add(_source);
        Controls.Add(_load);
        _load.Click += LoadPrices_Click;
        _source.LinkClicked += Source_LinkClicked;
        PricesChanged += RefreshPrices;
        RefreshPrices();
    }

    public void SetCard(string? key, string name, string set, string number, string game, GbpPriceService gbp)
    {
        _gbp = gbp;
        _key = key == null ? null : $"{game}:{key}";
        _query = $"{game} {set} {name} #{number}";
        _cardDescription = $"{name} - {set} #{number}";
        _load.Enabled = _key != null && !_loading;
        RefreshPrices();
    }

    public void ApplyColors(Color background, Color foreground)
    {
        BackColor = background;
        ForeColor = foreground;
        _table.BackColor = background;
        _table.ForeColor = foreground;
        _source.LinkColor = foreground;
        _source.ActiveLinkColor = foreground;
        _source.VisitedLinkColor = foreground;
        _load.BackColor = background;
        _load.ForeColor = foreground;
    }

    private void RefreshPrices()
    {
        (PriceChartingProduct Product, DateTime Loaded)? result =
            _key != null && Cache.TryGetValue(_key, out var cached) ? cached : null;
        string Format(string? field)
        {
            if (field == null || result == null)
                return "N/A";
            var price = result.Value.Product.GetPrice(field);
            return price.HasValue
                ? $"${price.Value.ToString("F2", CultureInfo.InvariantCulture)} / {_gbp?.FormatEstimate(price.Value, "USD") ?? "GBP unavailable"}"
                : "N/A";
        }

        _table.BeginUpdate();
        _table.Items.Clear();
        for (int grade = 6; grade <= 10; grade++)
        {
            var generalField = grade switch {
                6 => "condition-16-price", 7 => "cib-price", 8 => "new-price", 9 => "graded-price", _ => null
            };
            var gradeLabel = grade is 7 or 8 ? $"{grade}/{grade}.5" : grade.ToString();
            _table.Items.Add(new ListViewItem(new[] {
                gradeLabel, Format(generalField), Format(grade == 10 ? "manual-only-price" : null),
                Format(grade == 10 ? "condition-17-price" : null), Format(grade == 10 ? "condition-21-price" : null)
            }));
        }
        _table.EndUpdate();
        _source.Links.Clear();
        if (result.HasValue)
        {
            var product = result.Value.Product;
            _source.Text = $"PriceCharting: {product.Set} - {product.Name}\nLoaded {result.Value.Loaded:g}. USD / estimated GBP. N/A = unavailable; not inferred.";
            _source.Links.Add(0, _source.Text.IndexOf('\n'), $"https://www.pricecharting.com/game/{Uri.EscapeDataString(product.Id)}");
        }
        else
        {
            _source.Text = _key == null ? "Select a card to load graded values."
                : "Not loaded. Confirm the exact printing with Load / change graded values.\nN/A = unavailable; grades 7/8 include 7.5/8.5. General values are not PSA-specific.";
        }
    }

    private async void LoadPrices_Click(object? sender, EventArgs e)
    {
        var key = _key;
        if (key == null)
            return;
        _loading = true;
        _load.Enabled = false;
        try
        {
            using var picker = new ProductPicker(_service, _query, _cardDescription);
            if (picker.ShowDialog(this) != DialogResult.OK || picker.SelectedProduct == null)
                return;
            var product = await _service.GetProductAsync(picker.SelectedProduct.Id);
            if (IsDisposed || key != _key)
                return;
            Cache[key] = (product, DateTime.Now);
            PricesChanged?.Invoke();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
        {
            if (!IsDisposed)
                MessageBox.Show(this, ex is InvalidOperationException ? ex.Message :
                    "Could not load graded values from PriceCharting. Check your API token/subscription, connection, and the selected product, then retry.",
                    "Graded prices", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _loading = false;
            if (!IsDisposed)
                _load.Enabled = _key != null;
        }
    }

    private void Source_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (e.Link?.LinkData is not string url)
            return;
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
        }
        catch (System.ComponentModel.Win32Exception)
        {
            MessageBox.Show(this, "Could not open the PriceCharting page. Check your default browser settings.",
                "Graded prices", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            PricesChanged -= RefreshPrices;
        base.Dispose(disposing);
    }

    private sealed class ProductPicker : Form
    {
        private readonly TextBox _query = new() { Dock = DockStyle.Top };
        private readonly Button _search = new() { Text = "Search PriceCharting", Dock = DockStyle.Top, Height = 32 };
        private readonly ListBox _products = new() { Dock = DockStyle.Fill, HorizontalScrollbar = true };
        private readonly Button _select = new() { Text = "Confirm exact printing / finish", Dock = DockStyle.Bottom, Height = 34, Enabled = false };
        public PriceChartingProduct? SelectedProduct => _products.SelectedItem as PriceChartingProduct;

        public ProductPicker(PriceChartingService service, string query, string description)
        {
            Text = "Match printing: " + description;
            Size = new Size(850, 400);
            MinimumSize = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            _query.Text = query;
            Controls.Add(_products);
            Controls.Add(_select);
            Controls.Add(_search);
            Controls.Add(_query);
            _products.SelectedIndexChanged += (_, _) => _select.Enabled = SelectedProduct != null;
            _select.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
            _search.Click += async (_, _) => {
                if (string.IsNullOrWhiteSpace(_query.Text))
                {
                    MessageBox.Show(this, "Enter the card name, set, and collector number.", "Graded prices");
                    return;
                }
                _search.Enabled = false;
                _products.Items.Clear();
                try
                {
                    var products = await service.SearchAsync(_query.Text.Trim());
                    if (IsDisposed)
                        return;
                    foreach (var product in products)
                        _products.Items.Add(product);
                    if (products.Count == 0)
                        MessageBox.Show(this, "No matches. Adjust the search text and retry. Do not substitute a different printing.", "Graded prices");
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException)
                {
                    if (!IsDisposed)
                        MessageBox.Show(this, ex is InvalidOperationException ? ex.Message :
                            "PriceCharting search failed. Check your token/subscription and connection, then retry.",
                            "Graded prices", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    if (!IsDisposed)
                        _search.Enabled = true;
                }
            };
            Shown += (_, _) => _search.PerformClick();
        }
    }
}
