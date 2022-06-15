using Route256.RateLimiter.Options;

namespace Route256.RateLimiter.Storage;

public interface IRateLimitStorage
{
    Task SeedAsync();
    Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    Task<RateLimitPolicies> GetAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(CancellationToken cancellationToken = default);
    Task SetAsync(RateLimitPolicies entry, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default);
}