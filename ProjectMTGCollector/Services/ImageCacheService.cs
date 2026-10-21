using System.Collections.Concurrent;
using System.Drawing;
using System.IO;

namespace ProjectMTGCollector.Services;

public class ImageCacheService
{
    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, Image?> _cache;
    private readonly SemaphoreSlim _downloadSemaphore;
        private readonly Image _placeholderImage;

    public ImageCacheService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ProjectMTGCollector/1.0");
        _cache = new ConcurrentDictionary<string, Image?>();
        _downloadSemaphore = new SemaphoreSlim(5, 5);
        // create simple placeholder image
        _placeholderImage = CreatePlaceholderImage(320, 420);
    }

    public async Task<Image?> GetImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        if (_cache.TryGetValue(imageUrl, out var cachedImage))
            return cachedImage;

        await _downloadSemaphore.WaitAsync();
        try
        {
            if (_cache.TryGetValue(imageUrl, out cachedImage))
                return cachedImage;

            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
            using var ms = new MemoryStream(imageBytes);
            var image = Image.FromStream(ms);
            _cache[imageUrl] = image;
            return image;
        }
        catch
        {
            _cache[imageUrl] = null;
            return null;
        }
        finally
        {
            _downloadSemaphore.Release();
        }
    }

    public Image? GetCachedImage(string imageUrl)
    {
        _cache.TryGetValue(imageUrl, out var image);
        return image;
    }

    public Image GetPlaceholderImage()
    {
        return _placeholderImage;
    }

    public void ClearCache()
    {
        foreach (var image in _cache.Values)
        {
            image?.Dispose();
        }
        _cache.Clear();
    }

    private Image CreatePlaceholderImage(int width, int height)
    {
        var bmp = new Bitmap(width, height);
        using var g = Graphics.FromImage(bmp);
        g.Clear(Color.LightGray);
        var font = new Font("Segoe UI", 20F, FontStyle.Bold);
        var brush = Brushes.DarkGray;
        var text = "No Image";
        var size = g.MeasureString(text, font);
        g.DrawString(text, font, brush, (width - size.Width) / 2, (height - size.Height) / 2);
        return bmp;
    }
}
