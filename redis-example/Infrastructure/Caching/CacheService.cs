using System.Text.Json;
using redis_example.Abstractions.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace redis_example.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    /// <summary>
    /// A SemaphoreSlim instance to handle concurrent access to the caching mechanism.
    /// Prevents simultaneous factory calls for the same key.
    /// </summary>
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    /// <summary>
    /// Default caching options with absolute expiration of 2 minutes
    /// and sliding expiration of 30 seconds.
    /// </summary>
    private static readonly DistributedCacheEntryOptions Default = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
        SlidingExpiration = TimeSpan.FromMinutes(0.5)
    };

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Retrieves a cached value or creates and caches it if not found.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">A factory method to generate the value if not in cache.</param>
    /// <param name="options">Optional cache entry options for expiration settings.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>The cached or newly created value.</returns>
    public async Task<T?> GetOrCreateAsync<T>(
        string key, 
        Func<Task<T>> factory, 
        DistributedCacheEntryOptions? options = null, 
        CancellationToken cancellationToken = default
    )
    {
        // Attempt to retrieve the cached value
        var cachedValue = await _cache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedValue))
        {
            // Deserialize and return if found
            var value = JsonSerializer.Deserialize<T>(cachedValue);
            if (value is not null) return value;
        }

        // Ensure only one thread executes the factory logic at a time
        if (!await Semaphore.WaitAsync(2000, cancellationToken)) return default;

        try
        {
            // Recheck the cache to avoid redundant factory calls
            cachedValue = await _cache.GetStringAsync(key, cancellationToken);

            if (!string.IsNullOrWhiteSpace(cachedValue))
            {
                var value = JsonSerializer.Deserialize<T>(cachedValue);
                if (value is not null) return value;
            }

            // Generate a new value using the factory method
            var newValue = await factory();
            if (newValue is null) return default;

            // Cache the newly created value
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(newValue), options ?? Default, cancellationToken);

            return newValue;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    /// <summary>
    /// Removes a cached value associated with the specified key.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}
