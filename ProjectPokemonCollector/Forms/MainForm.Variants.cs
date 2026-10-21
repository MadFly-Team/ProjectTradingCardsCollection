using ProjectMTGCollector.Models;
using ProjectMTGCollector.Services;

namespace ProjectMTGCollector.Forms;

public partial class MainForm
{
    private void PopulateVariantRows(ListView list, Label header, List<Card> cards)
    {
        list.BeginUpdate();
        try
        {
            list.Items.Clear();
            var count = 0;
            foreach (var card in cards)
            {
                foreach (var variant in card.Variants)
                {
                    var selected = card.WithVariant(variant);
                    var item = new ListViewItem(card.SetName) { Tag = selected };
                    item.SubItems.Add(variant.Description);
                    item.SubItems.Add(card.CollectorNumber);
                    item.SubItems.Add(FormatVariantCurrency(variant.Prices?.Usd, "USD"));
                    item.SubItems.Add(FormatVariantCurrency(variant.Prices?.Eur, "EUR"));
                    item.SubItems.Add(card.ReleasedAt ?? "N/A");
                    list.Items.Add(item);
                    count++;
                }
            }
            header.Text = $"Physical Variants & Printings: {count} (select a row; load all for other printings)";
        }
        finally
        {
            list.EndUpdate();
        }
    }

    private string FormatVariantCurrency(string? value, string currency) =>
        GbpPriceService.TryParsePrice(value, out var amount)
            ? $"{value} {currency} / {_gbpPrices.FormatEstimate(amount, currency)}"
            : "N/A";
}
