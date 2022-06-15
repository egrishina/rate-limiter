using Route256.RateLimiter.Models;
using Route256.RateLimiter.Options;

namespace Route256.RateLimiter.Core;

public interface IRateLimitProcessor
{
    Task<IEnumerable<RateLimitRule>> GetMatchingRulesAsync(ClientRequestIdentity identity,
        CancellationToken cancellationToken = default);

    Task<ClientStatistics> GetClientStatisticsAsync(ClientRequestIdentity requestIdentity,
        CancellationToken cancellationToken = default);

    Task UpdateClientStatisticsAsync(ClientRequestIdentity requestIdentity, ClientStatistics clientStatistics,
        CancellationToken cancellationToken = default);
}