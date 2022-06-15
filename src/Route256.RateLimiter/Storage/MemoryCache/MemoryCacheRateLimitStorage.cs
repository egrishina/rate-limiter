using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Route256.RateLimiter.Options;

namespace Route256.RateLimiter.Storage.MemoryCache;

public class MemoryCacheRateLimitStorage : IRateLimitStorage
{
    private readonly IMemoryCache _cache;
    private readonly RateLimitPolicies _policies;
    private readonly string _key = "IP_rules";

    public MemoryCacheRateLimitStorage(IMemoryCache cache, IOptions<RateLimitPolicies> policies)
    {
        _cache = cache;
        _policies = policies.Value;
    }
    
    public async Task SeedAsync()
    {
        await SetAsync(_policies).ConfigureAwait(false);
    }
    
    public Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(_key, out _));
    }

    public Task<RateLimitPolicies> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(_key, out RateLimitPolicies stored))
        {
            return Task.FromResult(stored);
        }

        return Task.FromResult(default(RateLimitPolicies));
    }

    public Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        _cache.Remove(_key);

        return Task.CompletedTask;
    }

    public Task SetAsync(RateLimitPolicies entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.NeverRemove
        };

        if (expirationTime.HasValue)
        {
            options.SetAbsoluteExpiration(expirationTime.Value);
        }

        _cache.Set(_key, entry, options);

        return Task.CompletedTask;
    }
}