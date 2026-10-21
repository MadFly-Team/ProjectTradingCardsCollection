using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Services;

public sealed class GbpPriceService
{
    private static readonly HttpClient DefaultHttp = new() { Timeout = TimeSpan.FromSeconds(15) };
    private readonly HttpClient _http;
    private decimal? _usdToGbp;
    private decimal? _eurToGbp;

    public GbpPriceService(HttpClient? http = null) => _http = http ?? DefaultHttp;

    public DateOnly? RateDate { get; private set; }

    public async Task LoadRatesAsync()
    {
        using var response = await _http.GetAsync("https://api.frankfurter.dev/v1/latest?base=USD&symbols=GBP,EUR");
        response.EnsureSuccessStatusCode();
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = json.RootElement;
        if (root.ValueKind != JsonValueKind.Object ||
            !root.TryGetProperty("base", out var baseCurrency) || baseCurrency.ValueKind != JsonValueKind.String || baseCurrency.GetString() != "USD" ||
            !root.TryGetProperty("rates", out var rates) || rates.ValueKind != JsonValueKind.Object ||
            !rates.TryGetProperty("GBP", out var gbpRate) || gbpRate.ValueKind != JsonValueKind.Number || !gbpRate.TryGetDecimal(out var gbp) || gbp <= 0 ||
            !rates.TryGetProperty("EUR", out var eurRate) || eurRate.ValueKind != JsonValueKind.Number || !eurRate.TryGetDecimal(out var eur) || eur <= 0 ||
            !root.TryGetProperty("date", out var date) || date.ValueKind != JsonValueKind.String ||
            !DateOnly.TryParseExact(date.GetString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var rateDate))
            throw new JsonException("The exchange-rate provider returned invalid USD/EUR/GBP rates.");

        _usdToGbp = gbp;
        _eurToGbp = gbp / eur;
        RateDate = rateDate;
    }

    public static bool TryParsePrice(string? text, out decimal price) =>
        decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out price) && price >= 0;

    public decimal? ConvertToGbp(decimal amount, string? currency) => currency switch
    {
        "USD" => amount * _usdToGbp,
        "EUR" => amount * _eurToGbp,
        _ => null
    };

    public static string FormatGbp(decimal value) =>
        value.ToString("C2", CultureInfo.GetCultureInfo("en-GB"));

    public string FormatEstimate(decimal amount, string? currency)
    {
        var gbp = ConvertToGbp(amount, currency);
        return gbp.HasValue ? $"~{FormatGbp(gbp.Value)} GBP" : "GBP unavailable";
    }

    public string FormatPrice(Prices? prices, bool foil = false)
    {
        var usd = foil ? prices?.UsdFoil : prices?.Usd;
        var eur = foil ? prices?.EurFoil : prices?.Eur;
        if (TryParsePrice(usd, out var usdPrice))
            return $"${usd} / {FormatEstimate(usdPrice, "USD")}";
        if (TryParsePrice(eur, out var eurPrice))
            return $"\u20ac{eur} / {FormatEstimate(eurPrice, "EUR")}";
        return "N/A";
    }

    public string FormatCollectionPrice(CollectionCard card) => !card.CurrentPrice.HasValue
        ? "N/A"
        : $"{card.CurrentPrice.Value.ToString("F2", CultureInfo.InvariantCulture)} {card.PriceCurrency ?? "(currency unknown)"} / {FormatEstimate(card.CurrentPrice.Value, card.PriceCurrency)}";

    public string FormatCollectionTotal(IEnumerable<CollectionCard> cards)
    {
        var priced = cards.Where(c => c.CurrentPrice.HasValue).ToList();
        var sourceTotals = priced.GroupBy(c => c.PriceCurrency)
            .Select(g => $"{g.Sum(c => c.GetTotalValue()).ToString("F2", CultureInfo.InvariantCulture)} {g.Key ?? "(currency unknown)"}");
        var source = priced.Count == 0 ? "0.00" : string.Join(" + ", sourceTotals);
        if (priced.Any(c => !ConvertToGbp(c.CurrentPrice!.Value, c.PriceCurrency).HasValue))
            return $"{source} | GBP total unavailable (rate or source currency missing)";
        var total = priced.Sum(c => ConvertToGbp(c.CurrentPrice!.Value, c.PriceCurrency)!.Value * c.Quantity);
        return $"{source} | ~{FormatGbp(total)} GBP total";
    }
}
