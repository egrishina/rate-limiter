using Route256.RateLimiter.Models;
using Route256.RateLimiter.Options;

namespace Route256.RateLimiter.Storage;

public interface IClientStatisticsStorage
{
    Task<ClientStatistics> GetValueAsync(string key);
    Task UpdateValueAsync(string key, ClientStatistics value, RateLimitRule rule);
}