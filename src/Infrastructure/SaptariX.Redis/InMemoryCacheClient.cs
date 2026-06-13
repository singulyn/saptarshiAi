using System.Collections.Concurrent;

namespace SaptariX.Redis;

public sealed class InMemoryCacheClient : ICacheClient
{
    private readonly ConcurrentDictionary<string, CacheItem> _items = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (!_items.TryGetValue(key, out var item))
        {
            return Task.FromResult<T?>(default);
        }

        if (item.ExpiresAtUtc.HasValue && item.ExpiresAtUtc.Value <= DateTimeOffset.UtcNow)
        {
            _items.TryRemove(key, out _);
            return Task.FromResult<T?>(default);
        }

        return Task.FromResult(item.Value is T typed ? typed : default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default)
    {
        _items[key] = new CacheItem(value, ttl.HasValue ? DateTimeOffset.UtcNow.Add(ttl.Value) : null);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _items.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    private sealed record CacheItem(object? Value, DateTimeOffset? ExpiresAtUtc);
}
