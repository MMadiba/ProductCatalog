using System.Collections.Concurrent;

namespace ProductCatalog.Api.Services;

public class SearchCacheService
{
    private readonly ConcurrentDictionary<string, (object Result, DateTime Expires)> _cache = new();
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(5);

    public T? Get<T>(string key) where T : class
    {
        if (!_cache.TryGetValue(key, out var entry) || DateTime.UtcNow > entry.Expires)
            return null;
        return entry.Result as T;
    }

    public void Set<T>(string key, T value) where T : class
    {
        _cache[key] = (value, DateTime.UtcNow.Add(_ttl));
    }

    public string BuildKey(string? search, int? categoryId, int page, int pageSize)
    {
        return $"search:{search ?? ""}:cat:{categoryId?.ToString() ?? "all"}:p:{page}:ps:{pageSize}";
    }
}
