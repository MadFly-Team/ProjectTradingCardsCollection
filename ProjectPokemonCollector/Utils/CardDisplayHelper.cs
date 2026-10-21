using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Utils;

public static class CardDisplayHelper
{
    public static void DisplayCard(Card card)
    {
        Console.WriteLine("\n" + new string('=', 80));
        Console.WriteLine($"CARD: {card.Name}");
        Console.WriteLine(new string('=', 80));
        Console.WriteLine($"Category / Stage: {card.TypeLine}");

        if (!string.IsNullOrEmpty(card.OracleText))
            Console.WriteLine($"\nAttacks & Abilities:\n  {card.OracleText}");

        if (!string.IsNullOrEmpty(card.Power))
            Console.WriteLine($"\nHP: {card.Power}");

        Console.WriteLine($"\nSet: {card.SetName} ({card.Set.ToUpper()}) #{card.CollectorNumber}");
        Console.WriteLine($"Rarity: {card.Rarity}");

        if (!string.IsNullOrEmpty(card.Artist))
            Console.WriteLine($"Artist: {card.Artist}");
        if (card.Colors is { Count: > 0 })
            Console.WriteLine($"Pokemon types: {string.Join(", ", card.Colors)}");

        if (card.Prices != null)
        {
            Console.WriteLine("\nPrices:");
            if (!string.IsNullOrEmpty(card.Prices.Usd))
                Console.WriteLine($"  USD normal: ${card.Prices.Usd}");
            if (!string.IsNullOrEmpty(card.Prices.UsdFoil))
                Console.WriteLine($"  USD holo: ${card.Prices.UsdFoil}");
            if (!string.IsNullOrEmpty(card.Prices.Eur))
                Console.WriteLine($"  EUR normal: {card.Prices.Eur}");
            if (!string.IsNullOrEmpty(card.Prices.EurFoil))
                Console.WriteLine($"  EUR holo: {card.Prices.EurFoil}");
        }

        if (card.ImageUris?.Normal is { } imageUrl)
            Console.WriteLine($"\nImage: {imageUrl}");

        if (card.Legalities is { Count: > 0 })
        {
            Console.WriteLine("\nLegalities:");
            foreach (var legality in card.Legalities.Where(l => l.Value == "legal"))
                Console.WriteLine($"  {legality.Key}: legal");
        }

        Console.WriteLine(new string('=', 80));
    }

    public static void DisplayCardList(List<Card> cards)
    {
        Console.WriteLine($"\nFound {cards.Count} card(s):\n");
        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            Console.WriteLine($"{i + 1}. {card.Name} ({card.Set.ToUpper()}) - {card.Rarity}");
        }
    }

    public static void DisplaySet(Set set)
    {
        Console.WriteLine("\n" + new string('=', 80));
        Console.WriteLine($"SET: {set.Name}");
        Console.WriteLine(new string('=', 80));
        Console.WriteLine($"Code: {set.Code.ToUpper()}");
        Console.WriteLine($"Type: {set.SetType}");
        Console.WriteLine($"Card Count: {set.CardCount}");
        if (!string.IsNullOrEmpty(set.ReleasedAt))
            Console.WriteLine($"Released: {set.ReleasedAt}");
        Console.WriteLine(new string('=', 80));
    }

    public static void DisplaySetList(List<Set> sets)
    {
        Console.WriteLine($"\nFound {sets.Count} set(s):\n");
        for (int i = 0; i < sets.Count && i < 20; i++)
        {
            var set = sets[i];
            Console.WriteLine($"{i + 1}. {set.Name} ({set.Code.ToUpper()}) - {set.CardCount} cards - {set.ReleasedAt}");
        }
        if (sets.Count > 20)
            Console.WriteLine($"\n... and {sets.Count - 20} more sets.");
    }
}
