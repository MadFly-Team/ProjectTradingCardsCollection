using System.Text.Json.Serialization;

namespace ProjectMTGCollector.Models;

public class Card
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mana_cost")]
    public string? ManaCost { get; set; }

    [JsonPropertyName("cmc")]
    public decimal ConvertedManaCost { get; set; }

    [JsonPropertyName("type_line")]
    public string TypeLine { get; set; } = string.Empty;

    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; set; }

    [JsonPropertyName("power")]
    public string? Power { get; set; }

    [JsonPropertyName("toughness")]
    public string? Toughness { get; set; }

    [JsonPropertyName("colors")]
    public List<string>? Colors { get; set; }

    [JsonPropertyName("color_identity")]
    public List<string>? ColorIdentity { get; set; }

    [JsonPropertyName("set")]
    public string Set { get; set; } = string.Empty;

    [JsonPropertyName("set_name")]
    public string SetName { get; set; } = string.Empty;

    [JsonPropertyName("collector_number")]
    public string CollectorNumber { get; set; } = string.Empty;

    [JsonPropertyName("rarity")]
    public string Rarity { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public string? Artist { get; set; }

    [JsonPropertyName("image_uris")]
    public ImageUris? ImageUris { get; set; }

    [JsonPropertyName("prices")]
    public Prices? Prices { get; set; }

    [JsonPropertyName("legalities")]
    public Dictionary<string, string>? Legalities { get; set; }

    [JsonPropertyName("released_at")]
    public string? ReleasedAt { get; set; }

    [JsonPropertyName("card_faces")]
    public List<CardFace>? CardFaces { get; set; }

    [JsonPropertyName("finishes")]
    public List<string>? Finishes { get; set; }

    [JsonPropertyName("foil")]
    public bool Foil { get; set; }

    [JsonPropertyName("nonfoil")]
    public bool Nonfoil { get; set; }

    [JsonPropertyName("frame_effects")]
    public List<string>? FrameEffects { get; set; }

    [JsonPropertyName("frame")]
    public string? Frame { get; set; }

    [JsonPropertyName("border_color")]
    public string? BorderColor { get; set; }

    [JsonPropertyName("promo")]
    public bool Promo { get; set; }

    [JsonPropertyName("promo_types")]
    public List<string>? PromoTypes { get; set; }

    [JsonPropertyName("variation")]
    public bool Variation { get; set; }

    public List<CardVariant> Variants { get; set; } = new();

    [JsonIgnore]
    public CardVariant? SelectedVariant { get; private set; }

    public Card WithVariant(CardVariant variant)
    {
        if (!Variants.Any(v => v.Id == variant.Id))
            throw new ArgumentException("The selected variant does not belong to this card.", nameof(variant));

        var selected = (Card)MemberwiseClone();
        selected.SelectedVariant = Variants.First(v => v.Id == variant.Id);
        selected.Prices = selected.SelectedVariant.Prices;
        return selected;
    }
}

public class ImageUris
{
    [JsonPropertyName("small")]
    public string? Small { get; set; }

    [JsonPropertyName("normal")]
    public string? Normal { get; set; }

    [JsonPropertyName("large")]
    public string? Large { get; set; }

    [JsonPropertyName("png")]
    public string? Png { get; set; }

    [JsonPropertyName("art_crop")]
    public string? ArtCrop { get; set; }

    [JsonPropertyName("border_crop")]
    public string? BorderCrop { get; set; }
}

public class Prices
{
    [JsonPropertyName("usd")]
    public string? Usd { get; set; }

    [JsonPropertyName("usd_foil")]
    public string? UsdFoil { get; set; }

    [JsonPropertyName("eur")]
    public string? Eur { get; set; }

    [JsonPropertyName("eur_foil")]
    public string? EurFoil { get; set; }

    [JsonPropertyName("tix")]
    public string? Tix { get; set; }
}

public class CardFace
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("mana_cost")]
    public string? ManaCost { get; set; }

    [JsonPropertyName("type_line")]
    public string TypeLine { get; set; } = string.Empty;

    [JsonPropertyName("oracle_text")]
    public string? OracleText { get; set; }

    [JsonPropertyName("power")]
    public string? Power { get; set; }

    [JsonPropertyName("toughness")]
    public string? Toughness { get; set; }

    [JsonPropertyName("image_uris")]
    public ImageUris? ImageUris { get; set; }
}
