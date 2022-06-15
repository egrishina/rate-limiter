using Route256.RateLimiter.Models;

namespace Route256.RateLimiter.Storage;

public interface IClientStatisticsStorage
{
    Task<ClientStatistics> GetValueAsync(string key);
    Task UpdateValueAsync(string key, ClientStatistics value);
}