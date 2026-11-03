using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Services;

public partial class CollectionService
{
    private sealed class CollectionFile
    {
        public required int Version { get; set; }
        public required string Game { get; set; }
        public required List<Collection> Collections { get; set; }
    }

    public async Task ExportCollectionsAsync(string path)
    {
        if (string.Equals(Path.GetFullPath(path), Path.GetFullPath(_collectionsFilePath), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Choose a backup file other than the app's active collections.json file.");

        var json = JsonSerializer.Serialize(new CollectionFile
        {
            Version = 1,
            Game = CollectionFileGame,
            Collections = _collections
        }, _jsonOptions);
        await WriteCollectionFileAsync(path, json);
    }

    public async Task<int> ImportCollectionsAsync(string path)
    {
        var json = await File.ReadAllTextAsync(path);
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(typeInfo =>
        {
            if (typeInfo.Type == typeof(Collection) || typeInfo.Type == typeof(CollectionFolder) ||
                typeInfo.Type == typeof(CollectionCard))
            {
                foreach (var property in typeInfo.Properties)
                {
                    if (property.Set != null)
                        property.IsRequired = true;
                }
            }
        });
        var importOptions = new JsonSerializerOptions(_jsonOptions) { TypeInfoResolver = resolver };
        var file = JsonSerializer.Deserialize<CollectionFile>(json, importOptions)
            ?? throw new InvalidDataException("The file does not contain a collection export.");
        if (file.Version != 1)
            throw new InvalidDataException("This collection file version is not supported.");
        if (file.Game != CollectionFileGame)
            throw new InvalidDataException($"Choose a {CollectionFileGame} collection export. Files from other games cannot be imported.");
        if (file.Collections == null || file.Collections.Count == 0)
            throw new InvalidDataException("The export contains no collections to import.");

        foreach (var collection in file.Collections)
        {
            if (collection == null || string.IsNullOrWhiteSpace(collection.Name) ||
                collection.Description == null || collection.Folders == null)
                throw new InvalidDataException("A collection has an invalid name, description, or folder list.");
            foreach (var folder in collection.Folders)
            {
                if (folder == null || string.IsNullOrWhiteSpace(folder.Name) ||
                    folder.Description == null || folder.Cards == null)
                    throw new InvalidDataException("A folder has an invalid name, description, or card list.");
                foreach (var card in folder.Cards)
                {
                    if (card == null || string.IsNullOrWhiteSpace(card.CardId) ||
                        string.IsNullOrWhiteSpace(card.CardName) || card.SetCode == null ||
                        card.SetName == null || card.CollectorNumber == null || card.Condition == null ||
                        card.Quantity <= 0 || card.CurrentPrice < 0 || card.PurchasePrice < 0)
                        throw new InvalidDataException("A card has missing details, a non-positive quantity, or a negative price.");
                    ValidateImportedCard(card);
                }
                folder.Id = Guid.NewGuid().ToString();
            }
            collection.Id = Guid.NewGuid().ToString();
        }

        var combined = _collections.Concat(file.Collections).ToList();
        try
        {
            _ = combined.Sum(c => c.GetTotalCardCount());
            _ = combined.Sum(c => c.GetTotalValue());
        }
        catch (OverflowException ex)
        {
            throw new InvalidDataException("The imported quantities or values exceed the supported totals.", ex);
        }

        // Publish the new in-memory state only after the complete file is safely saved.
        await WriteCollectionFileAsync(_collectionsFilePath, JsonSerializer.Serialize(combined, _jsonOptions));
        _collections = combined;
        _lastLoadedWriteTimeUtc = GetCollectionsFileLastWriteUtc();
        CollectionsChanged?.Invoke(this, EventArgs.Empty);
        return file.Collections.Count;
    }

    static partial void ValidateImportedCard(CollectionCard card);

    private static async Task WriteCollectionFileAsync(string path, string json)
    {
        var fullPath = Path.GetFullPath(path);
        var temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await File.WriteAllTextAsync(temporaryPath, json);
            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }
}
