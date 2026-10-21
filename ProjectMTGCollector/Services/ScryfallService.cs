using System.Text.Json;
using ProjectMTGCollector.Models;

namespace ProjectMTGCollector.Services;

public record PageProgress(int Loaded, int? Total, int Page);

public class ScryfallService {
  private readonly HttpClient _httpClient;
  private readonly JsonSerializerOptions _jsonOptions;
  private const string BaseUrl = "https://api.scryfall.com";

  public ScryfallService() {
    _httpClient = new HttpClient();
    _httpClient.DefaultRequestHeaders.Add("User-Agent",
                                          "ProjectMTGCollector/1.0");
    _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");

    _jsonOptions =
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
  }

  public async Task<Card?>
  GetCardBySetAndCollectorAsync(string setCode, string collectorNumber) {
    try {
      var encodedSet = Uri.EscapeDataString(setCode);
      var encodedNumber = Uri.EscapeDataString(collectorNumber);
      var response = await _httpClient.GetAsync(
          $"{BaseUrl}/cards/{encodedSet}/{encodedNumber}");

      if (!response.IsSuccessStatusCode) {
        Console.WriteLine($"Error: {response.StatusCode}");
        return null;
      }

      var json = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<Card>(json, _jsonOptions);
    } catch (Exception ex) {
      Console.WriteLine($"Error fetching card by set and number: {ex.Message}");
      return null;
    }
  }

  public async Task<Card?> GetCardByNameAsync(string cardName) {
    try {
      var encodedName = Uri.EscapeDataString(cardName);
      var response = await _httpClient.GetAsync(
          $"{BaseUrl}/cards/named?fuzzy={encodedName}");

      if (!response.IsSuccessStatusCode) {
        Console.WriteLine($"Error: {response.StatusCode}");
        return null;
      }

      var json = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<Card>(json, _jsonOptions);
    } catch (Exception ex) {
      Console.WriteLine($"Error fetching card: {ex.Message}");
      return null;
    }
  }

  public async Task<List<Card>> GetAllPrintingsAsync(string cardName) {
    try {
      var encodedName = Uri.EscapeDataString($"!\"{cardName}\"");
      var initialUrl =
          $"{BaseUrl}/cards/search?q={encodedName}&unique=prints&order=released";
      return await FetchPagedResultsAsync<Card>(initialUrl);
    } catch (Exception ex) {
      Console.WriteLine($"Error fetching card printings: {ex.Message}");
      return new List<Card>();
    }
  }

  public async Task<List<Card>> SearchCardsAsync(string query,
                                                 int maxResults = 50) {
    try {
      var encodedQuery = Uri.EscapeDataString(query);
      var initialUrl = $"{BaseUrl}/cards/search?q={encodedQuery}&unique=cards";
      var results = await FetchPagedResultsAsync<Card>(initialUrl, maxResults);
      return results.Take(maxResults).ToList();
    } catch (Exception ex) {
      Console.WriteLine($"Error searching cards: {ex.Message}");
      return new List<Card>();
    }
  }

  public async Task<List<Card>>
  GetCardsBySetAsync(string setCode, IProgress<PageProgress>? progress = null) {
    try {
      var encodedSet = Uri.EscapeDataString(setCode);
      var initialUrl =
          $"{BaseUrl}/cards/search?q=set:{encodedSet}&unique=prints&order=set";
      return await FetchPagedResultsAsync<Card>(initialUrl, null, progress);
    } catch (Exception ex) {
      Console.WriteLine($"Error fetching cards from set: {ex.Message}");
      return new List<Card>();
    }
  }

  public async Task<List<Set>> GetAllSetsAsync() {
    try {
      var initialUrl = $"{BaseUrl}/sets";
      return await FetchPagedResultsAsync<Set>(initialUrl);
    } catch (Exception ex) {
      Console.WriteLine($"Error fetching sets: {ex.Message}");
      return new List<Set>();
    }
  }

  public async Task<Set?> GetSetAsync(string setCode) {
    try {
      var response = await _httpClient.GetAsync($"{BaseUrl}/sets/{setCode}");

      if (!response.IsSuccessStatusCode) {
        Console.WriteLine($"Error: {response.StatusCode}");
        return null;
      }

      var json = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<Set>(json, _jsonOptions);
    } catch (Exception ex) {
      Console.WriteLine($"Error fetching set: {ex.Message}");
      return null;
    }
  }

  public async Task DownloadCardImageAsync(string imageUrl, string filePath) {
    try {
      var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
      await File.WriteAllBytesAsync(filePath, imageBytes);
      Console.WriteLine($"Image saved to: {filePath}");
    } catch (Exception ex) {
      Console.WriteLine($"Error downloading image: {ex.Message}");
    }
  }

  private async Task<List<T>>
  FetchPagedResultsAsync<T>(string initialUrl, int? maxTotal = null,
                            IProgress<PageProgress>? progress = null) {
    var results = new List<T>();
    var nextPage = initialUrl;
    int page = 1;

    while (!string.IsNullOrEmpty(nextPage)) {
      var pageResult = await GetPagedResponseAsync<T>(nextPage);
      if (pageResult?.Data == null || pageResult.Data.Count == 0) {
        break;
      }

      results.AddRange(pageResult.Data);

      progress?.Report(
          new PageProgress(results.Count, pageResult.TotalCards, page));

      if (maxTotal.HasValue && results.Count >= maxTotal.Value) {
        return results.Take(maxTotal.Value).ToList();
      }

      if (pageResult.HasMore && !string.IsNullOrEmpty(pageResult.NextPage)) {
        nextPage = pageResult.NextPage;
        page++;
      } else {
        break;
      }
    }

    return results;
  }

  private async Task<ScryfallResponse<T>?>
  GetPagedResponseAsync<T>(string url) {
    var response = await _httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode) {
      Console.WriteLine($"Error: {response.StatusCode}");
      return null;
    }

    var json = await response.Content.ReadAsStringAsync();
    return JsonSerializer.Deserialize<ScryfallResponse<T>>(json, _jsonOptions);
  }
}
