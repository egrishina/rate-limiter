using Microsoft.Extensions.Options;
using Route256.RateLimiter.Helpers;
using Route256.RateLimiter.Models;
using Route256.RateLimiter.Options;
using Route256.RateLimiter.Storage;

namespace Route256.RateLimiter.Services;

public class RateLimitProcessor : IRateLimitProcessor
{
    private readonly RateLimitOptions _options;
    private readonly RateLimitPolicies _policies;
    private readonly IClientStatisticsStorage _clientStorage;

    public RateLimitProcessor(IOptions<RateLimitOptions> options, IOptions<RateLimitPolicies> policies,
        IClientStatisticsStorage clientStorage)
    {
        _options = options.Value;
        _policies = policies.Value;
        _clientStorage = clientStorage;
    }

    public async Task<IEnumerable<RateLimitRule>> GetMatchingRulesAsync(ClientRequestIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var limits = new List<RateLimitRule>();
        var individualRules = _policies.IpRules
            .Where(x => x.Ip == identity.ClientIp)
            .SelectMany(r => r.Rules)
            .ToList();
        
        if (individualRules.Any())
        {
            // search for rules with endpoints like "*" and "*:/matching_path"
            var path = $".+:{identity.Path}";
            var pathLimits = individualRules.Where(r => path.IsUrlMatch(r.Endpoint));
            limits.AddRange(pathLimits);
        }

        if (_options.GeneralRules.Any())
        {
            var generalLimits = new List<RateLimitRule>();
            // search for rules with endpoints like "*" and "*:/matching_path" in general rules
            var pathLimits = _options.GeneralRules.Where(r =>
                $"*:{identity.Path}".IsUrlMatch(r.Endpoint));
            generalLimits.AddRange(pathLimits);

            foreach (var generalLimit in generalLimits)
            {
                // add general rule if no specific rule is declared for the specified period
                if (!limits.Exists(l => l.Period == generalLimit.Period))
                {
                    limits.Add(generalLimit);
                }
            }
        }

        return limits;
    }

    public async Task<ClientStatistics> GetClientStatisticsAsync(ClientRequestIdentity identity,
        CancellationToken cancellationToken = default)
    {
        var key = $"{identity.Path}_{identity.ClientIp}";
        return await _clientStorage.GetValueAsync(key);
    }

    public async Task UpdateClientStatisticsAsync(ClientRequestIdentity identity, ClientStatistics clientStatistics,
        RateLimitRule rule, CancellationToken cancellationToken = default)
    {
        var key = $"{identity.Path}_{identity.ClientIp}";
        await _clientStorage.UpdateValueAsync(key, clientStatistics, rule);
    }
}