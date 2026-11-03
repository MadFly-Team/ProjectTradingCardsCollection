using System.Globalization;
using System.Net.Http;
using System.Text.Json;

namespace ProjectMTGCollector.Services;

public sealed record PriceChartingProduct(string Id, string Name, string Set,
    IReadOnlyDictionary<string, long> Prices)
{
    public override string ToString() => $"{Set} - {Name} (ID {Id})";

    public decimal? GetPrice(string key) => Prices.TryGetValue(key, out var cents) && cents > 0
        ? cents / 100m : null;
}

public sealed class PriceChartingService
{
    private static readonly HttpClient DefaultHttp = new() { Timeout = TimeSpan.FromSeconds(20) };
    private static readonly Mutex RequestGate = new(false, "Local\\TradingCardCollector.PriceCharting");
    private readonly HttpClient _http;

    public PriceChartingService(HttpClient? http = null) => _http = http ?? DefaultHttp;

    public async Task<List<PriceChartingProduct>> SearchAsync(string query)
    {
        using var json = await RequestAsync("products", "q", query).ConfigureAwait(false);
        if (!json.RootElement.TryGetProperty("products", out var products) || products.ValueKind != JsonValueKind.Array)
            throw new JsonException("PriceCharting returned an invalid product list.");
        return products.EnumerateArray().Select(ParseProduct).ToList();
    }

    public async Task<PriceChartingProduct> GetProductAsync(string id)
    {
        using var json = await RequestAsync("product", "id", id).ConfigureAwait(false);
        var product = ParseProduct(json.RootElement);
        if (product.Id != id)
            throw new JsonException("PriceCharting returned a different product than the selected printing.");
        return product;
    }

    private async Task<JsonDocument> RequestAsync(string endpoint, string parameter, string value)
    {
        var token = Environment.GetEnvironmentVariable("PRICECHARTING_API_TOKEN")?.Trim();
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Set PRICECHARTING_API_TOKEN to your paid PriceCharting API token, then restart the app (and Visual Studio if launching from it).");
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A product search or ID is required.", nameof(value));

        return await Task.Run(() =>
        {
            // Mutex ownership must stay on one worker thread, never the UI thread.
            try
            {
                if (!RequestGate.WaitOne(TimeSpan.FromSeconds(60)))
                    throw new HttpRequestException("Another graded-price lookup is busy. Retry shortly.");
            }
            catch (AbandonedMutexException)
            {
                System.Diagnostics.Trace.TraceWarning("Recovering the PriceCharting request gate after another app exited.");
                Thread.Sleep(1100);
            }
            try
            {
                using var response = _http.GetAsync(
                    $"https://www.pricecharting.com/api/{endpoint}?t={Uri.EscapeDataString(token)}&{parameter}={Uri.EscapeDataString(value)}").GetAwaiter().GetResult();
                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"PriceCharting returned HTTP {(int)response.StatusCode}. Check your subscription/token or retry later.");
                var json = JsonDocument.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                if (json.RootElement.ValueKind != JsonValueKind.Object ||
                    !json.RootElement.TryGetProperty("status", out var status) ||
                    status.ValueKind != JsonValueKind.String || status.GetString() != "success")
                {
                    json.Dispose();
                    throw new JsonException("PriceCharting rejected the request. Check your subscription/token or retry later.");
                }
                return json;
            }
            finally
            {
                // Keep requests at least one second apart across both apps.
                Thread.Sleep(1100);
                RequestGate.ReleaseMutex();
            }
        }).ConfigureAwait(false);
    }

    private static PriceChartingProduct ParseProduct(JsonElement product)
    {
        static string RequiredString(JsonElement value, string key)
        {
            if (value.ValueKind != JsonValueKind.Object ||
                !value.TryGetProperty(key, out var field) || field.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(field.GetString()))
                throw new JsonException($"PriceCharting returned an invalid {key}.");
            return field.GetString()!;
        }

        var id = RequiredString(product, "id");
        var name = RequiredString(product, "product-name");
        var set = RequiredString(product, "console-name");
        var prices = new Dictionary<string, long>();
        foreach (var property in product.EnumerateObject().Where(p => p.Name.EndsWith("-price", StringComparison.Ordinal)))
        {
            if (property.Value.ValueKind == JsonValueKind.Null)
                continue;
            long cents = 0;
            bool valid = property.Value.ValueKind == JsonValueKind.Number
                ? property.Value.TryGetInt64(out cents)
                : property.Value.ValueKind == JsonValueKind.String &&
                    long.TryParse(property.Value.GetString(), NumberStyles.None, CultureInfo.InvariantCulture, out cents);
            if (!valid || cents < 0)
                throw new JsonException($"PriceCharting returned an invalid {property.Name}.");
            prices[property.Name] = cents;
        }
        return new PriceChartingProduct(id, name, set, prices);
    }
}
