using System.Text.Json;
using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Services;

public class CollectionService
{
    private readonly string _collectionsFilePath;
    private readonly JsonSerializerOptions _jsonOptions;
    private List<Collection> _collections;
    private DateTime? _lastLoadedWriteTimeUtc;

    public event EventHandler? CollectionsChanged;

    public CollectionService(string? storageDirectory = null)
    {
        var appDataPath = storageDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PokemonCollector"
        );

        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        _collectionsFilePath = Path.Combine(appDataPath, "collections.json");
        _collections = new List<Collection>();
        _lastLoadedWriteTimeUtc = GetCollectionsFileLastWriteUtc();

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task LoadCollectionsAsync()
    {
        try
        {
            if (File.Exists(_collectionsFilePath))
            {
                var json = await File.ReadAllTextAsync(_collectionsFilePath);
                _collections = JsonSerializer.Deserialize<List<Collection>>(json, _jsonOptions) ?? new List<Collection>();
                _lastLoadedWriteTimeUtc = GetCollectionsFileLastWriteUtc();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading collections: {ex.Message}");
            _collections = new List<Collection>();
        }
    }

    public async Task SaveCollectionsAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_collections, _jsonOptions);
            await File.WriteAllTextAsync(_collectionsFilePath, json);
            _lastLoadedWriteTimeUtc = GetCollectionsFileLastWriteUtc();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving collections: {ex.Message}");
            throw;
        }
    }

    public DateTime? GetCollectionsFileLastWriteUtc()
    {
        return File.Exists(_collectionsFilePath)
            ? File.GetLastWriteTimeUtc(_collectionsFilePath)
            : (DateTime?)null;
    }

    public async Task<bool> ReloadIfChangedAsync()
    {
        var fileTime = GetCollectionsFileLastWriteUtc();

        // If file doesn't exist and we have no collections loaded, nothing to do
        if (fileTime == null)
        {
            if (_collections.Count == 0)
                return false;

            // File missing but collections exist; attempt reload to reset state
            await LoadCollectionsAsync();
            CollectionsChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        if (_lastLoadedWriteTimeUtc.HasValue && fileTime <= _lastLoadedWriteTimeUtc)
            return false;

        await LoadCollectionsAsync();
        CollectionsChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public List<Collection> GetAllCollections()
    {
        return _collections;
    }

    public Collection? GetCollection(string collectionId)
    {
        return _collections.FirstOrDefault(c => c.Id == collectionId);
    }

    public async Task<Collection> CreateCollectionAsync(string name, string description, string? imageUrl = null)
    {
        var collection = new Collection
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl
        };

        _collections.Add(collection);
        await SaveCollectionsAsync();
        CollectionsChanged?.Invoke(this, EventArgs.Empty);

        return collection;
    }

    public async Task UpdateCollectionAsync(Collection collection)
    {
        collection.UpdatedAt = DateTime.Now;
        await SaveCollectionsAsync();
        CollectionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task DeleteCollectionAsync(string collectionId)
    {
        _collections.RemoveAll(c => c.Id == collectionId);
        await SaveCollectionsAsync();
        CollectionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<CollectionFolder> AddFolderAsync(string collectionId, string folderName, string description = "")
    {
        var collection = GetCollection(collectionId);
        if (collection == null)
            throw new InvalidOperationException("Collection not found");

        var folder = new CollectionFolder
        {
            Name = folderName,
            Description = description
        };

        collection.Folders.Add(folder);
        await UpdateCollectionAsync(collection);

        return folder;
    }

    public async Task DeleteFolderAsync(string collectionId, string folderId)
    {
        var collection = GetCollection(collectionId);
        if (collection == null)
            throw new InvalidOperationException("Collection not found");

        collection.Folders.RemoveAll(f => f.Id == folderId);
        await UpdateCollectionAsync(collection);
    }

    public async Task AddCardToFolderAsync(string collectionId, string folderId, Card card, int quantity = 1,
        CardVariant? variant = null, string condition = "Near Mint", string? notes = null)
    {
        variant ??= card.SelectedVariant;
        var selected = card.Variants.FirstOrDefault(v => v.Id == variant?.Id)
            ?? throw new ArgumentException("Select a variant belonging to this card.", nameof(variant));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        var selectedCard = card.WithVariant(selected);
        var collection = GetCollection(collectionId);
        if (collection == null)
            throw new InvalidOperationException("Collection not found");

        var folder = collection.Folders.FirstOrDefault(f => f.Id == folderId);
        if (folder == null)
            throw new InvalidOperationException("Folder not found");

        var existingCard = folder.Cards.FirstOrDefault(c =>
            c.Matches(card.Id, selected.IsHolo, selected.Id));

        if (existingCard != null)
        {
            existingCard.Quantity += quantity;
            existingCard.Condition = condition;
            if (!string.IsNullOrWhiteSpace(notes)) existingCard.Notes = notes;
        }
        else
        {
            var collectionCard = new CollectionCard
            {
                CardId = card.Id,
                CardName = card.Name,
                SetCode = card.Set,
                SetName = card.SetName,
                CollectorNumber = card.CollectorNumber,
                ImageUrl = card.ImageUris?.Normal ?? card.CardFaces?.FirstOrDefault()?.ImageUris?.Normal,
                Quantity = quantity,
                IsFoil = selected.IsHolo,
                Variant = selected,
                Condition = condition,
                Notes = notes,
                CurrentPrice = GetCardPrice(selectedCard, false),
                PriceCurrency = GetCardPriceCurrency(selectedCard, false)
            };

            folder.Cards.Add(collectionCard);
        }

        await UpdateCollectionAsync(collection);
    }

    public async Task RemoveCardFromFolderAsync(string collectionId, string folderId, string cardId, bool isFoil, string? variantId = null)
    {
        var collection = GetCollection(collectionId);
        if (collection == null)
            throw new InvalidOperationException("Collection not found");

        var folder = collection.Folders.FirstOrDefault(f => f.Id == folderId);
        if (folder == null)
            throw new InvalidOperationException("Folder not found");

        folder.Cards.RemoveAll(c => c.Matches(cardId, isFoil, variantId));
        await UpdateCollectionAsync(collection);
    }

    public async Task UpdateCardQuantityAsync(string collectionId, string folderId, string cardId, bool isFoil, int newQuantity, string? variantId = null)
    {
        var collection = GetCollection(collectionId);
        if (collection == null)
            throw new InvalidOperationException("Collection not found");

        var folder = collection.Folders.FirstOrDefault(f => f.Id == folderId);
        if (folder == null)
            throw new InvalidOperationException("Folder not found");

        var card = folder.Cards.FirstOrDefault(c => c.Matches(cardId, isFoil, variantId));
        if (card != null)
        {
            if (newQuantity <= 0)
            {
                folder.Cards.Remove(card);
            }
            else
            {
                card.Quantity = newQuantity;
            }

            await UpdateCollectionAsync(collection);
        }
    }

    private decimal? GetCardPrice(Card card, bool isFoil)
    {
        if (card.Prices == null)
            return null;

        var currency = GetCardPriceCurrency(card, isFoil);
        var priceString = currency == "USD"
            ? (isFoil ? card.Prices.UsdFoil : card.Prices.Usd)
            : (isFoil ? card.Prices.EurFoil : card.Prices.Eur);
        return GbpPriceService.TryParsePrice(priceString, out var price) ? price : null;
    }

    private static string? GetCardPriceCurrency(Card card, bool isFoil)
    {
        if (GbpPriceService.TryParsePrice(isFoil ? card.Prices?.UsdFoil : card.Prices?.Usd, out _))
            return "USD";
        if (GbpPriceService.TryParsePrice(isFoil ? card.Prices?.EurFoil : card.Prices?.Eur, out _))
            return "EUR";
        return null;
    }

    public int GetTotalCardsAcrossAllCollections()
    {
        return _collections.Sum(c => c.GetTotalCardCount());
    }

    public decimal GetTotalValueAcrossAllCollections()
    {
        return _collections.Sum(c => c.GetTotalValue());
    }
}
