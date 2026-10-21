using System.Globalization;
using System.Text.Json.Serialization;

namespace ProjectMTGCollector.Models;

public sealed class CardVariant
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    [JsonPropertyName("subtype")]
    public string? Subtype { get; set; }
    [JsonPropertyName("size")]
    public string? Size { get; set; }
    [JsonPropertyName("stamps")]
    public List<string> Stamps { get; set; } = new();
    [JsonPropertyName("isSummary")]
    public bool IsSummary { get; set; }
    [JsonPropertyName("editionUnspecified")]
    public bool EditionUnspecified { get; set; }
    [JsonPropertyName("prices")]
    public Prices? Prices { get; set; }

    [JsonIgnore]
    public bool IsHolo => Type.Contains("holo", StringComparison.OrdinalIgnoreCase)
        || Type.Contains("reverse", StringComparison.OrdinalIgnoreCase);

    [JsonIgnore]
    public string Description
    {
        get
        {
            var parts = new List<string> { Label(Type) };
            if (!string.IsNullOrWhiteSpace(Subtype)) parts.Add(Label(Subtype));
            parts.AddRange(Stamps.Select(Label));
            if (!string.IsNullOrWhiteSpace(Size)) parts.Add(Label(Size));
            if (EditionUnspecified) parts.Add("Edition unspecified (First Edition available)");
            return string.Join(" / ", parts);
        }
    }

    public override string ToString() => Description;

    private static string Label(string value) => value switch
    {
        "reverse" or "reverse-holofoil" => "Reverse Holo",
        "holofoil" => "Holo",
        "1st-edition" => "First Edition",
        "wPromo" => "W Promo",
        "unspecified" => "Variant unspecified by provider",
        _ => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value.Replace('-', ' '))
    };
}
