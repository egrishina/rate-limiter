using System.Collections.Concurrent;
using Route256.RateLimiter.Helpers;
using Route256.RateLimiter.Models;
using Route256.RateLimiter.Options;

namespace Route256.RateLimiter.Storage.MemoryCache;

public class MemoryCacheClientStatisticsStorage : IClientStatisticsStorage
{
    private readonly ConcurrentDictionary<string, ClientStatistics> _clients = new();

    public async Task<ClientStatistics> GetValueAsync(string key)
    {
        var value = new ClientStatistics();
        return _clients.GetOrAdd(key, value);
    }

    public async Task UpdateValueAsync(string key, ClientStatistics value, RateLimitRule rule)
    {
        var newValue = DateTime.UtcNow > value.Timestamp.Add(rule.Period.ToTimeSpan())
            ? new ClientStatistics()
            : value;

        newValue.IncrementCounter();
        _clients.AddOrUpdate(key, newValue, (k, stat) => newValue);
    }
}