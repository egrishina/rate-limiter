using System.Collections.Concurrent;
using Route256.RateLimiter.Models;

namespace Route256.RateLimiter.Storage.MemoryCache;

public class MemoryCacheClientStatisticsStorage : IClientStatisticsStorage
{
    private ConcurrentDictionary<string, ClientStatistics> _allData = new();

    public async Task<ClientStatistics> GetValueAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateValueAsync(string key, ClientStatistics value)
    {
        throw new NotImplementedException();
    }
}